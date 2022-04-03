using System;
using UnityEditor;
using UnityEngine;

public class SetPositionWindow : EditorWindow
{
    private class SetPositionConfig : ScriptableObject
    {
        public GameObject prefab;
        public bool isUI;
    }

    private SetPositionConfig _config;
    private GameObject obj;
    private Canvas canvas;
    private Vector3 originPosition, currentPosition, originCameraPosition;
    private Transform _target;
    public event EventHandler<Vector2> OnPositionSet;
    private Scene _scene;

    public static SetPositionWindow Init(Transform target)
    {
        var window = GetWindow<SetPositionWindow>("Set Position Window");
        window._target = target;
        window.originCameraPosition = Camera.main.transform.position;
        window.Start();
        //var targetPos = target.position;
        //targetPos.z = Camera.main.transform.position.z;
        //Camera.main.transform.position = targetPos;
        return window;
    }
    private void Update()
    {
        if (obj)
        {
            currentPosition = obj.transform.localPosition;
            Repaint();
        }
    }
    public void Start()
    {
        _scene = _target.GetComponentInParent<Scene>();
        canvas = _scene.GetComponentInChildren<Canvas>();
        if (!canvas)
        {
            foreach(var obj in _scene.DependentObjects)
            {
                canvas = obj.GetComponent<Canvas>();
                if (canvas) break;
            }
        }
        LoadMarker();
        Vector3 targetPos = _scene.CameraPosition;
        targetPos.z = Camera.main.transform.position.z;
        Camera.main.transform.position = targetPos;
    }
    private void OnDestroy()
    {
        if (obj)
            DestroyImmediate(obj);
        Camera.main.transform.position = originCameraPosition;
        Selection.activeGameObject = _target.gameObject;
        EditorUtility.SetDirty(_config);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void LoadMarker()
    {
        _config = AssetDatabase.LoadAssetAtPath<SetPositionConfig>("Assets/Editor/SetPositionConfig.asset");
        if (!_config)
        {
            _config = CreateInstance<SetPositionConfig>();
            AssetDatabase.CreateAsset(_config, "Assets/Editor/SetPositionConfig.asset");
            AssetDatabase.Refresh();
        }
    }

    private void OnGUI()
    {
        GUILayout.Label(originPosition.ToString());
        GUILayout.Label(currentPosition.ToString());
        _config.prefab = (GameObject) EditorGUILayout.ObjectField(_config.prefab, typeof(GameObject), false);
        _config.isUI = EditorGUILayout.Toggle("Is UI", _config.isUI);
        //if (_isui)
        //{
        //    canvas = (Canvas)EditorGUILayout.ObjectField(canvas, typeof(Canvas), true);
        //}

        if (GUILayout.Button("Create", GUILayout.Width(80)))
        {
            //Vector3 targetPos = cameraPosition;
            //targetPos.z = Camera.main.transform.position.z;
            //Camera.main.transform.position = targetPos;

            if (obj)
                DestroyImmediate(obj);
            obj = Instantiate(_config.prefab, _config.isUI ? canvas.transform : null);
            originPosition = obj.transform.localPosition;
            Selection.activeGameObject = obj;
        }
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Set", GUILayout.Width(80)))
        {
            OnPositionSet?.Invoke(this, obj.transform.localPosition);
        }
        if (GUILayout.Button("Reset", GUILayout.Width(80)))
        {
            obj.transform.localPosition = originPosition;
        }
        EditorGUILayout.EndHorizontal();
    }
}
