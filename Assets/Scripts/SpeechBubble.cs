using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeechBubble : MonoBehaviour
{
    private TMP_Text text;
    public float writeSpeed = 10f;
    public string flagName;
    [SerializeField] private Transform target;
    public Vector2 bubblePosition;
    public bool useTarget;
    private TextFlag textFlag;
    void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
        textFlag = GetComponentInChildren<TextFlag>();
    }
    public TextFlag GetTextFlag() => textFlag;
    public void SetFlagName(string name)
    {
        flagName = name;
        textFlag.SetFlagValue(LanguageManager.reader.GetDialogue(flagName));
    }
    public void StartSpeech()
    {
        WriteText(textFlag.FlagValue.CurrentText());
    }

    public void WriteText(string str)
    {
        StartCoroutine(TypeText(str));
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private IEnumerator TypeText(string text)
    {
        float t =  0;
        int charIndex = 0;

        while(charIndex < text.Length)
        {
            t += Time.deltaTime * writeSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, text.Length);

            this.text.text = text.Substring(0, charIndex);

            yield return null;
        }
        this.text.text = text;
    }
}
