using UnityEngine;

public class Ability : ScriptableObject {

    [Header("Basic Ability Information")]
    public int index;

    [Space]
    public Sprite icon;
    public Sprite selectedIcon;
    public string abilityName;

    [Space]
    [Multiline]
    public string abilityDescription;
    public float length;

    [Space]
    public string type;
    public string range;

    [Space]
    public bool withDirect;
    public int directIndex;
    public bool showValue;

    [Space]
    public bool repeatAnimation;
    public int particleIndex;
    public float spawnY;

    [Header("Ability Effects")]
    public Effect[] effects;
}
