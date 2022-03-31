using DialogScriptCreator;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;

public class CutsceneCharacter : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogManager;
    private SpeechBubble _bubble;
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
        if (!_bubble)
            _bubble = dialogManager.CreateSpeechBubble(transform);
        _bubble.gameObject.SetActive(true);
        _bubble.SetFlagName(dialogName);
        isSpeak = true;
        CallBubble();
    }

    private async void CallBubble()
    {
        UnitSpeech unit = _bubble.GetTextFlag().FlagValue;
        while (true)
        {
            _bubble.StartSpeech();
            while (_bubble.IsTyping)
                await Task.Yield();
            await Task.Delay(1700);
            Debug.Log(unit.HasNext());
            Debug.Log(unit.ExcerptCount);
            if (!unit.HasNext()) break;
            unit.Next();
        }
        OnSpeechStop?.Invoke();
    }
}
