using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(FollowCamera))]
public class FollowCameraEditor : Editor
{
    SerializedProperty minXProp, maxXProp;
    private void OnEnable()
    {
        minXProp = serializedObject.FindProperty("minX");
        maxXProp = serializedObject.FindProperty("maxX");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        EditorGUILayout.LabelField("Horizontal Camera Borders");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(minXProp);
        EditorGUILayout.PropertyField(maxXProp);
        EditorGUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
    }
}