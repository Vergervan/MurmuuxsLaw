using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugTriggerWindow : EditorWindow
{
    [System.Serializable]
    private class DebugTriggerWindowInfo : ScriptableObject
    {
        [SerializeReference] public ActionTrigger actionTrigger;
        public bool showTriggers;
        public List<string> triggers;
    }
    [SerializeField] private DebugTriggerWindowInfo _info;
    private const string configPath = "Assets/Editor/DebugTriggerWindowConfig.asset";

    [MenuItem("Tools/Debug Trigger Utility")]
    static void ShowWindow()
    {
        GetWindow<DebugTriggerWindow>("Debug Trigger Utility").ShowAuxWindow();
    }

    private void LoadAsset()
    {
        _info = AssetDatabase.LoadAssetAtPath<DebugTriggerWindowInfo>(configPath);
        if (!_info)
        {
            _info = CreateInstance<DebugTriggerWindowInfo>();
            AssetDatabase.CreateAsset(_info, configPath);
            AssetDatabase.Refresh();
        }
    }

    private void OnEnable()
    {
        LoadAsset();
    }

    private void OnDisable()
    {
        AssetDatabase.SaveAssets();
    }

    private void OnGUI()
    {
        _info.actionTrigger = (ActionTrigger)EditorGUILayout.ObjectField(_info.actionTrigger, typeof(ActionTrigger), true, GUILayout.Width(240));
        if(_info.actionTrigger == null)
        {
            EditorGUILayout.HelpBox("You Must Set Up Action Trigger", MessageType.Error);
            return;
        }
        EditorGUILayout.BeginHorizontal();
        _info.showTriggers = EditorGUILayout.Foldout(_info.showTriggers, "Triggers");
        if(GUILayout.Button("Add Trigger", GUILayout.Width(120)))
        {
            if (_info.triggers == null)
            {
                _info.triggers = new List<string>();
            }
            _info.triggers.Add(string.Empty);
        }
        EditorGUILayout.EndHorizontal();
        if (_info.showTriggers && _info.triggers != null)
        {
            for(int i = 0; i < _info.triggers.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _info.triggers[i] = GUILayout.TextField(_info.triggers[i]);
                if(GUILayout.Button("✖", GUILayout.Width(20)))
                {
                    _info.triggers.Remove(_info.triggers[i]);
                    return;
                }
                if (GUILayout.Button("Call", GUILayout.Width(80)))
                {
                    _info.actionTrigger.TryToInvokeEvent(_info.triggers[i]);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
