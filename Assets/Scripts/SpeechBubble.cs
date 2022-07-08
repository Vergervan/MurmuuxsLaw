using System.Collections;
using UnityEngine;
using TMPro;
using DialogScriptCreator;
using System;

[RequireComponent(typeof(TextFlag), typeof(TMP_Text))]
public class SpeechBubble : MonoBehaviour
{
    private TMP_Text text;
    [SerializeField] private float writeSpeed = 10f;
    [SerializeField] private string flagName;
    private Dialog _dialog;
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 bubblePosition;
    [SerializeField] private bool useTarget;
    private TextFlag textFlag;
    private bool m_typing = false;
    public event EventHandler OnSpeechStop;
    public bool IsTyping
    {
        get => m_typing;
    }
    void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
        textFlag = GetComponentInChildren<TextFlag>();
    }
    public TextFlag GetTextFlag() => textFlag;
    public void SetFlagName(string name)
    {
        flagName = name;
        textFlag.SetFlagName(name);
        textFlag.SetFlagValue(LanguageManager.reader.GetUnitSpeech(flagName));
    }
    public void SetDialog(Dialog dialog)
    {
        _dialog = dialog;
        textFlag.SetFlagValue(LanguageManager.reader.GetUnitSpeech(dialog.Value));
    }
    public Dialog GetCurrentDialog() => _dialog;
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

        m_typing = true;
        while(charIndex < text.Length)
        {
            t += Time.deltaTime * writeSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, text.Length);

            this.text.text = text.Substring(0, charIndex);

            yield return null;
        }
        OnSpeechStop?.Invoke(this, new EventArgs());
        ClearEvents();
        m_typing = false;
    }
    public void ClearEvents()
    {
        OnSpeechStop = null;
    }
}
