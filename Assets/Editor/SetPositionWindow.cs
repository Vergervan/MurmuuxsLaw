using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SetPositionWindow : EditorWindow
{
    private GameObject objectPrefab;
    private GameObject obj;
    private bool _isui;
    private Canvas canvas;
    private Vector3 originPosition, currentPosition, originCameraPosition;
    private Transform _target;
    private EditorSceneManager sceneManager;
    public event EventHandler<Vector2> OnPositionSet;

    public static SetPositionWindow Init(Transform target)
    {
        var window = GetWindow<SetPositionWindow>("Set Position Window");
        window._target = target;
        window.originCameraPosition = Camera.main.transform.position;
        var targetPos = target.position;
        targetPos.z = Camera.main.transform.position.z;
        Camera.main.transform.position = targetPos;
        return window;
    }

    private void Update()
    {
        if (obj)
        {
            currentPosition = obj.transform.position;
            Repaint();
        }
    }

    private void OnDestroy()
    {
        if (obj)
            DestroyImmediate(obj);
        Camera.main.transform.position = originCameraPosition;
    }

    private void OnGUI()
    {
        GUILayout.Label(originPosition.ToString());
        GUILayout.Label(currentPosition.ToString());
        objectPrefab = (GameObject) EditorGUILayout.ObjectField(objectPrefab, typeof(GameObject), false);
        _isui = EditorGUILayout.Toggle("Is UI", _isui);
        if (_isui)
        {
            canvas = (Canvas)EditorGUILayout.ObjectField(canvas, typeof(Canvas), true);
        }
        if (GUILayout.Button("Create", GUILayout.Width(80)))
        {
            if (obj)
                DestroyImmediate(obj);
            obj = Instantiate(objectPrefab, _isui ? canvas.transform : null);
            originPosition = obj.transform.position;
        }
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Set", GUILayout.Width(80)))
        {
            OnPositionSet?.Invoke(this, obj.transform.position);
            obj.transform.position = originPosition;
        }
        if (GUILayout.Button("Cancel", GUILayout.Width(80)))
        {
            obj.transform.position = originPosition;
        }
        EditorGUILayout.EndHorizontal();
    }
}
