using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Character/Enemy")]
public class Enemy : Character {

    [Header("Enemy Statistics")]
    [Range(1,30)]public int mutantLevel;
    [Range(100, 1000)] public int maxPollutionLevel;
    [Range(10, 100)] public int maxAtk;
    [Range(10, 100)]public int maxDef;
    [Range(10, 500)]public int baseScrapReward;
    [Range(10, 500)]public int baseEXPReward;
    public string baseState;
    public string roleWeakness;

    [Header("Modifiers")]
    [Range(50, 100)]public int plModifier;
    [Range(1, 10)]public int atkModifier;
    [Range(1, 10)]public int defModifier;
    [Range(1, 5)]public int expModifier;
    [Range(1, 5)]public int scrapModifier;

    [HideInInspector] public List<Ability> instanceAbilities;

    [HideInInspector] public int currentPollutionLevel;
    [HideInInspector] public int currentAtk;
    [HideInInspector] public int currentDef;
    [HideInInspector] public int currentScrapReward;
    [HideInInspector] public int currentEXPReward;
    [HideInInspector] public string currentState;

    private int levelModifier;

    // Used because the isAttacked of enemy is an overrideable function
    private int damage;

    // Initialize currentStats to be equal to maxStats
    public virtual void OnInitialize()
    {
        levelModifier = Random.Range(0, 4);

        mutantLevel += levelModifier;

        currentPollutionLevel += (plModifier * mutantLevel);
        currentAtk += (atkModifier * mutantLevel);
        currentDef += (defModifier * mutantLevel);
        currentScrapReward += (scrapModifier * mutantLevel);
        currentEXPReward += (expModifier * mutantLevel);

        currentState = baseState;

        InitializeAbilities();
    }

    // Initialize each ability so that changes made will not be saved
    private void InitializeAbilities()
    {
        foreach (Ability ability in abilities)
        {
            instanceAbilities.Add(Instantiate(ability));
        }
    }

    // Check max so values do not go beyond max
    public int CheckMax(int targetStat)
    {
        if (targetStat > maxPollutionLevel)
            return maxPollutionLevel;

        else
            return targetStat;
    }

    // Call if enemy has been attacked
    public virtual void IsAttacked(int statModifier, Player attacker)
    {
        int estimatedStat = 0;
        damage = 0;

        if (attacker.currentAtk >= 0)
        {
            int projectedStrength = Random.Range(attacker.currentAtk, (int)(attacker.currentAtk * 1.25) + 1);

            if (attacker.characterClass.roleName == roleWeakness)
                projectedStrength += (int)(projectedStrength * 0.5);

            if (currentDef > 0)
                damage = CheckMin((projectedStrength + statModifier) - currentDef);

            else
                damage = projectedStrength + statModifier;

            estimatedStat = CheckMin(currentPollutionLevel - damage);
            currentPollutionLevel = estimatedStat;
        }

        else
        {
            if (currentDef > 0)
                damage = CheckMin(statModifier - currentDef);

            else
                damage = statModifier;

            estimatedStat = CheckMin(currentPollutionLevel - damage);
            currentPollutionLevel = estimatedStat;
        }

        if (currentPollutionLevel > (int)(maxPollutionLevel * 0.5f))
        {
            if (baseState == "Offensive")
                currentState = "Defensive";

            else
                currentState = "Offensive";
        }

        Debug.Log("Current Atk: " + attacker.currentAtk + " Stat Modifier: " + statModifier 
            + " Current Def: " + currentDef + " Damage: " + damage + " | " + estimatedStat);
    }

    // Call if enemy has been healed
    public void IsHealed(int statModifier)
    {
        int estimatedStat = 0;
        estimatedStat = CheckMax(currentPollutionLevel + statModifier);
        currentPollutionLevel = estimatedStat;
    }

    // Call if enemy has been buffed
    public void IsBuffed(Effect effect)
    {
        switch (effect.target)
        {
            case "PL":
                effects.Add(effect);
                break;

            case "ATK":
                currentAtk += effect.strength;
                effects.Add(effect);
                break;

            case "DEF":
                currentDef += effect.strength;
                effects.Add(effect);
                break;

            case "SPD":
                currentSpd += effect.strength;
                effects.Add(effect);
                break;
        }
    }

    // Call if enemy has been debuffed()
    public void IsDebuffed(Effect effect)
    {
        switch (effect.target)
        {
            case "PL":
                effects.Add(effect);
                break;

            case "ATK":
                currentAtk = currentAtk - effect.strength;
                effects.Add(effect);
                break;

            case "DEF":
                currentDef  = currentDef - effect.strength;
                effects.Add(effect);
                break;

            case "SPD":
                currentSpd = currentSpd - effect.strength;
                effects.Add(effect);
                break;

            case "SCRAP":
                currentScrapReward += effect.strength;
                effects.Add(effect);
                break;

            case "EXP":
                currentEXPReward += effect.strength;
                effects.Add(effect);
                break;
        }
    }

    // Get the damage value
    public int GetDamage()
    {
        return damage;
    }
}
