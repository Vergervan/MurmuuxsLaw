using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMenuManager : MonoBehaviour
{
    [SerializeField] private StackGroup menuObject;
    private RectTransform menuRect;
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
    private void Start()
    {
        menuRect = menuObject.GetComponent<RectTransform>();
    }
    public void ExecuteInPosition(Vector2 pos)
    {
        menuRect.localPosition = pos;
        if (!Enabled) Enabled = true;
    }
}
