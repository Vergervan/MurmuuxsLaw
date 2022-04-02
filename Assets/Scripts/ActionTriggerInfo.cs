using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ActionType = TriggerEvent.ActionType;
using UnityEngine.Events;

[System.Serializable]
public class ActionTriggerInfo
{
    public string triggerName;
    [SerializeField] private bool _showFoldout;
    [SerializeField] private List<TriggerEvent> events;
    private delegate void TriggerEventHandler();
    private event TriggerEventHandler handler;
    [SerializeField] private bool _useUnityEvent;
    [SerializeField] private UnityEvent _unityEvent;
    public ICollection<TriggerEvent> Events => events;
    public void AddEvent(TriggerEvent triggerEvent)
    {
        events.Add(triggerEvent);
    }
    public void RemoveEvent(TriggerEvent triggerEvent)
    {
        events.Remove(triggerEvent);
    }
    public void GenerateEvents()
    {
        if (events == null) return;
        foreach (var evnt in events)
        {
            switch (evnt.type)
            {
                case ActionType.RemoveItem:
                    if (evnt.inventory == null || evnt.item == Inventory.ItemType.Nothing) continue;
                    handler += () => evnt.inventory.RemoveItem(evnt.item);
                    break;
                case ActionType.Move:
                    if (evnt.target == null) continue;
                    handler += () => evnt.target.DOMove(evnt.vector3, evnt.duration);
                    break;
                case ActionType.Rotate:
                    if (evnt.target == null) continue;
                    handler += () => evnt.target.DORotate(evnt.vector3, evnt.duration);
                    break;
                case ActionType.SetCondition:
                    if (evnt.dialogManager == null) continue;
                    handler += () => evnt.dialogManager.SetConditionValue(evnt.name, evnt.boolValue);
                    break;
            }
        }
    }
    public void InvokeAllEvents()
    {
        Debug.Log(triggerName);
        _unityEvent?.Invoke();
        handler?.Invoke();
    }
}
