using UnityEngine;
using UnityEngine.EventSystems;

public class WindowBar : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform windowRect;
    [SerializeField] private Canvas canvas;
    private Vector2 upLeftBorder, downRightBorder;
    private RectTransform barRect;

    private void OnEnable()
    {
        Vector2 halfRes = canvas.pixelRect.size * 0.5f / canvas.scaleFactor;
        upLeftBorder = new Vector2(-halfRes.x, halfRes.y - barRect.sizeDelta.y);
        downRightBorder = new Vector2(halfRes.x - windowRect.sizeDelta.x, -halfRes.y + windowRect.sizeDelta.y);
    }
    private void Awake()
    {
        barRect = GetComponent<RectTransform>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector2 newSize = windowRect.anchoredPosition + eventData.delta / canvas.scaleFactor;
            newSize.x = Mathf.Clamp(newSize.x, upLeftBorder.x, downRightBorder.x);
            newSize.y = Mathf.Clamp(newSize.y, downRightBorder.y, upLeftBorder.y);
            windowRect.anchoredPosition = newSize;
        }
    }
}
