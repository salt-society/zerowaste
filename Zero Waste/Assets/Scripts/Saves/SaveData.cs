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
    public bool prologueFinish;
    public bool battleTutorial;

    public int currentCutscene;
    public int currentArea;
    public int currentNode;
    
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
        prologueFinish = false;
        battleTutorial = false;

        currentCutscene = 1;
        currentArea = 0;
        currentNode = 0;
    }
}
