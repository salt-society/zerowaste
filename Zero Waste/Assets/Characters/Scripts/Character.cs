using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character : ScriptableObject {

    [Header("Basic Information")]
    public int characterId;
    public string characterType;

    [Space]
    public string characterName;
    public Sprite characterThumb;
    public Sprite characterHalf;
    public Sprite characterFull;
 
    [Space]
    public GameObject prefab;
    public Vector3 scale;

    [Header("Shared Information")]
    [Range(50, 150)] public int baseSpd;
    [HideInInspector] public int currentSpd;

    [Header("Abilities")]
    public Ability[] abilities;

    [Header("Status Effects")]
    public List<Effect> effects;

    public int CheckMin(int targetStat)
    {
        if (targetStat < 0)
            return 0;

        else
            return targetStat;
    }
}
