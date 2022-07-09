using UnityEngine;
using UnityEngine.EventSystems;

public class StretchWindow : MonoBehaviour, IDragHandler
{
    [SerializeField] private GameWindow window;
    [SerializeField] private Canvas canvas;
    private readonly Vector2 minSize = new Vector2(200, 200);
    private float allowedProx = 1f;

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (eventData.position.y < allowedProx || eventData.position.x > (canvas.pixelRect.width - allowedProx)) return;
            Vector2 newSize = window.Rect.sizeDelta + (new Vector2(eventData.delta.x, -eventData.delta.y) / canvas.scaleFactor);
            newSize.x = Mathf.Max(minSize.x, newSize.x);
            newSize.y = Mathf.Max(minSize.y, newSize.y);
            window.Rect.sizeDelta = newSize;
            window.Bar.UpdateBorders();
        }
    }
}
