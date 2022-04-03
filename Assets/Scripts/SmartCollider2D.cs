using UnityEditor;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class SmartCollider2D : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
    }
}
