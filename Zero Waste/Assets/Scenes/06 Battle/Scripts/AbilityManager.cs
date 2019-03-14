﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public int abilityIndex;

    [Space]
    [Header("Managers")]
    public AttackController attackController;
    public TargetManager targetManager;
    public StatusManager statusManager;

    private PlayerAbility ability;

    private Player scavenger;
    private GameObject scavengerPrefab;
    private int scavengerPosition;

    public void ChooseAbility()
    {
        attackController.SetCurrentAbility(ability);

        if (!attackController.IsCurrentAttackNull())
            targetManager.SelectTarget(ability, this);
        else
            targetManager.CancelTargetSelection();
    }

    public void SetCurrentCharacterPrefab(GameObject characterPrefab)
    {
        this.scavengerPrefab = characterPrefab;
        scavenger = characterPrefab.GetComponent<CharacterMonitor>().Scavenger;
    }

    public void SetUpAbility(int index)
    {
        ability = scavenger.abilities[index] as PlayerAbility;
    }

    public void SetScavengerPosition(int position)
    {
        this.scavengerPosition = position;
    }

    public void IsAbilityAvailable()
    {
        if (scavenger.currentAnt >= ability.antRequirement)
            attackController.EnableAttackButton(1, abilityIndex);     
    }

    public IEnumerator ExecuteAbility(GameObject selectedTarget, string targetType)
    {
        // Decrement scavenger's antidote
        scavenger.currentAnt = scavenger.CheckMin(scavenger.currentAnt - ability.antRequirement);
        // statusManager.DecrementAntidote(ability.antRequirement, scavengerPosition);

        // Apply effects if there's any on selected mutant
        StartCoroutine(ApplyEffects(selectedTarget, targetType));

        yield return null;
    }

    IEnumerator ApplyEffects(GameObject characterPrefab, string targetType)
    {
        // Get target character's position on screen
        int targetPosition = characterPrefab.GetComponent<CharacterMonitor>().Position;

        // Player to Player
        if (targetType.Equals("Scavenger")) 
        {
            Player target = characterPrefab.GetComponent<CharacterMonitor>().Scavenger;
            foreach (Effect effect in ability.effects)
            {
                if (effect.effectType.Equals("Direct")) 
                {
                    target.IsHealed(effect.effectTarget, effect.effectStrength);

                    if (effect.effectTarget.Equals("HP"))
                    {
                        
                    }

                    if (effect.effectTarget.Equals("Ant"))
                    {

                    }
                }
                else if (effect.effectType.Equals("Status"))
                {
                    target.IsBuffed(Instantiate(effect));

                    if (effect.effectTarget.Equals("HP"))
                    {
                        
                    }
                    else if (effect.effectTarget.Equals("Ant"))
                    {

                    }
                    else
                    {

                    }
                }

            }
        } 
        // Player to Enemy
        else {
            Enemy mutant = characterPrefab.GetComponent<CharacterMonitor>().Mutant;
            foreach (Effect effect in ability.effects)
            {
                if (effect.effectType.Equals("Direct"))
                {
                    double previousHealth = mutant.currentPollutionLevel;

                    mutant.IsAttacked(effect.effectStrength, scavenger);
                    double currentHealth = mutant.currentPollutionLevel;
                    double damage = previousHealth - currentHealth;

                    // statusManager.DecrementPollutionBar(currentHealth, targetPosition);
                    statusManager.ShowDamage(damage.ToString(), characterPrefab, true);

                    characterPrefab.GetComponent<Animator>().SetBool("Damaged 01", true);
                    yield return new WaitForSeconds(2f);

                    statusManager.ShowDamage(damage.ToString(), characterPrefab, false);
                    characterPrefab.GetComponent<Animator>().SetBool("Damaged 01", false);
                }
                else if (effect.effectType.Equals("Status"))
                {
                    double previousHealth = mutant.currentPollutionLevel;
                    mutant.IsDebuffed(Instantiate(effect));

                    if (effect.effectTarget.Equals("PL"))
                    {
                        double currentHealth = mutant.currentPollutionLevel;
                        double damage = previousHealth - currentHealth;

                        // statusManager.DecrementPollutionBar(damage, targetPosition);
                        statusManager.ShowDamage(damage.ToString(), characterPrefab, true);
                    }
                    else
                    {

                    }
                }

            }
        }
    }
}
