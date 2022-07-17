using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System;
using System.Linq;

using Debug = UnityEngine.Debug;

public interface ILoadSavedData
{
    void OnLoad();
}

public class SaveFieldAttribute : Attribute { }

public class SavedDataHandler
{
    private static SavedDataHandler _instance = null;
    private List<ILoadSavedData> _data;

    private SavedDataHandler()
    {
        _data = new List<ILoadSavedData>();
    }

    public void Subscribe(ILoadSavedData data)
    {
        _data.Add(data);
        Type type = data.GetType();
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            SaveFieldAttribute fieldAttribute = (SaveFieldAttribute)Attribute.GetCustomAttribute(field, typeof(SaveFieldAttribute));
            if(fieldAttribute != null)
            {
                var val = field.GetValue(data);
                if (val is IEnumerable)
                {
                    Debug.Log($"{field.Name} is collection");
                    int counter = 0;
                    foreach(var item in val as IEnumerable)
                    {
                        counter++;
                        Debug.Log($"{counter} {item}");
                    }
                    Debug.Log("Count: " + counter);
                }
            }
        }
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
    public void SaveData(string path)
    {
        
    }
}
