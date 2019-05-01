using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Character/Player")]
[System.Serializable]
public class Player : Character {

    [Header("Statistics")]
    public Role characterClass;
    [Range(1, 30)] public int currentLevel;
    [Range(50, 70)] public int baseHP;
    [Range(100,100)] public int baseAnt;
    [Range(10, 20)] public int baseAtk;
    [Range(10, 20)] public int baseDef;
    [Range(10, 30)] public int antGen;
    [Range(1, 3)] public int threatLevel;

    [Header("Level Modifiers")]
    [Range(5, 10)] public int hpModifier;
    [Range(5, 10)] public int atkModifier;
    [Range(5, 10)] public int defModifier;
    
    [HideInInspector] public int currentHP;
    [HideInInspector] public int currentAnt;
    [HideInInspector] public int currentAtk;
    [HideInInspector] public int currentDef;
    [HideInInspector] public int currentAntGen;
    [HideInInspector] public int currentThreatLevel;
    public int currentExp;
    public int currentLevelCap;

    // Apply level modifiers to character
    public void OnInitialize()
    {
        baseAnt = 100;

        currentHP = baseHP + (currentLevel * hpModifier);
        currentAnt = baseAnt / 2;
        currentAtk = baseAtk + (currentLevel * atkModifier);
        currentDef = baseDef + (currentLevel * defModifier);
        currentSpd = baseSpd;
        currentAntGen = antGen;
        currentThreatLevel = threatLevel;
    }

    // Check if the HP or ANT stat will exceed the characters current MAX HP / ANT
    public int CheckMax(int endStat, string target)
    {
        switch (target)
        {
            case "HP":
                if (endStat > (baseHP + (int)(currentLevel * hpModifier)))
                    return baseHP + (int)(currentLevel * hpModifier);
                else
                    return endStat;

            case "ANT":
                if (endStat > baseAnt)
                    return baseAnt;
                else
                    return endStat;
        }

        return 0;
    }

    // Call if player has been attacked
    public int IsAttacked(string targetStat, int statModifier, Enemy attacker)
    {
        int estimatedStat = 0;

        switch (targetStat)
        {
            case "HP":
                int damage = 0;

                if(attacker.currentAtk > 0)
                {
                    int projectedStrength = Random.Range((int)(attacker.currentAtk * 0.5), (int)(attacker.currentAtk * 1.5) + 1);

                    if (currentDef > 0)
                        damage = CheckMin((projectedStrength + statModifier) - (int)(currentDef / 1.25));

                    else
                        damage = projectedStrength + statModifier;
                }

                else
                {
                    if (currentDef > 0)
                        damage = CheckMin(statModifier - (int)(currentDef / 1.25));

                    else
                        damage = statModifier;
                }

                estimatedStat = CheckMin(currentHP - damage);
                currentHP = estimatedStat;
                return damage;

            case "ANT":
                damage = statModifier;
                estimatedStat = CheckMin(currentAnt - statModifier);
                currentAnt = estimatedStat;
                return damage;
        }

        return 0;
    }

    // Call if player has been healed
    public int IsHealed(string targetStat, int statModifier)
    {
        int heal = statModifier;
        int estimatedStat = 0;

        switch (targetStat)
        {
            case "HP":
                estimatedStat = CheckMax(currentHP + statModifier, targetStat);
                currentHP = estimatedStat;
                return heal;

            case "ANT":
                estimatedStat = CheckMax(currentAnt + statModifier, targetStat);
                currentAnt = estimatedStat;
                return heal;
        }

        return 0;
    }

    // Call if player has been buffed
    public void IsBuffed(Effect effect)
    {
        switch(effect.target)
        {
            case "HP":
                effects.Add(effect);
                break;

            case "ANT":
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

            case "ANTGEN":
                currentAntGen += effect.strength;
                effects.Add(effect);
                break;

            case "TL":
                currentThreatLevel += effect.strength;
                effects.Add(effect);
                break;
        }
    }

    // Call if player has been debuffed()
    public void IsDebuffed(Effect effect)
    {
        int estimatedStat = 0;

        switch (effect.target)
        {
            case "HP":
                effects.Add(effect);
                break;

            case "ANT":
                effects.Add(effect);
                break;

            case "ATK":
                estimatedStat = currentAtk - effect.strength;
                currentAtk = estimatedStat;
                effects.Add(effect);
                break;

            case "DEF":
                estimatedStat = currentDef - effect.strength;
                currentDef = estimatedStat;
                effects.Add(effect);
                break;

            case "SPD":
                estimatedStat = currentSpd - effect.strength;
                currentSpd = estimatedStat;
                effects.Add(effect);
                break;

            case "ANTGEN":
                estimatedStat = currentAntGen - effect.strength;
                currentAntGen = estimatedStat;
                effects.Add(effect);
                break;

            case "TL":
                estimatedStat = currentThreatLevel - effect.strength;
                currentThreatLevel = estimatedStat;
                effects.Add(effect);
                break;
        }
    }

    // Clear all debuffs from player
    public void IsCleared()
    {
        if (effects.Count > 0)
        {
            List<Effect> removedDebuffs = new List<Effect>();
            
            foreach (Effect effect in effects)
                if (effect.state == "Debuff")
                    removedDebuffs.Add(effect);

            if (removedDebuffs.Count > 0)
            {
                foreach (Effect effect in removedDebuffs)
                { 
                    switch (effect.target)
                    {
                        case "ATK":
                            currentAtk += effect.strength;
                            break;

                        case "DEF":
                            currentDef += effect.strength;
                            break;

                        case "SPD":
                            currentSpd += effect.strength;
                            break;

                        case "ANTGEN":
                            currentAntGen += effect.strength;
                            break;

                        case "TL":
                            currentThreatLevel += effect.strength;
                            break;
                    }

                    effects.Remove(effect);
                }
            }
        }
    }
}
