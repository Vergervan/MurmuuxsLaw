using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static Reader reader = new Reader();
    void Awake()
    {
        ChangeLanguage("ru_RU");
    }
    public void ChangeLanguage(string fileName)
    {
        string fullFileName = string.Empty;
#if UNITY_EDITOR
            fullFileName = Application.dataPath + "/" + fileName;
#elif UNITY_STANDALONE
            fullFileName = System.IO.Directory.GetCurrentDirectory() + "\\" + fileName;
#endif
        if (reader.Filename == fileName) return;
        reader.Filename = fullFileName;
        reader.ReadFile();
    }
    public void LoadTextFlags(Scene scene)
    {
        List<TextFlag> flags = new List<TextFlag>();
        flags.AddRange(scene.GetComponentsInChildren<TextFlag>(true));
        foreach (var obj in scene.DependentObjects)
        {
            flags.AddRange(obj.GetComponentsInChildren<TextFlag>(true));
        }
        foreach (var flag in flags)
        {
            flag.SetFlagValue(reader.GetDialogue(flag.FlagName));
            flag.RefreshFlagValue();
        }
    }
}
