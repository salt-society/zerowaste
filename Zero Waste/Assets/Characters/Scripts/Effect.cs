using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Abilities/Effect")]
public class Effect : ScriptableObject {

    public Sprite effectIcon;
    public string effectName;

    [TextArea(1, 3)]
    public string effectDescription;

    [Space]
    public string effectTarget;
    public string effectType;
    public string effectState;
    public string application;
    
    [Space]
    public int effectStrength;
    public int effectDuration;
    public int turnsRemaining;

}
