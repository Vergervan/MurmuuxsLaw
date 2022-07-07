using UnityEngine;

public class ItemMenuManager : MonoBehaviour
{
    [SerializeField] private RectTransform menuObject;
    [SerializeField] private Canvas canvas;
    [SerializeField] private bool _enabled;
    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            menuObject.gameObject.SetActive(value);
        }
    }
    public void ExecuteInPosition(Vector2 pos, Vector2 objectSize)
    {
        Vector2 menuScaledSize = menuObject.sizeDelta * menuObject.localScale;
        Debug.Log("Start pos: " + pos);
        pos.y += (((menuScaledSize.y + objectSize.y) * 0.5f) * canvas.scaleFactor);
        menuObject.transform.position = pos;
        Debug.Log("Processed pos: " + pos);
        if (!Enabled) Enabled = true;
    }
}
