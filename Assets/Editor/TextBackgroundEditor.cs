using UnityEditor;

[CustomEditor(typeof(TextBackground))]
public class TextBackgroundEditor : Editor
{
    TextBackground textBackground;
    private void OnEnable()
    {
        textBackground = (TextBackground)target;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        textBackground.Text = EditorGUILayout.TextArea(textBackground.Text);

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(textBackground);
    }
}
