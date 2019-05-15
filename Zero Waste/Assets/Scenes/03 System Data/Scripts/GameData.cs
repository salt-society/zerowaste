using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    [Header("Directory")]
    public string gameDataDirectoryPath;
    public string gameDataFileName;

    [Header("Game Launch")]
    public DateTime dateOfFirstPlay;

    [Space]
    public List<DateTime> launchDates;

    [Header("Save Files")]
    public SaveData currentSave = null;
    public List<SaveData> saves;
    public int maxNoOfSaveFiles;
    public int currentNoOfSaves;

    public void InitializeGameData()
    {
        dateOfFirstPlay = DateTime.Now;
        launchDates = new List<DateTime>();
        launchDates.Add(dateOfFirstPlay);

        maxNoOfSaveFiles = 3;
        currentNoOfSaves = 0;
        saves = new List<SaveData>();
    }

    public int NextSceneId(string nextLevel)
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
