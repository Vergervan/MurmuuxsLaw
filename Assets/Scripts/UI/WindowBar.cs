using UnityEngine;
using UnityEngine.EventSystems;

public class WindowBar : MonoBehaviour, IDragHandler
{
    [SerializeField] private GameWindow window;
    [SerializeField] private Canvas canvas;
    private Vector2 upLeftBorder, downRightBorder;
    private RectTransform barRect;
    private void Awake()
    {
        barRect = GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        UpdateBorders();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector2 newSize = window.Rect.anchoredPosition + eventData.delta / canvas.scaleFactor;
            newSize.x = Mathf.Clamp(newSize.x, upLeftBorder.x, downRightBorder.x);
            newSize.y = Mathf.Clamp(newSize.y, downRightBorder.y, upLeftBorder.y);
            window.Rect.anchoredPosition = newSize;
        }
    }
    public void UpdateBorders()
    {
        Vector2 halfRes = canvas.pixelRect.size * 0.5f / canvas.scaleFactor;
        upLeftBorder = new Vector2(-halfRes.x, halfRes.y - barRect.sizeDelta.y);
        downRightBorder = new Vector2(halfRes.x - window.Rect.sizeDelta.x, -halfRes.y + window.Rect.sizeDelta.y);
    }
}
