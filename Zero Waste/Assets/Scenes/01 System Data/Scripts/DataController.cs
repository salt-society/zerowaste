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
    public string saveFolderName;

    [Header("Game Data")]
    public GameData currentGameData;
    public SaveData currentSaveData;

    [Space]
    public List<Player> allScavengersList;
    public List<Enemy> allWasteList;

    [Header("Player Roster")]
    public List<Player> scavengerRoster;

    [Space]
    public Cutscene currentCutscene;
    public string targetParty;
    public Effect[] battleModifiers;

    [Space]
    public Areas currentArea;
    public GameObject currentNodeObject;
    public Node currentNode;
    public GameObject currentBattleObject;
    public Battle currentBattle;

    [Space]
    public Player[] scavengerTeam;
    public Enemy[] wasteTeam;

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
        string path = Application.persistentDataPath + "/" + saveFolderName;
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

        Debug.Log("Creating game data...");
    }

    public void ReadGameData()
    {
        string gameDataFileName = "ZeroWaste.Game";
        if (Directory.Exists(Application.persistentDataPath + "/" + saveFolderName))
        {
            string path = Application.persistentDataPath + "/" + saveFolderName;

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(path + "/" + gameDataFileName, FileMode.Open);

            GameData currentGameData = binaryFormatter.Deserialize(fileStream) as GameData;
            this.currentGameData = currentGameData;
            currentSaveData = currentGameData.currentSave;
            fileStream.Close();

            Debug.Log("Reading game data...");
        }
    }

    public bool GameDataExists()
    {
        return File.Exists(Application.persistentDataPath + "/" + saveFolderName + "/ZeroWaste.Game");
    }

    public void SaveGameData()
    {
        string path = Application.persistentDataPath + "/" + saveFolderName;

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(path + "/" + currentGameData.gameDataFileName);
        binaryFormatter.Serialize(fileStream, currentGameData);

        fileStream.Close();

        Debug.Log("Saving game data...");
    }

    public void NewSaveData()
    {
        string saveDataFileName = "SaveData0" + currentGameData.currentNoOfSaves + ".Save";
        if (Directory.Exists(Application.persistentDataPath + "/" + saveFolderName))
        {
            string path = Application.persistentDataPath + "/" + saveFolderName;

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
                Debug.Log("Created save data...");
            }
        }
    }

    public void LoadSaveData(int saveNo)
    {
        string saveDataFileName = "SaveData0" + saveNo + ".Save";
        if (Directory.Exists(Application.persistentDataPath + "/" + saveFolderName))
        {
            string path = Application.persistentDataPath + "/" + saveFolderName;

            if (File.Exists(path + "/" + saveDataFileName))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = File.Open(path + "/" + saveDataFileName, FileMode.Open);

                SaveData currentSaveData = binaryFormatter.Deserialize(fileStream) as SaveData;
                this.currentSaveData = currentSaveData;
                currentGameData.currentSave = currentSaveData;
                fileStream.Close();
                
                SaveGameData();
                Debug.Log("Loading save data...");
            }
        }
    }

    public void LoadSaveData(string saveDataFileName)
    {
        if (Directory.Exists(Application.persistentDataPath + "/" + saveFolderName))
        {
            string path = Application.persistentDataPath + "/" + saveFolderName;

            if (File.Exists(path + "/" + saveDataFileName))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = File.Open(path + "/" + saveDataFileName, FileMode.Open);

                SaveData currentSaveData = binaryFormatter.Deserialize(fileStream) as SaveData;
                this.currentSaveData = currentSaveData;
                currentGameData.currentSave = currentSaveData;

                fileStream.Close();

                SaveGameData();
                Debug.Log("Loading save data...");
            }
        }
    }

    public bool SaveDataExists(string fileName)
    {
        return File.Exists(Application.persistentDataPath + "/" + saveFolderName + "/" + fileName);
    }

    public void SaveSaveData()
    {
        string path = Application.persistentDataPath + "/" + saveFolderName;

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(path + "/" + currentSaveData.fileName);
        binaryFormatter.Serialize(fileStream, currentSaveData);
        fileStream.Close();

        currentGameData.currentSave = currentSaveData;
        Debug.Log("Saving " + currentSaveData.fileName + "...");
    }

    public void AddScavenger(Player scavenger)
    {
        scavengerRoster.Add(scavenger);
    }

    public void SaveScavenger(Player scavenger)
    {
        string saveFileName = Path.GetFileNameWithoutExtension(currentSaveData.fileName);
        string fileName = saveFileName + "_Scavenger" + scavenger.characterId;

        if (Directory.Exists(Application.persistentDataPath + "/" + saveFolderName))
        {
            string path = Application.persistentDataPath + "/" + saveFolderName;

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Create(path + "/" + fileName + ".txt");

            var scavengerJson = JsonUtility.ToJson(scavenger);
            binaryFormatter.Serialize(fileStream, scavengerJson);

            currentSaveData.UnlockScavenger(scavenger.characterId);

            fileStream.Close();
        }
    }

    public void SaveScavengers()
    {
        foreach (Player scavenger in scavengerRoster)
            SaveScavenger(scavenger);
    }

    public void RemoveScavenger(Player scavenger)
    {
        scavengerRoster.Remove(scavenger);

        string saveFileName = Path.GetFileNameWithoutExtension(currentSaveData.fileName);
        string fileName = saveFileName + "_" + scavenger.characterId + ".txt";
        string scavengerPath = Application.persistentDataPath + "/" + saveFolderName + "/" + fileName;

        if (File.Exists(scavengerPath))
            File.Delete(scavengerPath);
    }

    public void LoadScavengers(List<int> scavengerIdList)
    {
        scavengerRoster = new List<Player>();
        foreach (int scavengerId in scavengerIdList)
        {
            string saveFileName = Path.GetFileNameWithoutExtension(currentSaveData.fileName);
            string fileName = saveFileName + "_Scavenger" + scavengerId + ".txt";

            Player scavengerToAdd = ScriptableObject.CreateInstance<Player>();
            foreach(Player scavenger in allScavengersList) 
            {
                if (scavenger.characterId.Equals(scavengerId))
                {
                    scavengerToAdd.name = scavenger.name;
                    break;
                }
                    
            }

            if (File.Exists(Application.persistentDataPath + "/" + saveFolderName + "/" + fileName))
            {
                string path = Application.persistentDataPath + "/" 
                    + saveFolderName + "/" + fileName;

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = File.Open(path, FileMode.Open);
                JsonUtility.FromJsonOverwrite((string)binaryFormatter.Deserialize(fileStream), scavengerToAdd);

                scavengerRoster.Add(scavengerToAdd);

                fileStream.Close();
            }
        }
    }

    public int GetNextSceneId(string nextLevel)
    {
        switch (nextLevel)
        {
            case "Splash Screen":
                {
                    return 0;
                }
            case "Loading Data":
                {
                    return 1;
                }
            case "Title Screen":
                {
                    return 2;
                }
            case "Cutscene":
                {
                    return 3;
                }
            case "ZWA":
                {
                    return 4;
                }
            case "Map":
                {
                    return 5;
                }
            case "Battle":
                {
                    return 6;
                }
            default:
                {
                    return -1;
                }
        }
    }
}
