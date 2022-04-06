using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FollowCamera))]
public class FollowCameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        GUILayout.Label("Horizontal Camera Borders");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("minX"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxX"));
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("Vertical Camera Borders");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("minY"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxY"));
        EditorGUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
    }
}