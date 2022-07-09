using UnityEngine;
using UnityEngine.EventSystems;

public class StretchWindow : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform windowRect;
    [SerializeField] private Canvas canvas;
    private readonly Vector2 minSize = new Vector2(200, 200);
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mousePos = Input.mousePosition;
        if (mousePos.y <= 5f) return; 
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            windowRect.sizeDelta += new Vector2(eventData.delta.x, -eventData.delta.y) / canvas.scaleFactor;
            Vector2 newSize = windowRect.sizeDelta;
            if(windowRect.sizeDelta.x < minSize.x)
            {
                newSize.x = minSize.x;
            }
            if(windowRect.sizeDelta.y < minSize.y)
            {
                newSize.y = minSize.y;
            }
            windowRect.sizeDelta = newSize;
        }
        Debug.Log($"{eventData.position}");
    }
}
