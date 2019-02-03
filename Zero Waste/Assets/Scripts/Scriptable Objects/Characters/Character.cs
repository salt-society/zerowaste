using System.Collections.Generic;
using UnityEngine;

public class Character : ScriptableObject {

    [Header("Basic Information")]
    public Sprite characterThumb;
    public Sprite characterHalf;
    public Sprite characterFull;
    public string characterName;

    [Space]
    public GameObject prefab;
    public Vector3 scale;

    [Header("Shared Information")]
    [Range(1, 5)] public int baseSpd;
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
