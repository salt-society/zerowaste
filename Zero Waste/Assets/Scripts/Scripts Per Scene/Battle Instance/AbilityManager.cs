using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public PlayerAbility ability;
    public int abilityIndex;

    [Space]
    public AttackController attackController;

    public void SetUpAbility(PlayerAbility ability)
    {
        this.ability = ability;
    }

    public void IsAbilityAvailable(Player currentCharacter)
    {
        if (currentCharacter.currentAnt >= ability.antRequirement)
        {
            attackController.EnableAttackButton(1, abilityIndex);
            Debug.Log(currentCharacter.currentAnt + " : " + ability.antRequirement);
        }
            
    }


}
