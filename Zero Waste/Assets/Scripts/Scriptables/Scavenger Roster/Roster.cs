using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scavenger Roster", menuName = "Scavenger Roster")]
public class Roster : ScriptableObject
{
    public List<Player> scavengers;
    public int scavengerCount;
}
