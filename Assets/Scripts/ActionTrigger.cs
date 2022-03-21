using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using ActionType = ActionTrigger.TriggerEvent.ActionType;

[Serializable]
public class ActionTrigger : MonoBehaviour
{
    [Serializable]
    public class ActionTriggerInfo
    {
        public string triggerName;
        [SerializeField] private bool _showFoldout;
        [SerializeField] private List<TriggerEvent> events;
        private delegate void TriggerEventHandler();
        private event TriggerEventHandler handler;
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
            foreach(var evnt in events)
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
                }
            }
        }
        public void InvokeAllEvents()
        {
            handler?.Invoke();
        }
    }
    [Serializable]
    public class TriggerEvent
    {
        public enum ActionType
        {
            RemoveItem,
            Move, 
            Rotate
        }
        public ActionType type;
        public Inventory.ItemType item;
        public Transform target;
        public Inventory inventory;
        public Vector3 vector3;
        public float duration;
    }

    [SerializeField] private List<ActionTriggerInfo> _actions;
    public int ActionsCount => _actions.Count;
    public ICollection<ActionTriggerInfo> Actions => _actions;
    public void AddAction(ActionTriggerInfo info)
    {
        _actions.Add(info);
    }
    public void RemoveAction(ActionTriggerInfo info)
    {
        _actions.Remove(info);
    }
    public bool TryToInvokeEvent(string triggerName)
    {
        foreach(var item in _actions)
        {
            if(item.triggerName == triggerName)
            {
                item.InvokeAllEvents();
                return true;
            }
        }
        return false;
    }
}
