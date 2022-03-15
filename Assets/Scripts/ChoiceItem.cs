using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ChoiceItem : MonoBehaviour
{
    [SerializeField] private TMP_Text textObj;
    [SerializeField] private RectTransform rectTransform;
    public RectTransform TextRect { get => rectTransform; }
    public string Text { get => textObj.text; set => textObj.text = value; }
    public TMP_Text TextScript { get => textObj; }
    void Awake()
    {
        textObj = GetComponent<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
    }
}
