﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Node", menuName = "Map/Node")]
public class Node : ScriptableObject
{
    [Header("Details")]
    public Sprite nodeIcon;
    public Sprite background;

    [Space]
    public int nodeId;
    public string nodeName;
    public string info;
    public string BGM;
    public bool isTutorial;
    public Color threatLevel;

    [Space]
    public WastePool wastePool;

    [Space]
    public bool doesUnlockScavenger;
    public int scavengerID;

    [Space]
    public bool isEpilogue;

    [Space]
    public Areas area;

    [Space]
    public List<int> battleIds;

    [Space]
    public bool hasPath;
    public Vector3 pathPosition;
    public Vector3 pathRotation;
    public Vector3 pathScale;

    [Space]
    public List<Battle> battles;

    [Header("Progess")]
    public List<string> nextLevels;
    public List<int> levelIds;

    [Space]
    public string nextScene;
}
