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
    public bool withDirectEffect;
    public int toSelfDirectEffect;
    public bool showValue;

    [Space]
    public bool repeatAnimation;
    public int castParticle;
    public bool customCoords;
    public bool cCoordX;
    public float spawnX;
    public bool cCoordY;
    public float spawnY;

    [Space]
    public string SFX;

    [Header("Ability Effects")]
    public Effect[] effects;
}
