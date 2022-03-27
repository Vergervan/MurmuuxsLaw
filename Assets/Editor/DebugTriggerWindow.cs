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

    private void OnEnable()
    {
        var json = EditorPrefs.GetString("DebugTriggerWindowConfig", null);
        if (!string.IsNullOrWhiteSpace(json))
        {
            JsonUtility.FromJsonOverwrite(EditorPrefs.GetString("DebugTriggerWindowConfig"), this);
        }
    }

    private void OnDisable()
    {
        EditorPrefs.SetString("DebugTriggerWindowConfig",JsonUtility.ToJson(this));
    }

    private void OnGUI()
    {
        _actionTrigger = (ActionTrigger)EditorGUILayout.ObjectField(_actionTrigger, typeof(ActionTrigger), true, GUILayout.Width(240));
        if(_actionTrigger == null)
        {
            EditorGUILayout.HelpBox("You Must Set Up Action Trigger", MessageType.Error);
            return;
        }
        EditorGUILayout.BeginHorizontal();
        _showTriggers = EditorGUILayout.Foldout(_showTriggers, "Triggers");
        if(GUILayout.Button("Add Trigger", GUILayout.Width(120)))
        {
            if (_triggers == null)
            {
                _triggers = new List<string>();
            }
            _triggers.Add(string.Empty);
        }
        EditorGUILayout.EndHorizontal();
        if (_showTriggers && _triggers != null)
        {
            for(int i = 0; i < _triggers.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _triggers[i] = GUILayout.TextField(_triggers[i]);
                if(GUILayout.Button("✖", GUILayout.Width(20)))
                {
                    _triggers.Remove(_triggers[i]);
                    return;
                }
                if (GUILayout.Button("Call", GUILayout.Width(80)))
                {
                    _actionTrigger.TryToInvokeEvent(_triggers[i]);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
