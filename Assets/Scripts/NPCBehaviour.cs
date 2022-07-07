using System;
using System.Collections;
using UnityEngine;
using DialogScriptCreator;

public class NPCBehaviour : MonoBehaviour
{
    [Serializable]
    public class NPCSettings
    {
        public string dialogName;
        public string altDialogName;
        public int limitToAlt;
    }

    [SerializeField] private NPCSettings npcSettings;
    [SerializeField] private DialogueManager dManager;
    [SerializeField] private PlayerController player;
    private SpeechBubble npcBubble;
    private Dialog m_dialog, m_altdialog;
    private Dialog currentDialog;
    private bool isSpeak = false;
    private Coroutine hideBubble = null;

    private DialogueWindow _window;
    private void Start()
    {
        _window = dManager.GetDialogController().GetDialogWindow();
        m_dialog = dManager.GetDialog(npcSettings.dialogName);
        if(!string.IsNullOrWhiteSpace(npcSettings.altDialogName))
            m_altdialog = dManager.GetDialog(npcSettings.altDialogName);
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

        player.SetCameraZoom(transform, 1f);

        if (!isSpeak)
            StartSpeak();
        else
            ContinueSpeaking();
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
            if (Input.GetKeyDown(KeyCode.Return))
                ContinueSpeaking();
            if (Vector2.Distance(player.transform.position, transform.position) > 2f)
            {
                ResetSpeech();
                _window.TurnOff();
            }
        }
    }

    private void StartSpeak()
    {
        currentDialog = m_dialog;
        int num = currentDialog.GetAvailableRoutesCount(true);
        Debug.Log("Available routes: " + num);
        if (num <= npcSettings.limitToAlt)
            currentDialog = m_altdialog;
        if (npcBubble == null) npcBubble = dManager.CreateSpeechBubble(transform);
        npcBubble.gameObject.SetActive(true);
        if (currentDialog.IsAnswer) throw new Exception("Can't use an answer as a dialog");
        if (currentDialog.IsDialog) dManager.SetRoutesInDialogWindow(this, currentDialog.Routes);
        npcBubble.SetDialog(currentDialog);
        isSpeak = true;
        if (!npcBubble.GetTextFlag().FlagValue.HasNext() && currentDialog.IsDialog)
        {
            npcBubble.OnSpeechStop += (o, e) => _window.TurnOn();
        }
        npcBubble.StartSpeech();
    }

    private void ContinueSpeaking()
    {
        UnitSpeech speech = npcBubble.GetTextFlag().FlagValue;
        if (!speech.HasNext())
        {
            if (!_window.IsOpened)
            {
                DisableBubble();
                npcBubble.GetTextFlag().FlagValue.SetToStart();
                isSpeak = false;
                player.ResetZoom();
            }
            return;
        }
        speech.Next();
        Dialog currentDialog = npcBubble.GetCurrentDialog();
        if (!speech.HasNext() && currentDialog.IsDialog)
        {
            npcBubble.OnSpeechStop += (o, e) => _window.TurnOn();
        }
        if (!currentDialog.IsDialog)
        {
            _window.TurnOff();
        }
        npcBubble.StartSpeech();
    }

    public void ChangeDialog(Dialog newDialog)
    {
        currentDialog = newDialog;
        _window.TurnOff();
        if (newDialog.IsAnswer) throw new Exception("Can't use an answer as a dialog");
        npcBubble.SetDialog(newDialog);
        if (newDialog.IsDialog)
        {
            dManager.SetRoutesInDialogWindow(this, newDialog.Routes);
            npcBubble.OnSpeechStop += (o, e) => _window.TurnOn();
        }
        isSpeak = true;
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
