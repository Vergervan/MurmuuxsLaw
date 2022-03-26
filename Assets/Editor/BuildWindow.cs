using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildWindow : EditorWindow
{
    [SerializeField] private string buildPath;
    [SerializeField] private List<Object> scenesToBuild;
    [SerializeField] private List<Object> dialogScripts;
    [SerializeField] private List<Object> localeFiles;

    [SerializeField] private bool dialogScriptsFoldout, localeFilesFoldout;

    [MenuItem("Window/Custom Build Window")]
    public static void ShowWindow()
    {
        GetWindow<BuildWindow>("Custom Build");
    }
    private void OnGUI()
    {
        if (GUILayout.Button("Choose build folder", GUILayout.Width(150)))
        {
            buildPath = EditorUtility.OpenFolderPanel("Choose folder", string.Empty, string.Empty);
        }
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
    private void BuildAll()
    {
        if (Directory.Exists(buildPath))
        {
            Directory.Delete(buildPath, true);
        }
        BuildPipeline.BuildPlayer(new string[] { "Assets/Scenes/MainScene.unity" }, buildPath + "/MurmuuxsLaw.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
        CopyDialogScripts();
        CopyLocaleFiles();
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
            string assetPath = AssetDatabase.GetAssetPath(dialog);
            FileUtil.CopyFileOrDirectory(assetPath, scriptsPath + Path.GetFileName(assetPath));
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
            string assetPath = AssetDatabase.GetAssetPath(file);
            FileUtil.CopyFileOrDirectory(assetPath, localePath + Path.GetFileName(assetPath));
        }
    }
    private void ShowDialogScriptsGUI()
    {
        dialogScriptsFoldout = EditorGUILayout.Foldout(dialogScriptsFoldout, "Dialog Scripts");
        if (dialogScriptsFoldout)
        {
            if (GUILayout.Button("Add Dialog Script", GUILayout.Width(150)))
            {
                dialogScripts.Add(new Object());
            }
            for (int i = 0; i < dialogScripts.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                Object file = EditorGUILayout.ObjectField(dialogScripts[i], typeof(Object), false);
                string path = AssetDatabase.GetAssetPath(file);
                if (Path.GetExtension(path) == ".ds")
                {
                    dialogScripts[i] = file;
                }
                else
                {
                    dialogScripts[i] = null;
                }

                if (GUILayout.Button("✖"))
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
        if (localeFilesFoldout)
        {
            if (GUILayout.Button("Add Locale File", GUILayout.Width(150)))
            {
                localeFiles.Add(new Object());
            }
            for (int i = 0; i < localeFiles.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                localeFiles[i] = EditorGUILayout.ObjectField(localeFiles[i], typeof(Object), false);
                if (GUILayout.Button("✖"))
                {
                    localeFiles.RemoveAt(i);
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
