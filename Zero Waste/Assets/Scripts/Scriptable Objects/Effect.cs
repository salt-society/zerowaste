using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Effect")]
public class Effect : ScriptableObject {

    public Sprite effectIcon;
    public string effectName;
    public string effectType;
    public string effectType2;
    public string effectTarget;
    public int effectStrength;
    public int effectDuration;
    public int turnsRemaining;

}
