using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InGameButton))]
public class InGameButtonEditor : Editor
{
    InGameButton script;
    private void OnEnable()
    {
        script = (InGameButton)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        script.ButtonEnabled = EditorGUILayout.Toggle("Button Enabled", script.ButtonEnabled);
    }
}

