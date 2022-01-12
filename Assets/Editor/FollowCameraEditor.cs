using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FollowCamera))]
public class FollowCameraEditor : Editor
{
    float left = 0, right = 0;
    FollowCamera followCamera;
    private void OnEnable()
    {
        followCamera = (FollowCamera)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.LabelField("Horizontal Camera Borders");
        EditorGUILayout.BeginHorizontal();
        left = EditorGUILayout.FloatField(left);
        right = EditorGUILayout.FloatField(right);
        EditorGUILayout.EndHorizontal();
        followCamera.SetHorizontalCameraBorders(left, right);
        EditorUtility.SetDirty(followCamera);
    }
}
