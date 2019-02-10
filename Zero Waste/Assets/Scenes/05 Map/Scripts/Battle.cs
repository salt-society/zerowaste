using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Battle", menuName = "Battle")]
public class Battle : ScriptableObject {

    [Header("Details")]
    public Sprite nodeIcon;

    [Space]
    public string battleId;
    public string battleName;
    public string info;

    [Space]
    public Areas area;

    [Header("Modifier")]
    public string targetParty;
    public Effect[] effects;

    [Header("Progess")]
    public string nextLevel;
}
