using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InGameButton : MonoBehaviour
{
    [SerializeField] private UnityEvent action;
    [SerializeField] private bool allowScale;
    [SerializeField] private float scaleRatio;
    [SerializeField, HideInInspector]
    private bool _buttonEnabled = true;
    public bool ButtonEnabled
    {
        get => _buttonEnabled;
        set
        {
            _buttonEnabled = value;
            Color color = gameObject.GetComponent<SpriteRenderer>().color;
            color.a = value ? 1f : 0.5f;
            gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }
    [SerializeField] private bool IsToggle;
    private bool toggleState = false;
    [SerializeField] private UnityEvent altenrateAction;

    private Vector3 baseScale;
    void OnMouseDown()
    {
        if (!ButtonEnabled) return;
        if (IsToggle)
        {
            toggleState = !toggleState;
            if (toggleState)
                altenrateAction?.Invoke();
            else 
                action?.Invoke();
            return;
        }
        action?.Invoke();
    }
    private void OnMouseEnter()
    {
        if (!ButtonEnabled) return;
        if (allowScale)
        {
            baseScale = transform.localScale;
            transform.localScale *= scaleRatio;
        }
    }
    private void OnMouseExit()
    {
        if (!ButtonEnabled) return;
        transform.localScale = baseScale;
    }
    private void OnDisable()
    {
        if(baseScale != Vector3.zero) transform.localScale = baseScale;
    }
}
