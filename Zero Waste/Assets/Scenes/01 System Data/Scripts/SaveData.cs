using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData { // holds player progress

    [Header("Save Details")]
    public string fileName;
    public int playCount;

    public List<DateTime> launchDates;
    public DateTime dateCreated;
    public DateTime dateLastAccessed;

    [Header("Player Details")]
    public string playerName;
    public string gender;

    [Header("Game Progress")]
    public string nextLevel;

    public List<Cutscene> finishedCutscenes;
    public List<Areas> unlockedAreas;
    public List<Battle> unlockedTerreBattles;
    public List<Battle> unlockedMareBattles;
    public List<Battle> unlockedAtmosBattles;

    public int currentCutscene;
    public int currentArea;
    public int currentTerreBattles;
    public int currentMareBattles;
    public int currentAtmosBattles;
    
    public void InitializeSaveData()
    {
        dateCreated = DateTime.Now;
        dateLastAccessed = dateCreated;

        launchDates = new List<DateTime>();
        launchDates.Add(dateCreated);
        playCount++;

        SetDefault();
    }

    public void SetDefault()
    {
        nextLevel = "Cutscene";

        currentCutscene = 0;
        currentArea = 0;
        currentTerreBattles = 0;
        currentMareBattles = 0;
        currentAtmosBattles = 0;
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

    public void FinishCutscene(Cutscene cutscene)
    {
        finishedCutscenes.Add(cutscene);
        currentCutscene = finishedCutscenes.Count;
    }

    public void AddArea(Areas area)
    {
        unlockedAreas.Add(area);
        currentArea = unlockedAreas.Count;
    }

    public void AddBattle(Battle battle, string areaName)
    {
        if (areaName.Equals("Terre"))
        {
            unlockedTerreBattles.Add(battle);
            currentTerreBattles = unlockedTerreBattles.Count;
        }

        if (areaName.Equals("Mare"))
        {
            unlockedMareBattles.Add(battle);
            currentMareBattles = unlockedMareBattles.Count;
        }

        if (areaName.Equals("Atmos"))
        {
            unlockedAtmosBattles.Add(battle);
            currentAtmosBattles = unlockedAtmosBattles.Count;
        }
    }
}
