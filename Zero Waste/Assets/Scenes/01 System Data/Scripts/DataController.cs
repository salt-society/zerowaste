using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class DataController : MonoBehaviour
{
    [Header("Data Communication")]
    public static DataController dataControllerInstance;

    [Header("Game Data")]
    public GameData currentGameData;
    public SaveData currentSaveData;

    [Header("Cutscene")]
    public Effect[] battleModifiers;

    [Space]
    public string folderName;

    void Awake()
    {
        if (dataControllerInstance == null)
            dataControllerInstance = this;
        else if (dataControllerInstance != this)
            Destroy(this);

        DontDestroyOnLoad(gameObject);
    }

    public void CreateGameData()
    {
        string gameDataFileName = "ZeroWaste.Game";
        if (!Directory.Exists(Application.persistentDataPath + "/" + folderName))
        {
            string path = Application.persistentDataPath + "/" + folderName;
            Directory.CreateDirectory(path);

            GameData newGameData = new GameData();
            newGameData.gameDataDirectoryPath = path;
            newGameData.gameDataFileName = gameDataFileName;
            newGameData.InitializeGameData();
            this.currentGameData = newGameData;

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Create(path + "/" + gameDataFileName);
            binaryFormatter.Serialize(fileStream, newGameData);

            fileStream.Close();
        }
            
    }

    public void ReadGameData()
    {
        string gameDataFileName = "ZeroWaste.Game";
        if (Directory.Exists(Application.persistentDataPath + "/" + folderName))
        {
            string path = Application.persistentDataPath + "/" + folderName;

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(path + "/" + gameDataFileName, FileMode.Open);

            GameData currentGameData = binaryFormatter.Deserialize(fileStream) as GameData;
            this.currentGameData = currentGameData;
            fileStream.Close();
        }
    }

    public bool GameDataExists()
    {
        return File.Exists(Application.persistentDataPath + "/" + folderName + "/ZeroWaste.Game");
    }

    public void SaveGameDate()
    {
        string path = Application.persistentDataPath + "/" + folderName;

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(path + "/" + currentGameData.gameDataFileName);
        binaryFormatter.Serialize(fileStream, currentGameData);

        fileStream.Close();
    }

    public void NewSaveData()
    {
        string saveDataFileName = "SaveData0" + currentGameData.currentNoOfSaves + ".Save";
        if (Directory.Exists(Application.persistentDataPath + "/" + folderName))
        {
            string path = Application.persistentDataPath + "/" + folderName;

            if (!File.Exists(path + "/" + saveDataFileName))
            {
                SaveData newSaveData = new SaveData();
                newSaveData.fileName = saveDataFileName;
                newSaveData.InitializeSaveData();
                currentGameData.saves.Add(newSaveData);
                currentGameData.currentNoOfSaves++;
                currentGameData.currentSave = newSaveData;
                this.currentSaveData = newSaveData;

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = File.Create(path + "/" + saveDataFileName);
                binaryFormatter.Serialize(fileStream, newSaveData);
                fileStream.Close();

                SaveGameDate();
            }
        }
    }

    public void LoadSaveData(int saveNo)
    {
        string saveDataFileName = "SaveData0" + saveNo + ".Save";
        if (Directory.Exists(Application.persistentDataPath + "/" + folderName))
        {
            string path = Application.persistentDataPath + "/" + folderName;

            if (File.Exists(path + "/" + saveDataFileName))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = File.Open(path + "/" + saveDataFileName, FileMode.Open);

                SaveData currentSaveData = binaryFormatter.Deserialize(fileStream) as SaveData;
                this.currentSaveData = currentSaveData;
                currentGameData.currentSave = currentSaveData;

                fileStream.Close();
            }
        }
    }

    public void LoadSaveData(string saveDataFileName)
    {
        if (Directory.Exists(Application.persistentDataPath + "/" + folderName))
        {
            string path = Application.persistentDataPath + "/" + folderName;

            if (File.Exists(path + "/" + saveDataFileName))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = File.Open(path + "/" + saveDataFileName, FileMode.Open);

                SaveData currentSaveData = binaryFormatter.Deserialize(fileStream) as SaveData;
                this.currentSaveData = currentSaveData;
                currentGameData.currentSave = currentSaveData;

                fileStream.Close();
            }
        }
    }

    public bool SaveDataExists(string fileName)
    {
        return File.Exists(Application.persistentDataPath + "/" + folderName + "/" + fileName);
    }

    public void SaveSaveData()
    {
        string path = Application.persistentDataPath + "/" + folderName;

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(path + "/" + currentSaveData.fileName);
        binaryFormatter.Serialize(fileStream, currentSaveData);

        fileStream.Close();
    }
}
