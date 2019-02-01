using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Area", menuName = "Area")]
public class Areas : ScriptableObject {

    [Header("State of Level")]
    public bool isAreaLocked = true;

    [Space]
    public string parentName;

    [Space]
    [Header("Area Details")]
    public new string name;
    public string description;
    public int areaNumber;
    public Vector3 coordinates;

    [Space]
    [Header("Nodes")]
    public BattleInstance[] battles;

    [Space]
    [Header("Graphics")]
    public Sprite continent;
    
}
