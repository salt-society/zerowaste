using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData {

    [Header("Save Details")]
    public string fileName;
    public int playCount;

    public List<DateTime> launchDates;
    public DateTime dateCreated;
    public DateTime dateLastAccessed;

    [Space]
    public string playerName;
    public string gender;

    [Header("Game Progress")]
    public bool battleTutorial;
    public bool zwaTutorial;
    public bool mapTutorial;

    [Space]
    public int currentCutsceneId;
    public int currentAreaId;
    public int currentNodeId;
    public int currentBattleId;

    [Space]
    public int currentCutsceneCount;
    public int currentAreaCount;
    public int currentNodeCount;
    public int currentBattleCount;

    [Space]
    public List<bool> unlockedCutscenes;
    public List<bool> unlockedAreas;
    public List<bool> unlockedNodes;
    public List<bool> unlockedBattles;

    public Dictionary<int, bool> cutscenes;
    public Dictionary<int, bool> areas;
    public Dictionary<int, bool> nodes;
    public Dictionary<int, bool> battles;
    public Dictionary<int, bool> isBattlePlayed;

    [Header("Character Roster")]
    public List<int> scavengerList;
    public Dictionary<int, int> boosterList;

    public void InitializeSaveData()
    {
        dateCreated = DateTime.Now;
        dateLastAccessed = dateCreated;

        launchDates = new List<DateTime>();
        launchDates.Add(dateCreated);
        playCount++;

        SetDefaultValues();
    }

    public void SetDefaultValues()
    {
        currentCutsceneId = 0;
        currentAreaCount = -1;
        currentBattleId = -1;
        currentNodeCount = -1;

        unlockedCutscenes = new List<bool>();
        unlockedAreas = new List<bool>();
        unlockedNodes = new List<bool>();
        unlockedBattles = new List<bool>();

        cutscenes = new Dictionary<int, bool>();
        areas = new Dictionary<int, bool>();
        nodes = new Dictionary<int, bool>();
        battles = new Dictionary<int, bool>();
        isBattlePlayed = new Dictionary<int, bool>();

        scavengerList = new List<int>();
        boosterList = new Dictionary<int, int>();
    }

    public void ChangePlayDetails()
    {
        dateLastAccessed = DateTime.Now;
        launchDates.Add(dateLastAccessed);
        playCount++;
    }

    public void UnlockCutscene()
    {
        unlockedCutscenes.Add(false);
    }

    public void UnlockCutscene(int key, bool value)
    {
        cutscenes.Add(key, value);
        currentCutsceneId = key;
    }

    public void FinishedCutscene()
    {
        unlockedCutscenes[currentCutsceneId] = true;
        currentCutsceneId++;
    }

    public void FinishedCutscene(int cutsceneId)
    {
        cutscenes[cutsceneId] = true;
    }

    public void UnlockScavenger(int scavengerId)
    {
        if(!scavengerList.Contains(scavengerId))
        {
            scavengerList.Add(scavengerId);
        }
    }

    public void RemoveScavenger(int scavengerId)
    {
        scavengerList.Remove(scavengerId);
    }

    public void UnlockArea()
    {
        unlockedAreas.Add(false);
    }

    public void UnlockArea(int key, bool value)
    {
        areas.Add(key, value);
    }

    /*public void FinishedArea(int areaId)
    {
        unlockedAreas[areaId] = true;
    }*/

    public void FinishedArea(int areaId)
    {
        areas[areaId] = true;
    }

    public void UnlockedNode()
    {
        unlockedNodes.Add(false);
    }

    public void UnlockNode(int key, bool value)
    {
        nodes.Add(key, value);
    }

    /*public void FinishedNode(int nodeId)
    {
        unlockedNodes[nodeId] = true;
    }*/

    public void FinishedNode(int nodeId)
    {
        nodes[nodeId] = true;
    }

    public void UnlockedBattle()
    {
        unlockedBattles.Add(false);
    }

    public void UnlockBattle(int key, bool value)
    {
        battles.Add(key, value);
        isBattlePlayed.Add(key, value);
    }

    /*public void FinishedBattle(int battleId) 
    {
        unlockedBattles[battleId] = true;
    }*/

    public void PlayBattle(int battleId)
    {
        isBattlePlayed[battleId] = true;
    }

    public void FinishedBattle(int battleId)
    {
        battles[battleId] = true;
    }
}
