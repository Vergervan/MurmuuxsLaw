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
    public void ExecuteInPosition(Vector2 pos)
    {
        Debug.Log(pos);
        if (pos.y < menuObject.sizeDelta.y)
        {
            pos.y -= (pos.y - menuObject.sizeDelta.y);
        }
        menuObject.transform.position = pos;
        Debug.Log(pos);
        if (!Enabled) Enabled = true;
    }
}
