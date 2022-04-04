using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider2D)), System.Serializable, ExecuteInEditMode]
public class SmartCollider2D : MonoBehaviour
{
    private Collider2D _collider;
    [SerializeField] private Vector2 _originOffset, _toggledOffset, _originSize, _toggledSize;
    [SerializeField] private bool altOffset;
    [SerializeField] private bool _muteTrigger = false;
    public Collider2D Collider => _collider;
    public bool IsToggled => altOffset;

    private void Awake()
    {
        foreach (var item in GetComponents<Collider2D>())
        {
            if (!item.isTrigger)
            {
                _collider = item;
                break;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_muteTrigger) return;
        altOffset = true;
        Collider.offset = _toggledOffset;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_muteTrigger) return;
        altOffset = false;
        Collider.offset = _originOffset;
    }
}
