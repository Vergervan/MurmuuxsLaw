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
}

public class SaveFieldAttribute : Attribute { }

public class SavedDataHandler
{
    private static SavedDataHandler _instance = null;
    private List<ILoadSavedData> _data;
    private StringBuilder _newFile;

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

    }
    public void SaveData(string saveName)
    {
        _newFile = new StringBuilder();
        StringBuilder iterBuilder = new StringBuilder();
        lock (_data)
        {
            foreach (var item in _data)
            {
                Type type = item.GetType();
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    iterBuilder.Clear();
                    SaveFieldAttribute fieldAttribute = (SaveFieldAttribute)Attribute.GetCustomAttribute(field, typeof(SaveFieldAttribute));
                    if (fieldAttribute != null)
                    {
                        var fieldValue = field.GetValue(item);
                        iterBuilder.Append($"{field.Name}=");
                        if (fieldValue is IEnumerable)
                        {
                            IEnumerable arr = fieldValue as IEnumerable;
                            iterBuilder.Append('[');
                            int counter = 0;
                            foreach (var it in arr)
                            {
                                if(counter != 0)
                                {
                                    iterBuilder.Append(',');
                                }
                                iterBuilder.Append(it);
                                counter++;
                            }
                            if (counter == 0) continue;
                            iterBuilder.Append("]\n");
                        }
                        else
                        {
                            iterBuilder.Append($"{field.Name}={fieldValue}\n");
                        }
                        _newFile.Append(iterBuilder.ToString());
                    }
                }
            }
        }
        CreateFile(saveName);
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
