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
        // Date when Save was created
        dateCreated = DateTime.Now;
        dateLastAccessed = dateCreated;

        // Keep track of how much user plays by
        // recording dates game was launched
        launchDates = new List<DateTime>();
        launchDates.Add(dateCreated);
        playCount++;

        // Set values of game at start
        playerName = "Ryleigh Nieves";

        // Ids
        currentCutsceneId = -1;
        currentAreaId = -1;
        currentNodeId = -1;
        currentBattleId = -1;
    }

    public void LaunchGameDetails()
    {
        // Record last date game was accessed
        // Game is considered launched when it reached loading screen
        dateLastAccessed = DateTime.Now;

        launchDates.Add(dateLastAccessed);
        playCount++;
    }

    public void UnlockCutscene(int key, bool value)
    {
        // To prevent errors, make sure dictionary isn't empty
        // before adding it with values
        if (cutscenes == null)
        {
            cutscenes = new Dictionary<int, bool>();

            cutscenes.Add(key, value);
            currentCutsceneId = key;
        }
        else
        {
            // Also make sure to check if id(s) already in
            // dictionary before adding to avoid errors
            if (!cutscenes.ContainsKey(key))
            {
                cutscenes.Add(key, value);
                currentCutsceneId = key;
            }
        }
    }

    public void FinishedCutscene(int cutsceneId)
    {
        // Make sure key is existing before overriding values
        if(cutscenes.ContainsKey(cutsceneId))
            cutscenes[cutsceneId] = true;
    }

    public void UnlockScavenger(int scavengerId)
    {
        // To prevent errors, make sure dictionary isn't empty
        // before adding it with values
        if (scavengerList == null)
        {
            scavengerList = new List<int>();
            scavengerList.Add(scavengerId);
        }
        else
        {
            // Also make sure to check if id(s) already in
            // dictionary before adding to avoid errors
            if (!scavengerList.Contains(scavengerId))
                scavengerList.Add(scavengerId);
        }
    }

    public void RemoveScavenger(int scavengerId)
    {
        // Make sure key is existing before overriding values
        if(scavengerList.Contains(scavengerId))
            scavengerList.Remove(scavengerId);
    }

    public void AddBooster(int boosterKey, int quantity)
    {
        // To prevent errors, make sure dictionary isn't empty
        // before adding it with values
        if (boosterList == null)
        {
            boosterList = new Dictionary<int, int>();
            boosterList.Add(boosterKey, quantity);
        }
        else
        {
            // Adds booster to list if it isn't there yet
            if (!boosterList.ContainsKey(boosterKey))
            {
                boosterList.Add(boosterKey, quantity);
            }
            // If booster's already existing, quantity just adds up
            else 
            {
                boosterList[boosterKey] += quantity;
            }
        }
    }


    public void UseBooster(int boosterKey)
    {
        // Decrement booster count whenever player uses,
        // which should just once per turn
        if (boosterList.ContainsKey(boosterKey))
        {
            // Make sure to decrement only if there are boosters
            if (boosterList[boosterKey] > 0)
            {
                boosterList[boosterKey] -= 1;
            }
                
            // Check again quantity of booster 
            // and remove from list if 0
            if (boosterList[boosterKey] == 0)
            {
                boosterList.Remove(boosterKey);
            }
        }
    }

    public void UnlockArea(int key, bool value)
    {
        // To prevent errors, make sure dictionary isn't empty
        // before adding it with values
        if (areas == null)
        {
            areas = new Dictionary<int, bool>();
            areas.Add(key, value);
        }
        else
        {
            // Also make sure to check if id(s) already in
            // dictionary before adding to avoid errors
            if (!areas.ContainsKey(key))
            {
                areas.Add(key, value);
            }
        }
    }

    public void FinishedArea(int areaId)
    {
        // Make sure key is existing before overriding values
        if(areas.ContainsKey(areaId))
            areas[areaId] = true;
    }

    public void UnlockNode(int key, bool value)
    {
        // To prevent errors, make sure dictionary isn't empty
        // before adding it with values
        if (nodes == null)
        {
            nodes = new Dictionary<int, bool>();
            nodes.Add(key, value);
        }
        else
        {
            // Also make sure to check if id(s) already in
            // dictionary before adding to avoid errors
            if(!nodes.ContainsKey(key)) 
            {
                nodes.Add(key, value);
            }
        }
    }

    public void FinishedNode(int nodeId)
    {
        // Make sure key is existing before overriding values
        if(nodes.ContainsKey(nodeId))
            nodes[nodeId] = true;
    }

    public void UnlockBattle(int key, bool value)
    {
        // To prevent errors, make sure dictionary isn't empty
        // before adding it with values
        if (battles == null)
        {
            battles = new Dictionary<int, bool>();
            isBattlePlayed = new Dictionary<int, bool>();

            battles.Add(key, value);
            isBattlePlayed.Add(key, value);
        }
        else
        {
            // Also make sure to check if id(s) already in
            // dictionary before adding to avoid errors
            if (!battles.ContainsKey(key))
            {
                battles.Add(key, value);
                isBattlePlayed.Add(key, value);
            }
        } 
    }

    public void PlayBattle(int battleId)
    {
        // Make sure key is existing before overriding values
        if(isBattlePlayed.ContainsKey(battleId))
            isBattlePlayed[battleId] = true;
    }

    public void FinishedBattle(int battleId)
    {
        // Make sure key is existing before overriding values
        if(battles.ContainsKey(battleId))
            battles[battleId] = true;
    }
}
