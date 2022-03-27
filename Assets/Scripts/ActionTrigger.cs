using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionTrigger : MonoBehaviour
{
    [SerializeField] private List<ActionTriggerInfo> _actions;
    private void Awake()
    {
        foreach(var action in _actions)
        {
            action.GenerateEvents();
        }
    }
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
