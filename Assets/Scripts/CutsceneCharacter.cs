using DialogScriptCreator;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;

public class CutsceneCharacter : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogManager;
    [SerializeField] private int _excerptDelay;
    private SpeechBubble _bubble;
    private Dialog _dialog;
    private bool isSpeak = false;
    [SerializeField] private UnityEvent OnSpeechStop;

    public void StartDialog(string flagName)
    {
        if (!_bubble)
            _bubble = dialogManager.CreateSpeechBubble(transform);
        _bubble.SetFlagName(flagName);
        _bubble.gameObject.SetActive(true);
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
            await Task.Delay(_excerptDelay);
            if (!unit.HasNext()) break;
            unit.Next();
        }
        _bubble.gameObject.SetActive(false);
        OnSpeechStop?.Invoke();
    }
}
