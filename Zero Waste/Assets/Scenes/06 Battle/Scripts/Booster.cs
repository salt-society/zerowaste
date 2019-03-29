using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Booster", menuName = "Item/Booster")]
public class Booster : ScriptableObject
{
    public Sprite icon;
    public string boosterName;
    public string type;

    [Space]
    public int particleIndex;
    public Effect[] effects;
}
