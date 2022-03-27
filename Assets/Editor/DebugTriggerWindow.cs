using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugTriggerWindow : EditorWindow
{
    [SerializeField] private ActionTrigger _actionTrigger;
    [SerializeField] private bool _showTriggers;
    [SerializeField] private List<string> _triggers;

    [MenuItem("Tools/Debug Trigger Utility")]
    static void ShowWindow()
    {
        GetWindow<DebugTriggerWindow>("Debug Trigger Utility").ShowAuxWindow();
    }

    private void OnGUI()
    {
        _actionTrigger = (ActionTrigger)EditorGUILayout.ObjectField(_actionTrigger, typeof(ActionTrigger), true);
        if(_actionTrigger == null)
        {
            EditorGUILayout.HelpBox("You Must Set Up Action Trigger", MessageType.Error);
            return;
        }
        EditorGUILayout.BeginHorizontal();
        _showTriggers = EditorGUILayout.Foldout(_showTriggers, "Triggers");
        if(GUILayout.Button("Add Trigger"))
        {
            _triggers.Add(string.Empty);
        }
        EditorGUILayout.EndHorizontal();
        if (_showTriggers && _triggers != null)
        {
            foreach(var trigger in _triggers)
            {
                var str = trigger;
                EditorGUILayout.BeginHorizontal();
                str = GUILayout.TextField(trigger, "Trigger");
                if(GUILayout.Button("✖", GUILayout.Width(15)))
                {
                    _triggers.Remove(trigger);
                    return;
                }
                if (GUILayout.Button("Call"))
                {
                    _actionTrigger.TryToInvokeEvent(trigger);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
