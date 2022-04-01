using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionTrigger : MonoBehaviour
{
    private static Dictionary<string, ActionTriggerInfo> _globalActions = new Dictionary<string, ActionTriggerInfo>();
    [SerializeField] private List<ActionTriggerInfo> _actions;
    private void Awake()
    {
        foreach(var action in _actions)
        {
            action.GenerateEvents();
            if (!_globalActions.ContainsKey(action.triggerName) && !string.IsNullOrWhiteSpace(action.triggerName))
            {
                _globalActions.Add(action.triggerName, action);
            }
        }
    }
    public int ActionsCount => _actions == null ? 0 : _actions.Count;
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
        if (_globalActions.ContainsKey(triggerName))
        {
            _globalActions[triggerName].InvokeAllEvents();
            return true;
        }
        return false;
    }
}
