using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

using Debug = UnityEngine.Debug;

public class BuildWindow : EditorWindow
{
    [SerializeField] private string buildPath;
    [SerializeField] private List<Object> scenesToBuild;
    [SerializeField] private List<string> dialogScripts;
    [SerializeField] private List<string> localeFiles;
    [SerializeField] private bool dialogScriptsFoldout, localeFilesFoldout;

    [MenuItem("Window/Custom Build Window")]
    public static void ShowWindow()
    {
        GetWindow<BuildWindow>("Custom Build");
    }

    private void OnEnable()
    {
        if (File.Exists("custombuildsettings.json"))
        {
            var json = File.ReadAllText("custombuildsettings.json");
            JsonUtility.FromJsonOverwrite(json, this);
            Debug.Log("File exists: " + json);
            Debug.Log($"{dialogScripts.Count} {localeFiles.Count}");
            Repaint();
        }
        dialogScripts ??= new List<string>();
        localeFiles ??= new List<string>();
    }
    private void SaveSettings()
    {
        var json = JsonUtility.ToJson(this);
        using (var settings = File.CreateText("custombuildsettings.json"))
        {
            settings.Write(json);
            settings.Flush();
        }
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Choose a build folder", GUILayout.Width(150)))
        {
            buildPath = EditorUtility.OpenFolderPanel("Choose a folder", string.Empty, string.Empty);
        }
        if(GUILayout.Button("Save", GUILayout.Width(60)))
        {
            SaveSettings();
        }
        EditorGUILayout.EndHorizontal();
        bool hasPath = !string.IsNullOrWhiteSpace(buildPath);
        if (hasPath)
        {
            GUILayout.Label("Build Path: " + buildPath);
        }

        GUI.enabled = hasPath;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Build", GUILayout.Width(100)))
        {
            BuildAll();
            return;
        }
        GUI.enabled = true;
        if (GUILayout.Button("Show in Explorer", GUILayout.Width(150)))
        {
            Application.OpenURL($"file://{buildPath}");
        }
        EditorGUILayout.EndHorizontal();
        if (!hasPath)
        {
            EditorGUILayout.HelpBox("Select build folder", MessageType.Error);
        }

        ShowDialogScriptsGUI();
        ShowLocaleFilesGUI();
    }
    private void BuildAll(bool run = false)
    {
        if (Directory.Exists(buildPath))
        {
            Directory.Delete(buildPath, true);
        }
        string exePath = buildPath + "/MurmuuxsLaw.exe";
        CopyDialogScripts();
        CopyLocaleFiles();
        BuildPipeline.BuildPlayer(new string[] { "Assets/Scenes/MainScene.unity" }, exePath, BuildTarget.StandaloneWindows64, BuildOptions.None);
    }
    private void CopyDialogScripts()
    {
        string scriptsPath = buildPath + "/scripts/";
        if (!Directory.Exists(scriptsPath))
        {
            Directory.CreateDirectory(scriptsPath);
        }
        foreach (var dialog in dialogScripts)
        {
            if (dialog == null) continue;
            FileUtil.CopyFileOrDirectory(dialog, scriptsPath + Path.GetFileName(dialog));
        }
    }
    private void CopyLocaleFiles()
    {
        string localePath = buildPath + "/locale/";
        if (!Directory.Exists(localePath))
        {
            Directory.CreateDirectory(localePath);
        }
        foreach (var file in localeFiles)
        {
            if (file == null) continue;
            FileUtil.CopyFileOrDirectory(file, localePath + Path.GetFileName(file));
        }
    }
    private void ShowDialogScriptsGUI()
    {
        dialogScriptsFoldout = EditorGUILayout.Foldout(dialogScriptsFoldout, "Dialog Scripts");
        if (dialogScriptsFoldout && dialogScripts != null)
        {
            if (GUILayout.Button("Add Dialog Script", GUILayout.Width(150)))
            {
                dialogScripts.Add(null);
            }
            for (int i = 0; i < dialogScripts.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                Object file = EditorGUILayout.ObjectField(dialogScripts[i] == null ? null : AssetDatabase.LoadMainAssetAtPath(dialogScripts[i]), typeof(Object), false);

                if (file != null)
                {
                    string path = AssetDatabase.GetAssetPath(file);
                    dialogScripts[i] = (Path.GetExtension(path) == ".ds") ? path : null;
                }

                if (GUILayout.Button("✖", GUILayout.Width(40)))
                {
                    dialogScripts.RemoveAt(i);
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
    private void ShowLocaleFilesGUI()
    {
        localeFilesFoldout = EditorGUILayout.Foldout(localeFilesFoldout, "Locale Files");
        if (localeFilesFoldout && localeFiles != null)
        {
            if (GUILayout.Button("Add Locale File", GUILayout.Width(150)))
            {
                localeFiles.Add(null);
            }
            for (int i = 0; i < localeFiles.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                Object file = EditorGUILayout.ObjectField(localeFiles[i] == null ? null : AssetDatabase.LoadMainAssetAtPath(localeFiles[i]), typeof(Object), false);

                if (file != null)
                {
                    string path = AssetDatabase.GetAssetPath(file);
                    localeFiles[i] = (Path.GetExtension(path) == ".lc") ? path : null;
                }

                if (GUILayout.Button("✖", GUILayout.Width(40)))
                {
                    localeFiles.RemoveAt(i);
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
