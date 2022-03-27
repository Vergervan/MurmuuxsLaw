using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StackGroup))]
public class StackGroupEditor : Editor
{
    private float left = 0, right = 0, up = 0, down = 0;
    [SerializeField] private bool useMargin;
    private StackGroup stackGroup;
    private void OnEnable()
    {
        stackGroup = (StackGroup)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        useMargin = EditorGUILayout.Toggle("Margin", useMargin);
        if (useMargin)
        {
            EditorGUILayout.LabelField("Horizontal");
            EditorGUILayout.BeginHorizontal();
            left = EditorGUILayout.FloatField(left);
            right = EditorGUILayout.FloatField(right);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Vertical");
            EditorGUILayout.BeginHorizontal();
            up = EditorGUILayout.FloatField(up);
            down = EditorGUILayout.FloatField(down);
            EditorGUILayout.EndHorizontal();
            stackGroup.SetMargin(left, right, up, down);
        }
        EditorUtility.SetDirty(stackGroup);
    }
}
