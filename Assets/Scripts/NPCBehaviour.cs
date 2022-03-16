using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DialogScriptCreator;

public class NPCBehaviour : MonoBehaviour
{
    [Serializable]
    public class NPCSettings
    {
        public string dialogName;
        public string altDialogName;
    }

    [SerializeField] private NPCSettings npcSettings;
    [SerializeField] private DialogueManager dManager;
    [SerializeField] private PlayerController player;
    private SpeechBubble npcBubble;
    private Dialog m_dialog;
    private Dialog currentDialog;
    private bool isSpeak = false;
    private Coroutine hideBubble = null;
    private void Start()
    {
        m_dialog = dManager.GetDialog(npcSettings.dialogName);
        currentDialog = m_dialog;
    }
    void OnMouseDown()
    {
        if (Vector2.Distance(player.transform.position, transform.position) > 2f)
        {
            if (npcBubble != null) npcBubble.gameObject.SetActive(false);
            return;
        }
        if (hideBubble != null) StopCoroutine(hideBubble);
        if (!isSpeak)
        {
            currentDialog = m_dialog;
            if (npcBubble == null) npcBubble = dManager.CreateSpeechBubble(transform);
            npcBubble.gameObject.SetActive(true);
            if (m_dialog.IsAnswer) throw new Exception("Can't use an answer as a dialog");
            if (m_dialog.IsDialog) dManager.SetRoutesInDialogWindow(this, m_dialog.Routes);
            npcBubble.SetDialog(m_dialog);
            isSpeak = true;
            if (!npcBubble.GetTextFlag().FlagValue.HasNext() && m_dialog.IsDialog)
            {
                npcBubble.OnSpeechStop += (o, e) => dManager.GetDialogController().GetDialogWindow().TurnOn();
            }
            npcBubble.StartSpeech();
        }
        else
        {
            UnitSpeech speech = npcBubble.GetTextFlag().FlagValue;
            if (!speech.HasNext())
            {
                if (currentDialog.IsMonolog)
                {
                    DisableBubble();
                    npcBubble.GetTextFlag().FlagValue.SetToStart();
                    isSpeak = false;
                }
                return;
            }
            speech.Next();
            if (!speech.HasNext() && npcBubble.GetCurrentDialog().IsDialog)
            {
                npcBubble.OnSpeechStop += (o, e) => dManager.GetDialogController().GetDialogWindow().TurnOn();
            }
            if (!npcBubble.GetCurrentDialog().IsDialog)
            {
                dManager.GetDialogController().GetDialogWindow().TurnOff();
            }
            npcBubble.StartSpeech();
        }
    }

    private void Update()
    {
        if (npcBubble)
        {
            Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
            pos.y *= 1.25f;
            npcBubble.transform.position = pos;
        }
        if (isSpeak)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > 2f)
            {
                ResetSpeech();
            }
        }
    }
    public void ChangeDialog(Dialog newDialog)
    {
        currentDialog = newDialog;
        dManager.GetDialogController().GetDialogWindow().TurnOff();
        if (newDialog.IsAnswer) throw new Exception("Can't use an answer as a dialog");
        if (newDialog.IsDialog) dManager.SetRoutesInDialogWindow(this, newDialog.Routes);
        npcBubble.SetDialog(newDialog);
        isSpeak = true;
        //if (!npcBubble.GetTextFlag().FlagValue.HasNext() && newDialog.IsDialog)
        //{
        //    npcBubble.OnSpeechStop += (o, e) => dManager.GetDialogController().GetDialogWindow().ToggleWindow();
        //}
        npcBubble.StartSpeech();
    }
    public void ResetSpeech()
    {
        StartCoroutine(HideBubble());
        npcBubble.GetTextFlag().FlagValue.SetToStart();
        isSpeak = false;
    }
    private IEnumerator HideBubble()
    {
        yield return new WaitForSeconds(3f);
        DisableBubble();
    }

    private void DisableBubble()
    {
        npcBubble.gameObject.SetActive(false);
        hideBubble = null;
    }
}
