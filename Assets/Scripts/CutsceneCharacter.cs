using DialogScriptCreator;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;

[RequireComponent(typeof(ActionTrigger))]
public class CutsceneCharacter : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogManager;
    [SerializeField] private int _excerptDelay;
    [SerializeField] private Vector2 _bubblePosition;
    private SpeechBubble _bubble;
    private Dialog _dialog;
    private bool _speaking = false;
    [SerializeField] private UnityEvent OnSpeechStop;

    //TODO Установка позиции облачка
    //Стрелочку для переключения под облачком

    public bool IsSpeaking => _speaking;

    public void StartDialog(string flagName)
    {
        if (!_bubble)
            _bubble = dialogManager.CreateSpeechBubble(transform);
        _bubble.transform.localPosition = _bubblePosition;
        _bubble.SetFlagName(flagName);
        _bubble.gameObject.SetActive(true);
        CallBubble();
    }

    private async void CallBubble()
    {
        UnitSpeech unit = _bubble.GetTextFlag().FlagValue;
        _speaking = true;
        while (true)
        {
            _bubble.StartSpeech();
            while (_bubble.IsTyping)
                await Task.Yield();
            await Task.Delay(_excerptDelay);
            if (!unit.HasNext()) break;
            unit.Next();
        }
        _speaking = false;
        _bubble.gameObject.SetActive(false);
        OnSpeechStop?.Invoke();
    }

    public void SetBubblePosition(Vector2 position)
    {
        _bubblePosition = position;
    }
}
