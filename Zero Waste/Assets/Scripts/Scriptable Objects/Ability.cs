using UnityEngine;

public class Ability : ScriptableObject {

    [Header("Basic Ability Information")]
    public Sprite abilityIcon;
    public string abilityName;
    public string abilityType;
    public string abilityRange;
    [Multiline]public string abilityDescription;

    [Header("Ability Effects")]
    public Effect[] effects;
}
