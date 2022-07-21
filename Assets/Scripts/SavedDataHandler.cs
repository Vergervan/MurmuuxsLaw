using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System;
using System.Linq;

using Debug = UnityEngine.Debug;
using System.Text;
using System.IO;

public interface ILoadSavedData
{
    void OnLoad();
    string GetStringData(ref bool isArray);
}

public class SaveFieldAttribute : Attribute { }

public class SavedDataHandler
{
    private static SavedDataHandler _instance = null;
    private List<ILoadSavedData> _data;
    private StringBuilder _newFile;
    private Dictionary<string, string> values = new Dictionary<string, string>();

    private SavedDataHandler()
    {
        _data = new List<ILoadSavedData>();
    }

    public void Subscribe(ILoadSavedData data)
    {
        _data.Add(data);
    }
    public void Unsubscribe(ILoadSavedData data)
    {
        _data.Remove(data);
    }
    public static SavedDataHandler GetInstance()
    {
        if (_instance == null)
            _instance = new SavedDataHandler();
        return _instance;
    }

    public void LoadData(string path)
    {
        path += ".gsv";
        using(FileStream fs = new FileStream(path, FileMode.Open))
        {
            using(StreamReader sr = new StreamReader(fs))
            {
                string data = sr.ReadToEnd();
            }
        }
        values.Clear();
        Debug.Log("Save loaded");
    }
    public void SaveData(string saveName)
    {
        _newFile = new StringBuilder();
        lock (_data)
        {
            foreach (var item in _data)
            {
                Type type = item.GetType();
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    SaveFieldAttribute fieldAttribute = (SaveFieldAttribute)Attribute.GetCustomAttribute(field, typeof(SaveFieldAttribute));
                    if (fieldAttribute != null)
                    {
                        var fieldValue = field.GetValue(item);
                        if (!(fieldValue is ILoadSavedData)) continue;
                        ILoadSavedData dataToSave = fieldValue as ILoadSavedData;
                        bool arr = false;
                        string strData = dataToSave.GetStringData(ref arr);
                        if (string.IsNullOrEmpty(strData)) continue;
                        _newFile.Append(arr ? $"{field.Name}=[{strData}]\n" : $"{field.Name}={strData}\n");
                    }
                }
            }
        }
        CreateFile(saveName);
        Debug.Log("Game saved");
    }
    private void CreateFile(string saveName)
    {
        saveName += ".gsv";
#if UNITY_STANDALONE && !UNITY_EDITOR
        if (!Directory.Exists("saves"))
        {
            Directory.CreateDirectory("saves");
        }
        saveName = "./saves/" + saveName;
#endif
        using (FileStream fs = new FileStream(saveName, FileMode.OpenOrCreate))
        {
            byte[] buf = Encoding.UTF8.GetBytes(_newFile.ToString());
            fs.Write(buf, 0, buf.Length);
            fs.Flush();
        }
        _newFile.Clear();
    }
}
