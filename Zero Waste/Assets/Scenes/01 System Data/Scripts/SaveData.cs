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
    public List<int> scavengerIdList;
    public Dictionary<int, int> boosterIdList;

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
        scavengerIdList = new List<int>();

        unlockedNodes = new Dictionary<Node, bool>();
        unlockedBattles = new Dictionary<Battle, bool>();

        currentCutscene = 0;
        currentArea = 0;
        currentBattleId = 0;
        currentNodeId = -1;
    }

    public void AddScavenger(int scavengerId)
    {
        scavengerIdList.Add(scavengerId);
    }

    public void DeleteScavenger(int scavengerId)
    {
        scavengerIdList.Remove(scavengerId);
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

    public void AddBattle(Battle battle)
    {
        unlockedBattles.Add(battle, false);
        currentBattleId = unlockedBattles.Count - 1;
    }

    public void FinishNode(Node node, bool status)
    {
        unlockedNodes[node] = status;
    }

    public void FinishBattle(Battle battle, bool status)
    {
        unlockedBattles[battle] = status;
    }
}
