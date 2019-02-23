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
    public string nextLevel;

    [Space]
    public int currentCutsceneId;
    public int currentAreaId;
    public int currentNodeId;
    public int currentBattleId;

    [Space]
    public List<bool> unlockedCutscenes;
    public List<bool> unlockedAreas;
    public List<bool> unlockedNodes;
    public List<bool> unlockedBattles;

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
        currentAreaId = -1;
        currentBattleId = -1;
        currentNodeId = -1;

        unlockedCutscenes = new List<bool>();
        unlockedAreas = new List<bool>();
        unlockedNodes = new List<bool>();
        unlockedBattles = new List<bool>();

        scavengerList = new List<int>();
        boosterList = new Dictionary<int, int>();

        nextLevel = "Cutscene";
    }

    public void UnlockCutscene()
    {
        unlockedCutscenes.Add(false);
    }

    public void FinishedCutscene()
    {
        unlockedCutscenes[currentCutsceneId] = true;
        currentCutsceneId++;
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

    public void FinishedArea(int areaId)
    {
        unlockedAreas[areaId] = true;
    }

    public void UnlockedNode()
    {
        unlockedNodes.Add(false);
    }

    public void FinishedNode(int nodeId)
    {
        unlockedNodes[nodeId] = true;
    }

    public void UnlockedBattle()
    {
        unlockedBattles.Add(false);
    }

    public void FinishedBattle(int battleId) 
    {
        unlockedBattles[battleId] = true;
    }
}
