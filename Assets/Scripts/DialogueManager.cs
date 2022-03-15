using UnityEngine;
using DialogScriptCreator;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private DialogueController dialogueController;
    private readonly DialogScriptReader scriptReader = new DialogScriptReader();
    private string fileName = "main.ds";
    private void Awake()
    {
        string fullFileName = string.Empty;
#if UNITY_EDITOR
        fullFileName = $"{Application.dataPath}/{fileName}";
#elif UNITY_STANDALONE
        fullFileName = System.IO.Directory.GetCurrentDirectory() + "\\scripts\\" + fileName;
#endif
        Debug.Log($"Read a dialog script by path: {fullFileName}");
        if (!scriptReader.ReadScript(fullFileName))
        {
            Debug.LogError($"Can't read a dialog script {fileName}");
        }
    }
    public SpeechBubble CreateSpeechBubble(Transform target)
    {
        GameObject bubbleObj = Instantiate(speechBubblePrefab, transform);
        SpeechBubble bubble = bubbleObj.GetComponent<SpeechBubble>();
        bubble.SetTarget(target);
        return bubble;
    }
    public Dialog GetDialog(string dialogName) => scriptReader.GetDialogByName(dialogName);
    public void SetRoutesInDialogWindow(IEnumerable<Route> routes) => dialogueController.SetRoutes(routes);
}
