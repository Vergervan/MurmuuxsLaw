using UnityEngine;
using UnityEngine.EventSystems;

public class WindowBar : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform windowRect;
    [SerializeField] private Canvas canvas;

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            windowRect.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }
}
