using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private bool isSpeak = false;
    private Coroutine hideBubble = null;
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
            if (npcBubble == null) npcBubble = dManager.CreateSpeechBubble(transform);
            npcBubble.gameObject.SetActive(true);
            DialogScriptCreator.Dialog dialog = dManager.GetDialog(npcSettings.dialogName);
            if (dialog.IsAnswer) throw new Exception("Can't use an answer as a dialog");
            if (dialog.IsDialog) dManager.SetRoutesInDialogWindow(dialog.Routes);
            npcBubble.SetDialog(dialog);
            isSpeak = true;
            npcBubble.StartSpeech();
            if(!npcBubble.GetTextFlag().FlagValue.HasNext())
                dManager.GetDialogController().GetDialogWindow().ToggleWindow();
        }
        else
        {
            UnitSpeech speech = npcBubble.GetTextFlag().FlagValue;
            if (!speech.HasNext())
            {
                return;
            }
            //if (nextIsNull)
            //{
            //    DisableBubble();
            //    npcBubble.GetTextFlag().FlagValue.SetToStart();
            //    isSpeak = false;
            //    return;
            //}
            npcBubble.StartSpeech();
            dManager.GetDialogController().GetDialogWindow().ToggleWindow();
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
