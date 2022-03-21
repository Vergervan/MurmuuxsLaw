using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[Serializable]
public class ActionTrigger : MonoBehaviour
{
    [Serializable]
    public class ActionTriggerInfo
    {
        public string triggerName;
        public UnityEvent triggerEvent;
        [SerializeField] private bool _showFoldout;
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
                item.triggerEvent?.Invoke();
                return true;
            }
        }
        return false;
    }
}
