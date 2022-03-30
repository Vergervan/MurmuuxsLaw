using DialogScriptCreator;
using UnityEngine;

public class CutsceneCharacter : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogManager;
    private SpeechBubble _bubble;
    private Dialog m_dialog;

    void Start()
    {
        
    }
    public void StartDialog(string dialogName)
    {
        m_dialog = dialogManager.GetDialog(dialogName);
        if (_bubble == null)
            _bubble = dialogManager.CreateSpeechBubble(transform);
    }
}
