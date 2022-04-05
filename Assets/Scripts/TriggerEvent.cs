using UnityEngine;

[System.Serializable]
public class TriggerEvent
{
    public enum ActionType
    {
        RemoveItem,
        Move,
        Rotate,
        SetCondition
    }
    public enum AnimatorOption
    {
        Int,
        String,
        Trigger
    }
    public ActionType type;
    public Inventory.ItemType item;
    public Transform target;
    public Inventory inventory;
    public DialogueManager dialogManager;
    public Animator animator;
    public AnimatorOption animOption;
    public string name;
    public bool boolValue;
    public Vector3 vector3;
    public float duration;
}
