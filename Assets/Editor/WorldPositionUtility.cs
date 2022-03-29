using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WorldPositionUtility : EditorWindow
{
    [MenuItem("Tools/World Position Utility")]
    static void ShowWindow()
    {
        GetWindow<WorldPositionUtility>("World Position").ShowAuxWindow();
    }

    private void OnGUI()
    {
        GUILayout.Label(Event.current.mousePosition.ToString());
        GUILayout.Label($"{HandleUtility.GUIPointToWorldRay(Event.current.mousePosition)}");
    }
}
