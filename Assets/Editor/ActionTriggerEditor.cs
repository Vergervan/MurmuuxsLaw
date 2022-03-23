using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using ActionTriggerInfo = ActionTrigger.ActionTriggerInfo;
using TriggerEvent = ActionTrigger.TriggerEvent;
using ActionType = ActionTrigger.TriggerEvent.ActionType;

[CustomEditor(typeof(ActionTrigger))]
[CanEditMultipleObjects]
public class ActionTriggerEditor : Editor
{
    private ActionTrigger trigger;
    private readonly object lockObj = new object();
    private SerializedProperty _currentAction;
    private SerializedProperty _showFoldout;
    private SerializedProperty actions;

    void OnEnable()
    {
        trigger = (ActionTrigger)target;
        actions = serializedObject.FindProperty("_actions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.LabelField("Actions Count: " + trigger.ActionsCount);
        EditorGUILayout.Space();
        if (GUILayout.Button("Add Trigger"))
        {
            lock (lockObj)
            {
                trigger.AddAction(new ActionTriggerInfo());
                serializedObject.ApplyModifiedProperties();
                return;
            }
        }
        int counter = 0;
        lock (lockObj)
        {
            foreach (var item in trigger.Actions)
            {
                EditorGUILayout.Space();
                GUILine(1);
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Trigger Name:");
                item.triggerName = EditorGUILayout.TextField(item.triggerName);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                _currentAction = actions.GetArrayElementAtIndex(counter);
                _showFoldout = _currentAction.FindPropertyRelative("_showFoldout");
                EditorGUILayout.BeginHorizontal();
                _showFoldout.boolValue = GUILayout.Toggle(_showFoldout.boolValue, "Show Events");
                if(GUILayout.Button("Add Event"))
                {
                    trigger.Actions.ElementAt(counter).AddEvent(new TriggerEvent());
                }
                EditorGUILayout.EndHorizontal();
                if (_showFoldout.boolValue)
                {
                    var events = trigger.Actions.ElementAt(counter).Events;
                    if (events != null)
                    {
                        foreach (var evnt in events)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginHorizontal();
                            evnt.type = (ActionType)EditorGUILayout.EnumPopup(evnt.type);
                            if (GUILayout.Button("✖"))
                            {
                                trigger.Actions.ElementAt(counter).Events.Remove(evnt);
                                serializedObject.ApplyModifiedProperties();
                                return;
                            }
                            EditorGUILayout.EndHorizontal();
                            ShowEventFields(evnt);
                        }
                    }
                }
                EditorGUILayout.Space();

                if (GUILayout.Button("Remove Trigger"))
                {
                    trigger.RemoveAction(item);
                    break;
                }
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

    private void ShowEventFields(TriggerEvent _event)
    {
        switch (_event.type)
        {
            case ActionType.RemoveItem:
                EditorGUILayout.BeginHorizontal();
                _event.inventory = (Inventory)EditorGUILayout.ObjectField(_event.inventory, typeof(Inventory), true);
                _event.item = (Inventory.ItemType)EditorGUILayout.EnumPopup(_event.item);
                EditorGUILayout.EndHorizontal();
                break;
            case ActionType.Rotate:
            case ActionType.Move:
                _event.target = (Transform)EditorGUILayout.ObjectField(_event.target, typeof(Transform), true);
                EditorGUILayout.BeginHorizontal();
                _event.vector3.x = EditorGUILayout.FloatField(_event.vector3.x);
                _event.vector3.y = EditorGUILayout.FloatField(_event.vector3.y);
                _event.vector3.z = EditorGUILayout.FloatField(_event.vector3.z);
                EditorGUILayout.EndHorizontal();
                break;
            case ActionType.SetCondition:
                _event.dialogManager = (DialogueManager)EditorGUILayout.ObjectField(_event.dialogManager, typeof(DialogueManager), true);
                EditorGUILayout.BeginHorizontal();
                _event.name = EditorGUILayout.TextField(_event.name);
                _event.boolValue = GUILayout.Toggle(_event.boolValue, "Value");
                EditorGUILayout.EndHorizontal();
                break;
        }
    }
}
