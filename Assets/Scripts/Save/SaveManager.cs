using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class SaveManager : SingletonMonobehaviour<SaveManager>
{
    public GameObjectSave gameObjectSave;
    public List<ISaveable> iSaveableObjectList;

    protected override void Awake()
    {
        base.Awake();

        iSaveableObjectList = new List<ISaveable>();
    }

    public void LoadDataFromFile()
    {
        string filePath = Application.persistentDataPath + "/data.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            gameObjectSave = JsonConvert.DeserializeObject<GameObjectSave>(json);
            iSaveableObjectList[0].ISaveableLoad(gameObjectSave);
        }
    }

    public void SaveDataToFile()
    {
        gameObjectSave = iSaveableObjectList[0].ISaveableSave();

        string filePath = Application.persistentDataPath + "/data.json";
        string json = JsonConvert.SerializeObject(gameObjectSave);
        File.WriteAllText(filePath, json);

    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveDataToFile();
        }
    }

    void OnApplicationQuit()
    {
        SaveDataToFile();
    }
}