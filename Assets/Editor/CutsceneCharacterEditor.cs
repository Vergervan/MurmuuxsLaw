using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CutsceneCharacter))]
public class CutsceneCharacterEditor : Editor
{
    private SetPositionWindow _window;
    private CutsceneCharacter character;

    private void OnEnable()
    {
        character = (CutsceneCharacter)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("Set Bubble Position", GUILayout.Width(150)))
        {
            _window = SetPositionWindow.Init(character.transform);
            _window.OnPositionSet += (o, e) => character.SetBubblePosition(e);
            _window.Show();
        }
    }
}
