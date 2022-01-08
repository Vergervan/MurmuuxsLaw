using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InGameButton))]
public class InGameButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InGameButton script = (InGameButton)target;
        DrawDefaultInspector();
        script.ButtonEnabled = EditorGUILayout.Toggle("Button Enabled", script.ButtonEnabled);
    }
}

