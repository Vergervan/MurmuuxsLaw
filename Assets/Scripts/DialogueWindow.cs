using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueWindow : MonoBehaviour
{
    [SerializeField] private DialogueController controller;
    [SerializeField] private Image buttonBackground;
    [SerializeField] private Sprite altImage;

    [SerializeField] private RectTransform windowObject;
    [SerializeField] private RectTransform content;

    [SerializeField] private GameObject answerVariantPrefab;
    private Sprite baseSprite;
    private bool _opened = false;
    public bool IsOpened { get => _opened; }
    void Start()
    {
        baseSprite = buttonBackground.sprite;
    }
    //public void ToggleWindow()
    //{
    //    if (controller.ChoicesCount == 0) return;
    //    _opened = !_opened;
    //    buttonBackground.sprite = _opened ? altImage : baseSprite;
    //    windowObject.SetActive(_opened);
    //    if (_opened) 
    //        controller.SelectChoice(0);
    //}

    public void TurnOn()
    {
        if (controller.ChoicesCount == 0 || _opened) return;
        buttonBackground.sprite = altImage;
        Vector2 size = windowObject.sizeDelta;
        windowObject.sizeDelta = new Vector2(size.x, 0);
        content.gameObject.SetActive(false);
        windowObject.gameObject.SetActive(true);
        windowObject.DOSizeDelta(size, 0.2f).OnComplete(() => 
        {
            _opened = true;
            content.gameObject.SetActive(true);
            controller.SelectChoice(0);
        });
    }
    public void TurnOff()
    {
        if (!_opened) return;
        buttonBackground.sprite = baseSprite;
        Vector2 size = windowObject.sizeDelta;
        content.gameObject.SetActive(false);
        windowObject.DOSizeDelta(new Vector2(size.x, 0), 0.2f).OnComplete(() =>
        {
            _opened = false;
            windowObject.gameObject.SetActive(false);
            windowObject.sizeDelta = size;
        });
    }
}
