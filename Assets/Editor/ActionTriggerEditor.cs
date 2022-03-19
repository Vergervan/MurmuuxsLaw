using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using ActionTriggerInfo = ActionTrigger.ActionTriggerInfo;

[CustomEditor(typeof(ActionTrigger))]
[CanEditMultipleObjects]
public class ActionTriggerEditor : Editor
{
    private ActionTrigger trigger;
    private readonly object lockObj = new object();
    private int actionsCount;
    private SerializedProperty _event;
    private SerializedProperty actions;

    void OnEnable()
    {
        trigger = (ActionTrigger)target;
        actions = serializedObject.FindProperty("_actions");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Actions Count: " + trigger.ActionsCount);
        EditorGUILayout.Space();
        if (GUILayout.Button("Add Event"))
        {
            lock (lockObj)
            {
                trigger.AddAction(new ActionTriggerInfo());
                serializedObject.ApplyModifiedProperties();
                return;
            }
        }
        EditorGUILayout.Space();
        GUILine(1);
        EditorGUILayout.Space();
        int counter = 0;
        lock (lockObj)
        {
            foreach (var item in trigger.Actions)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Trigger Name:");
                item.triggerName = EditorGUILayout.TextField(item.triggerName);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                _event = actions.GetArrayElementAtIndex(counter).FindPropertyRelative("triggerEvent");
                EditorGUILayout.PropertyField(_event);
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Remove Event"))
                {
                    trigger.RemoveAction(item);
                    break;
                }
                EditorGUILayout.Space();
                GUILine(1);
                EditorGUILayout.Space();
                counter++;
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void GUILine(float height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, height);
        rect.height = height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}
