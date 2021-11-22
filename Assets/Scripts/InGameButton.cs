using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InGameButton : MonoBehaviour
{
    public UnityEvent action;
    [SerializeField] private bool allowScale;
    [SerializeField] private float scaleRatio;
    private Vector3 baseScale;
    void OnMouseDown()
    {
        action?.Invoke();
    }
    private void OnMouseEnter()
    {
        if (allowScale)
        {
            baseScale = transform.localScale;
            transform.localScale *= scaleRatio;
        }
    }
    private void OnMouseExit()
    {
        transform.localScale = baseScale;
    }
}
