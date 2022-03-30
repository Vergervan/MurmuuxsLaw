
using UnityEngine;

[System.Serializable]
class ActionTriggerEditorInfo : ScriptableObject
{
    public bool setupVector = false;
    public TriggerEvent currentEvent = null;
    public Vector3 prevVector;
}
