using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private SavedDataHandler dataHandler;
    void Awake()
    {
        dataHandler = SavedDataHandler.GetInstance();
    }

    public void SaveGame()
    {
        dataHandler.SaveData("test_save");
    }

    public void LoadSave()
    {
        dataHandler.LoadData("test_save");
    }
}
