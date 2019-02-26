using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Battle", menuName = "Map/Battle")]
public class Battle : ScriptableObject {

    [Header("Details")]
    public Sprite nodeIcon;

    [Space]
    public int battleId;
    public string battleName;
    public string info;
    public Color threatLevel;

    [Space]
    public Node node;
    public WastePool wastePool;

    [Space]
    public bool isMajorCutscene;
    public bool cutsceneAtStart;
    public Cutscene startCutscene;

    [Space]
    public bool cutsceneAtEnd;
    public Cutscene endCutscene;

    [Header("Progess")]
    public string nextLevel;
}
