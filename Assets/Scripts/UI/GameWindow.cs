using UnityEngine;

public class GameWindow : MonoBehaviour
{
    private StretchWindow _stretch;
    private WindowBar _bar;
    private RectTransform _rect;

    public StretchWindow Stretch => _stretch;
    public WindowBar Bar => _bar;
    public RectTransform Rect => _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _stretch = GetComponentInChildren<StretchWindow>(true);
        _bar = GetComponentInChildren<WindowBar>(true);
    }
}
