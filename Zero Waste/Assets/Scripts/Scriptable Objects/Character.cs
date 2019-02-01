using System.Collections.Generic;
using UnityEngine;

public class Character : ScriptableObject {

    [Header("Basic Information")]
    public Sprite characterPortrait;
    public Sprite characterImage;
    public Sprite defeatedImage;
    public string characterName;

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
