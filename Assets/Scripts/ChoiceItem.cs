using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class ChoiceItem : MonoBehaviour
{
    private TMP_Text textObj;
    private RectTransform rectTransform;
    public RectTransform TextRect { get => rectTransform; }
    public string Text { get => textObj.text; set => textObj.text = value; }
    public TMP_Text TextScript { get => textObj; }
    void Awake()
    {
        textObj = GetComponent<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
    }
}
