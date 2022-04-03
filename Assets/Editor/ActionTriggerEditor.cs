using System.Linq;
using UnityEditor;
using UnityEngine;
using ActionType = TriggerEvent.ActionType;

[CustomEditor(typeof(ActionTrigger))]
[CanEditMultipleObjects]
public class ActionTriggerEditor : Editor
{
    private ActionTriggerEditorInfo _info;
    private ActionTrigger trigger;
    private readonly object lockObj = new object();
    private SerializedProperty _currentAction;
    private SerializedProperty _showFoldout;
    private SerializedProperty actions;
    private SerializedProperty _useUnityEvent;
    private SerializedProperty _unityEvent;
    void OnEnable()
    {
        trigger = (ActionTrigger)target;
        actions = serializedObject.FindProperty("_actions");
        _info = AssetDatabase.LoadAssetAtPath<ActionTriggerEditorInfo>("Assets/Editor/ATEditorInfo.asset");
        if (!_info)
        {
            _info = CreateInstance<ActionTriggerEditorInfo>();
            AssetDatabase.CreateAsset(_info, "Assets/Editor/ATEditorInfo.asset");
            AssetDatabase.Refresh();
        }
    }

    private void OnDisable()
    {
        EditorUtility.SetDirty(_info);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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
                _showFoldout.boolValue = EditorGUILayout.Foldout(_showFoldout.boolValue, "Events");
                if(GUILayout.Button("Add Event"))
                {
                    trigger.Actions.ElementAt(counter).AddEvent(new TriggerEvent());
                }
                EditorGUILayout.EndHorizontal();
                _useUnityEvent = _currentAction.FindPropertyRelative("_useUnityEvent");
                if (_showFoldout.boolValue)
                {
                    _useUnityEvent.boolValue = GUILayout.Toggle(_useUnityEvent.boolValue, "Use UEvent");
                    if (_useUnityEvent.boolValue)
                    {
                        _unityEvent = _currentAction.FindPropertyRelative("_unityEvent");
                        EditorGUILayout.PropertyField(_unityEvent);
                    }
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
                DrawBaseVectorField(_event);

                bool isCurrentEvent = _event.Equals(_info.currentEvent);
                if (GUILayout.Button((_info.setupVector && isCurrentEvent) ? "Stop" : "Set Up"))
                {
                    if (!isCurrentEvent)
                    {
                        _info.currentEvent = _event;
                        _info.setupVector = true;
                        _info.prevVector = _event.type == ActionType.Move ? _event.target.position : _event.target.eulerAngles;
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }
                    if (_info.setupVector)
                    {
                        _event.vector3 = _event.type == ActionType.Move ? _event.target.position : _event.target.eulerAngles;
                        if (_info.prevVector == _event.vector3)
                        {
                            _event.vector3 = Vector3.zero;
                        }
                        switch (_event.type) 
                        {
                            case ActionType.Move:
                                _event.target.position = _info.prevVector;
                                break;
                            case ActionType.Rotate:
                                _event.target.eulerAngles = _info.prevVector;
                                break;
                        }
                        GUILayout.Label(_info.prevVector.ToString());
                        _info.setupVector = false;
                    }
                }
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
    private void DrawBaseVectorField(TriggerEvent _event)
    {
        EditorGUILayout.BeginHorizontal();
        _event.target = (Transform)EditorGUILayout.ObjectField(_event.target, typeof(Transform), true);
        GUILayout.Label("Duration:", GUILayout.Width(55));
        _event.duration = EditorGUILayout.FloatField(_event.duration);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Vector3:");
        _event.vector3.x = EditorGUILayout.FloatField(_event.vector3.x);
        _event.vector3.y = EditorGUILayout.FloatField(_event.vector3.y);
        _event.vector3.z = EditorGUILayout.FloatField(_event.vector3.z);
        EditorGUILayout.EndHorizontal();
    }
}
