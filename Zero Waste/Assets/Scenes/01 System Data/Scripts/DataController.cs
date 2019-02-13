using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class DataController : MonoBehaviour
{
    [Header("Data Controller")]
    public static DataController instance;

    [Header("Game Data")]
    public GameData currentGameData;
    public SaveData currentSaveData;

    [Space]
    public Node currentNode;
    public Battle currentBattle;

    [Space]
    public List<Player> scavengerRoster;
    public List<Enemy> wasteRoster;

    [Header("Cutscene")]
    public Effect[] battleModifiers;

    [Space]
    public string folderName;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
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

    public void SaveGameData()
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

                SaveGameData();
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

    public void SaveScavenger(Player player)
    {
        string saveFileName = Path.GetFileNameWithoutExtension(currentSaveData.fileName);
        string fileName = saveFileName + "_" + player.name;

        if (Directory.Exists(Application.persistentDataPath + "/" + folderName))
        {
            string path = Application.persistentDataPath + "/" + folderName;

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Create(path + "/" + fileName + ".txt");

            var scavengerJson = JsonUtility.ToJson(player);
            binaryFormatter.Serialize(fileStream, scavengerJson);

            fileStream.Close();
        }
    }

    public Player LoadScavenger(string fileName)
    {
        Player scavenger = new Player();
        if (!Directory.Exists(Application.persistentDataPath + "/" + folderName
            + "/" + currentSaveData.fileName + "_ScavengerRoster"))
        {
            string path = Application.persistentDataPath + "/" + folderName
            + "/" + currentSaveData.fileName + "_ScavengerRoster";

            if (File.Exists(path + "/" + fileName))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = File.Open(path + "/" + fileName, FileMode.Open);
                JsonUtility.FromJsonOverwrite((string)binaryFormatter.Deserialize(fileStream), scavenger);

                Debug.Log(scavenger.characterName);

                fileStream.Close();
            }
        }

        return scavenger;
    }
}
