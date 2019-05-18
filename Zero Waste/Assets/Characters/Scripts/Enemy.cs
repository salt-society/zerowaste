using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Character/Enemy")]
public class Enemy : Character {

    [Header("Enemy Statistics")]
    [Range(100, 300)] public int basePollutionLevel;
    [Range(10, 20)] public int baseAtk;
    [Range(10, 20)] public int baseDef;
    [Range(5, 50)] public int baseScrapReward;
    [Range(5, 50)] public int baseEXPReward;
    public string baseState;
    public string roleWeakness;

    [Header("Modifiers")] 
    [Range(10, 20)] public int plModifier;
    [Range(5, 10)] public int atkModifier;
    [Range(5, 10)] public int defModifier;
    private int expModifier;
    private int scrapModifier;

    public int baseLevel;
    public int maxLevel;
    public int areaEncountered;
    public string description;
    [HideInInspector] public int mutantLevel;

    [HideInInspector] public List<Ability> instanceAbilities;

    [HideInInspector] public int currentPollutionLevel;
    [HideInInspector] public int currentAtk;
    [HideInInspector] public int currentDef;
    [HideInInspector] public int currentScrapReward;
    [HideInInspector] public int currentEXPReward;
    [HideInInspector] public string currentState;

    private bool hasChangedState;

    // Used because the isAttacked of enemy is an overrideable function
    private int damage;

    // Initialize currentStats to be equal to maxStats
    public virtual void OnInitialize()
    {
        mutantLevel = Random.Range(baseLevel, maxLevel + 1);

        expModifier = 10;
        scrapModifier = 10;

        currentPollutionLevel = basePollutionLevel + (plModifier * mutantLevel);
        currentAtk = baseAtk + (atkModifier * mutantLevel);
        currentDef = baseDef + (defModifier * mutantLevel);
        currentSpd = baseSpd;
        currentScrapReward = baseScrapReward + (scrapModifier * mutantLevel);
        currentEXPReward = baseEXPReward + (expModifier * mutantLevel);

        currentState = baseState;
        hasChangedState = false;

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
        if (targetStat > basePollutionLevel)
            return basePollutionLevel;

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
                damage = CheckMin((projectedStrength + statModifier) - (int)(currentDef / 1.25));

            else
                damage = projectedStrength + statModifier;

            estimatedStat = CheckMin(currentPollutionLevel - damage);
            currentPollutionLevel = estimatedStat;
        }

        else
        {
            if (currentDef > 0)
                damage = CheckMin(statModifier - (int)(currentDef / 1.25));

            else
                damage = statModifier;

            estimatedStat = CheckMin(currentPollutionLevel - damage);
            currentPollutionLevel = estimatedStat;
        }

        if (currentPollutionLevel > (int)(basePollutionLevel * 0.5f) && hasChangedState == false)
        {
            if (baseState == "Offensive")
                currentState = "Defensive";

            else
                currentState = "Offensive";
        }
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
