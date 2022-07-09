using UnityEngine;
using UnityEngine.EventSystems;

public class StretchWindow : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform windowRect;
    [SerializeField] private Canvas canvas;
    private readonly Vector2 minSize = new Vector2(200, 200);
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (eventData.position.y < 1f) return;
            Vector2 newSize = windowRect.sizeDelta + (new Vector2(eventData.delta.x, -eventData.delta.y) / canvas.scaleFactor);
            if(newSize.x < minSize.x)
            {
                newSize.x = minSize.x;
            }
            if(newSize.y < minSize.y)
            {
                newSize.y = minSize.y;
            }
            windowRect.sizeDelta = newSize;
        }
    }
}
