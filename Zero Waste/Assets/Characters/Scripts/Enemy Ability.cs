using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Ability", menuName = "Abilities/Enemy Ability")]
public class EnemyAbility : Ability {

    public int cooldown;
    public int turnTillActive;

}
