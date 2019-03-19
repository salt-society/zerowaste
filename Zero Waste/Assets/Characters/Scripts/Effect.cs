using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Abilities/Effect")]
public class Effect : ScriptableObject {

    public Sprite effectIcon;
    public string effectName;

    [TextArea(1, 3)]
    public string effectDescription;

    [Space]
    public string effectTarget; // HP, ANT, ATK, DEF, SPD, PL
    public string effectType; // Direct | Status
    public string effectState; // Buff | Debuff
    public string effectApplication; // Variable (HP, ANT, ATK, DEF, SPD, PL up or down) | Condition (Poison)

    [Space]
    public string animationParameter;
    public int particleNo;
    
    [Space]
    public int effectStrength;
    public int effectDuration;
    public int turnsRemaining;

}
