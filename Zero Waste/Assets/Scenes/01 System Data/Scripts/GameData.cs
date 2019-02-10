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

    [Header("Scene Indentifier")]
    public int loading;
    public int title;
    public int cutscene;
    public int world;
    public int battle;

    public void InitializeGameData()
    {
        dateOfFirstPlay = DateTime.Now;
        launchDates = new List<DateTime>();
        launchDates.Add(dateOfFirstPlay);

        maxNoOfSaveFiles = 3;
        currentNoOfSaves = 0;
        saves = new List<SaveData>();

        loading = 1;
        title = 2;
        cutscene = 3;
        world = 4;
        battle = 5;
    }
}
