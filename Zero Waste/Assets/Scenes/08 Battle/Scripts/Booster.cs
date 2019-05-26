using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Booster", menuName = "Item/Booster")]
public class Booster : ScriptableObject
{
    public int boosterId;

    [Space]
    public Sprite icon;
    public Sprite selectedIcon;
    public string boosterName;
    public string type;
    public string description;
    public Sprite typeIcon;

    [Space]
    public int particleIndex;
    public Effect[] effects;
}
