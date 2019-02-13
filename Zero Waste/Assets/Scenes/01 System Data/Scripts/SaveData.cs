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

    public Dictionary<Battle, bool> terreBattles;
    public Dictionary<Battle, bool> mareBattles;
    public Dictionary<Battle, bool> atmosBattles;

    public Dictionary<Node, bool> unlockedNodes;
    public Dictionary<Battle, bool> unlockedBattles;

    public int currentCutscene;
    public int currentArea;
    public int currentTerre;
    public int currentMare;
    public int currentAtmos;

    public int currentNodeId;
    public int currentBattleId;

    [Header("Character Roster")]
    public List<String> scavengerRoster;
    
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

        finishedCutscenes = new List<Cutscene>();
        scavengerRoster = new List<String>();

        terreBattles = new Dictionary<Battle, bool>();
        mareBattles = new Dictionary<Battle, bool>();
        atmosBattles = new Dictionary<Battle, bool>();

        unlockedNodes = new Dictionary<Node, bool>();
        unlockedBattles = new Dictionary<Battle, bool>();

        currentCutscene = 0;
        currentArea = 0;
        currentBattleId = 0;
        currentNodeId = -1;

        currentTerre = 0;
        currentMare = 0;
        currentAtmos = 0;

        
    }

    public void AddScavenger(String scavengerName)
    {
        scavengerRoster.Add(scavengerName);
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

    public void FinishCutscene()
    {
        currentCutscene++;
    }

    public void AddArea(Areas area)
    {
        unlockedAreas.Add(area);
        currentArea = unlockedAreas.Count;
    }

    public void AddNode(Node node)
    {
        unlockedNodes.Add(node, false);
        currentNodeId = unlockedNodes.Count - 1;
    }

    public void AddBattle(Battle battle, string areaName)
    {
        if (areaName.Equals("Terre"))
        {
            terreBattles.Add(battle, false);
            currentTerre = terreBattles.Count - 1;
            currentBattleId = unlockedBattles.Count - 1;
        }

        if (areaName.Equals("Mare"))
        {
            mareBattles.Add(battle, false);
            currentMare = mareBattles.Count - 1;
            currentBattleId = unlockedBattles.Count - 1;
        }

        if (areaName.Equals("Atmos"))
        {
            atmosBattles.Add(battle, false);
            currentAtmos = atmosBattles.Count - 1;
            currentBattleId = unlockedBattles.Count - 1;
        }
    }

    public void AddBattle(Battle battle)
    {
        unlockedBattles.Add(battle, false);
        currentBattleId = unlockedBattles.Count - 1;
    }

    public void FinishNode(Node node, bool status)
    {
        unlockedNodes[node] = status;
    }

    public void FinishBattle(Battle battle, string areaName, bool status)
    {
        if (areaName.Equals("Terre"))
        {
            terreBattles[battle] = status;
        }

        if (areaName.Equals("Mare"))
        {
            mareBattles[battle] = status;
        }

        if (areaName.Equals("Atmos"))
        {
            atmosBattles[battle] = status; 
        }
    }

    public void FinishBattle(Battle battle, bool status)
    {
        unlockedBattles[battle] = status;
    }
}
