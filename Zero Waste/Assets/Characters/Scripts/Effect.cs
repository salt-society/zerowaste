using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Abilities/Effect")]
public class Effect : ScriptableObject {

    public Sprite icon;
    public string effectName;

    [TextArea(1, 3)]
    public string description;

    [Space]
    public string target; // HP, ANT, ATK, DEF, SPD, PL
    public string type; // Direct | Status
    
    [Space]
    public int particleIndex;
    public bool particleLoop;

    [Space]
    public int strength;

    [Header("If Status")]
    public string state; // Buff | Debuff
    public string application; // Variable (HP, ANT, ATK, DEF, SPD) | Condition (Poison)
    public int duration;
    public int turnsRemaining;

    [Space]
    public string receiveAnim;
    public int effectIndex;
    public float animationLength;

}
