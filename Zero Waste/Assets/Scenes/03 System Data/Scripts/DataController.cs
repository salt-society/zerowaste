using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class DataController : MonoBehaviour
{
    public static DataController instance;

    [Header("Game Mode")]
    public bool testing;
    public bool demo;

    [Space]
    public int nextScene;

    [Space]
    public string saveFolderName;
    public GameData currentGameData;
    public SaveData currentSaveData;

    [Header("Character Data")]
    public List<Player> allScavengersList;
    public List<Enemy> allWasteList;

    [Header("Player's Roster")]
    public List<Player> scavengerRoster;

    [Header("Item Data")]
    public List<Booster> boosters;

    [Header("Battle Data")]
    public List<Battle> allBattles;

    [Header("Battle Inputs")]
    public Cutscene currentCutscene;
    public string targetParty;
    public Effect[] battleModifiers;

    [Space]
    public Areas currentArea;
    public GameObject currentNodeObject;
    public Node currentNode;
    public GameObject currentBattleObject;
    public Battle currentBattle;

    [Header("Battle Team/Party")]
    public Player[] scavengerTeam;
    public int scavengerCount;
    public Enemy[] mutantTeam;
    public int mutantCount;
    public Booster[] boosterInBattle;

    [Header("ZWA - HQ")]
    public StoryLevel currentStory;

    [Header("ZWA - My Room")]
    public MutantInfo mutantInfo;
    public BoosterInfo boosterInfo;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        DontDestroyOnLoad(gameObject);
    }

    void OnApplicationQuit()
    {
        // TEMP
        DeleteAllData();
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
                newSaveData.saveId = currentGameData.saves.Count;
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

    public void DeleteAllData()
    {
        if (Directory.Exists(Application.persistentDataPath + "/" + saveFolderName))
            Directory.Delete(Application.persistentDataPath + "/" + saveFolderName, true);
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

    public void AddExp(Player scavengerToEnhance)
    {
        LevelRequirements levelReq = new LevelRequirements();
        foreach (Player scavenger in scavengerRoster)
        {
            if (scavenger.characterId.Equals(scavengerToEnhance.characterId))
            {
                scavenger.currentLevel = scavengerToEnhance.currentLevel;
                scavenger.currentExp = scavengerToEnhance.currentExp;

                // Save
                SaveScavenger(scavenger);
                SaveSaveData();
                SaveGameData();
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
            case "Disclaimer":
                {
                    return 1;
                }
            case "Loading":
                {
                    return 2;
                }
            case "Loading Data":
                {
                    return 3;
                }
            case "Title Screen":
                {
                    return 4;
                }
            case "Cutscene":
                {
                    return 5;
                }
            case "ZWA":
                {
                    return 6;
                }
            case "Map":
                {
                    return 7;
                }
            case "Battle":
                {
                    return 8;
                }
            default:
                {
                    return 0;
                }
        }
    }

    public void UnlockLevels(List<string> nextLevels, List<int> levelIds)
    {
        // Unlock different levels at a single time

        int index = 0;
        foreach (string level in nextLevels)
        {
            // Check what to unlock, get its corresponding id and unlock it
            // using the functions written on save data

            if (level.Equals("Area"))
            {
                currentSaveData.UnlockArea(levelIds[index], false);
                index++;
            }

            if (level.Equals("Node"))
            {
                currentSaveData.UnlockNode(levelIds[index], false);
                index++;
            }

            if (level.Equals("Battle"))
            {
                currentSaveData.UnlockBattle(levelIds[index], false);
                index++;
            }
        }

        // Save every time levels are unlocked
        SaveSaveData();
        SaveGameData();
    }

    public void AddScrap(int scrap)
    {
        currentSaveData.AddScrap(scrap);
        SaveSaveData();
        SaveGameData();
    }

    public void UseScrap(int scrap)
    {
        currentSaveData.UseScrap(scrap);
        SaveSaveData();
        SaveGameData();
    }
}
