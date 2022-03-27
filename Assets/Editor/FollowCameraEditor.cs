using UnityEditor;
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
        GUILayout.Label("Horizontal Camera Borders");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(minXProp);
        EditorGUILayout.PropertyField(maxXProp);
        EditorGUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
    }
}
