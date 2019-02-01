using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SystemData { // holds game settings
    // defaults values on launch
    private static bool IS_TUTORIAL_ON = true;
    private static bool TUTORIAL_FIN = false;

    private static SaveData CURRENT_SAVE_DATA = null;
    private static String CURRENT_SAVE_NAME = null;
    private static int MAX_SAVE_FILES = 30;

    private static bool SHOW_MAIN_CAR = false;
    private static int TYPE_SPEED = 2;

    private static bool IS_MUSIC_ON = true;

	// record first launch
    public DateTime firstLaunch = DateTime.Today;
    public bool isTutorialOn = IS_TUTORIAL_ON;
    public bool isTutorialFinished = TUTORIAL_FIN;

    // current save
    public SaveData currentSave = CURRENT_SAVE_DATA;
    public String currentSaveName = CURRENT_SAVE_NAME;
    public int maxSaveFiles = MAX_SAVE_FILES;

    // player settings
    // visual novel
    public bool showMainChar = SHOW_MAIN_CAR;
    public int typeSpeed = TYPE_SPEED;
    // gfx

    // sfx
    public bool isMusicOn = IS_MUSIC_ON;
}
