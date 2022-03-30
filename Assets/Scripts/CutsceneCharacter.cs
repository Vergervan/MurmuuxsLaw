using DialogScriptCreator;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneCharacter : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogManager;
    [SerializeField] private SpeechBubble _bubble;
    private Dialog _dialog;
    private bool isSpeak = false;
    [SerializeField] private UnityEvent OnSpeechStop;

    private void OnMouseDown()
    {
        if (isSpeak)
        {
            UnitSpeech unit = _bubble.GetTextFlag().FlagValue;
            if (!unit.HasNext())
            {
                _bubble.gameObject.SetActive(false);
                isSpeak = false;
                unit.SetToStart();
                OnSpeechStop?.Invoke();
                return;
            }
            _bubble.StartSpeech();
        }
    }

    public void StartDialog(string dialogName)
    {
        _dialog = dialogManager.GetDialog(dialogName);
        _bubble.SetDialog(_dialog);
        if (_bubble == null)
            _bubble = dialogManager.CreateSpeechBubble(transform);
        _bubble.gameObject.SetActive(true);
        _bubble.StartSpeech();
        isSpeak = true;
    }
}
