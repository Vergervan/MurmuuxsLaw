using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SmartCollider2D))]
public class SmartColliderEditor : Editor
{
    private bool _setupOrigin = false, _setupAlt = false;
    private SmartCollider2D _collider;

    private void OnEnable()
    {
        _collider = (SmartCollider2D)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_muteTrigger"));

        var originOffset = serializedObject.FindProperty("_originOffset");
        var originSize = serializedObject.FindProperty("_originSize");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(originOffset);
        if (GUILayout.Button(_setupOrigin ? "Stop" : "Set Up", GUILayout.Width(60)))
        {
            _setupOrigin = !_setupOrigin;
            if (!_setupOrigin)
            {
                originOffset.vector2Value = _collider.Collider.offset;
                originSize.vector2Value = _collider.Collider.GetComponent<BoxCollider2D>().size;
            }
        }
        EditorGUILayout.EndHorizontal();

        var altOffset = serializedObject.FindProperty("_toggledOffset");
        var altSize = serializedObject.FindProperty("_toggledSize");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(altOffset);
        if (GUILayout.Button(_setupAlt ? "Stop" : "Set Up", GUILayout.Width(60)))
        {
            _setupAlt = !_setupAlt;
            if (!_setupAlt)
            {
                altOffset.vector2Value = _collider.Collider.offset;
                altSize.vector2Value = _collider.Collider.GetComponent<BoxCollider2D>().size;
            }
        }
        EditorGUILayout.EndHorizontal();

        if(GUILayout.Button("Set To Origin", GUILayout.Width(150)))
        {
            _collider.Collider.offset = originOffset.vector2Value;
            _collider.Collider.GetComponent<BoxCollider2D>().size = originSize.vector2Value;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
