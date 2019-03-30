using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Ability", menuName = "Abilities/Enemy Ability")]
public class EnemyAbility : Ability {

    [Space]
    public int cooldown;
    public int turnTillActive;
    [Range(0.0f, 100.0f)] public float chanceToProc;
    public string abilityState;
}
