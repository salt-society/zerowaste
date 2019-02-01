using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData { // holds player progress
    // game defaults upon play
    public static string DEF_FIRST_NAME = "Ryleigh";
    public static string DEF_LAST_NAME = "Nieves";
    public static string DEF_PLAYER_GENDER = "Male";

    public static int INITIAL_ENERGY_CAP = 30;
    public static int INITIAL_SCRAPS = 0;
    public static int INITIAL_HEALING_CRYSTALS = 0;

    public static int INITIAL_LEVEL = 1;
    public static int MAX_LEVEL = 100;
    public static double INITIAL_EXP = 0.00d;
    public static double INITIAL_EXP_CAP = 100.00d;

    public static double OVERALL_GAME_COMPLETION = 0;
    public static int AREAS_UNLOCKED = 5;
    public static double SRILANKA_COMPLETION = 0;
    public static int SRILANKA_NODES_UNLOCKED = 0;
    public static double GUYANA_COMPLETION = 0;
    public static int GUYANA_NODES_UNLOCKED = 0;
    public static double STKITTSNEVIS_COMPLETION = 0;
    public static int STKITTSNEVIS_NODES_UNLOCKED = 0;
    public static double KUWAIT_COMPLETION = 0;
    public static int KUWAIT_NODES_UNLOCKED = 0;

    public static string FILE_NAME = "SaveData_.save";
    public static string LAST_SAVE_DATE = "00/00/0000 00:00:00 AM";
    public static bool IS_SAVE_EMPTY = true;

    // basic player info
    public string firstName = DEF_FIRST_NAME;
    public string lastName = DEF_LAST_NAME;
    public string gender = DEF_PLAYER_GENDER;

    // game resources
    public int currentEnergy = INITIAL_ENERGY_CAP;
    public int energyCap = INITIAL_ENERGY_CAP;
    public int currencyScraps = INITIAL_SCRAPS;
    public int healingCrystals = INITIAL_HEALING_CRYSTALS;

    // player game progress
    public int level = INITIAL_LEVEL; // level of player
    public double experience = INITIAL_EXP;
    public double expCap = INITIAL_EXP_CAP;

    public double gameCompletion = OVERALL_GAME_COMPLETION; // story
    public int areasUnlocked = AREAS_UNLOCKED;
    // sri lanka
    public double sriLankaCompletion = SRILANKA_COMPLETION;
    // guyana
    public double guyanaCompletion = GUYANA_COMPLETION;
    // st. kitts and nevis
    public double stKittsNevisCompletion = STKITTSNEVIS_COMPLETION;
    // kuwait
    public double kuwaitCompletion = KUWAIT_COMPLETION;

    // roster and game items
    // booster, scavenger, materials

    // save details
    public string fileName = FILE_NAME;
    public string lastSaveDate = LAST_SAVE_DATE;
    public bool isSaveEmpty = IS_SAVE_EMPTY;
}
