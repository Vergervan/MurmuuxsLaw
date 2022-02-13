using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Margin
{
    public float left, right, up, down;
}

[ExecuteInEditMode]
public class StackGroup : MonoBehaviour
{
    [SerializeField] private float maxWidth;
    [SerializeField] private float spacing;

    [SerializeField] private RectTransform rect;
    [SerializeField] private Margin margin = new Margin();
    private void Start()
    {
        rect = transform.GetComponent<RectTransform>();
    }
    void Update()
    {
        float offset = 0f;
        foreach(var obj in transform.GetComponentsInChildren<RectTransform>())
        {
            if (obj == rect || obj.parent != rect) continue;
            obj.anchoredPosition = new Vector2(obj.sizeDelta.x/2, 0 - offset - (offset == 0f ? 0f : spacing));
            obj.sizeDelta = new Vector2(maxWidth, obj.sizeDelta.y + margin.up);
            offset += obj.sizeDelta.y;
        }
    }
    public void AddItem(GameObject obj)
    {
        Instantiate(obj, transform);
    }
    public void SetMargin(float left, float right, float up, float down)
    {
        margin.left = left;
        margin.right = right;
        margin.down = down;
        margin.up = up;
    }
}
