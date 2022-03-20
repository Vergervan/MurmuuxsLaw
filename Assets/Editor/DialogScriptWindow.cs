using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class DialogScriptWindow : EditorWindow
{
    private string scriptText;
    private string scriptPath;
    private FileStream ifs;

    [MenuItem("Window/Dialog Script Editor")]
    public static void ShowWindow()
    {
        GetWindow<DialogScriptWindow>("Dialog Script Editor");
    }

    private void OnProjectChange()
    {
        scriptText = string.Empty;
        scriptPath = string.Empty;
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Open Script"))
        {
            scriptPath = EditorUtility.OpenFilePanel("Choose script file", Application.dataPath, "ds");
            byte[] buffer = { };
            using (ifs = new FileStream(scriptPath, FileMode.Open, FileAccess.Read))
            {
                int numBytesToRead = (int)(ifs.Length), numBytesRead = 0;
                buffer = new byte[numBytesToRead];
                while (numBytesToRead > 0)
                {
                    int n = ifs.Read(buffer, numBytesRead, numBytesToRead);
                    numBytesRead += n;
                    numBytesToRead -= n;
                }
            }
            scriptText = Encoding.UTF8.GetString(buffer);
        }
        if (!string.IsNullOrWhiteSpace(scriptPath))
        {
            GUILayout.Label(scriptPath);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                byte[] toWrite = Encoding.UTF8.GetBytes(scriptText);
                File.WriteAllText(scriptPath, string.Empty);
                using (ifs = new FileStream(scriptPath, FileMode.Open, FileAccess.Write))
                {
                    ifs.Write(toWrite, 0, toWrite.Length);
                    ifs.Flush();
                }
            }
            if (GUILayout.Button("Clear All"))
            {
                File.WriteAllText(scriptPath, string.Empty);
                scriptText = "";
            }
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.Space();
        EditorStyles.textField.wordWrap = true;
        EditorGUILayout.TextArea(scriptText);
    }
}
