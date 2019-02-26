using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Character/Player")]
[System.Serializable]
public class Player : Character {

    [Header("Statistics")]
    public Role characterClass;
    [Range(1, 30)] public int currentLevel;
    [Range(5, 10)] public int baseHP;
    public int baseAnt;
    [Range(1, 5)] public int baseAtk;
    [Range(1, 5)] public int baseDef;
    [Range(1, 3)] public int antGen;
    [Range(1, 5)] public int threatLevel;

    [Header("Level Modifiers")]
    [Range(1f, 2f)] public double hpModifier;
    [Range(0.5f, 1f)] public double atkModifier;
    [Range(0.5f, 1f)] public double defModifier;
    [Range(1f, 2f)] public double spdModifier;

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
        currentHP = baseHP + (int)((currentLevel - 1) * hpModifier);
        currentAnt = (int)(baseAnt / 2);
        currentAtk = baseAtk + (int)((currentLevel - 1) * atkModifier);
        currentDef = baseDef + (int)((currentLevel - 1) * defModifier);
        currentSpd = baseSpd + (int)((currentLevel - 1) * spdModifier);
        currentAntGen = antGen;
        currentThreatLevel = threatLevel;
    }

    // Check if the HP or ANT stat will exceed the characters current MAX HP / ANT
    public int CheckMax(int endStat, string target)
    {
        switch (target)
        {
            case "HP":
                if (endStat > (baseHP + (int)((currentLevel - 1) * hpModifier)))
                    return baseHP + (int)((currentLevel - 1) * hpModifier);
                else
                    return endStat;

            case "Ant":
                if (endStat > baseAnt)
                    return baseAnt;
                else
                    return endStat;
        }

        return 0;
    }

    // Call if player has been attacked
    public void IsAttacked(string targetStat, int statModifier, Enemy attacker)
    {
        int estimatedStat = 0;

        switch (targetStat)
        {
            case "HP":
                int damage = 0;
                damage = CheckMin((attacker.currentAtk + statModifier) - currentDef);
                estimatedStat = CheckMin(currentHP - damage);
                currentHP = estimatedStat;
                break;

            case "Ant":
                estimatedStat = CheckMin(currentAnt - statModifier);
                currentAnt = estimatedStat;
                break;
        }
    }

    // Call if player has been healed
    public void IsHealed(string targetStat, int statModifier)
    {
        int estimatedStat = 0;

        switch (targetStat)
        {
            case "HP":
                estimatedStat = CheckMax(currentHP + statModifier, "HP");
                currentHP = estimatedStat;
                break;

            case "Ant":
                estimatedStat = CheckMax(currentAnt + statModifier, "Ant");
                currentAnt = estimatedStat;
                break;
        }
    }

    // Call if player has been buffed
    public void IsBuffed(Effect effect)
    {
        int estimatedStat = 0;

        switch(effect.effectTarget)
        {
            case "HP":
                estimatedStat = CheckMax(currentHP + effect.effectStrength, "HP");
                currentHP = estimatedStat;
                effects.Add(effect);
                break;

            case "Ant":
                estimatedStat = CheckMax(currentAnt + effect.effectStrength, "Ant");
                currentAnt = estimatedStat;
                effects.Add(effect);
                break;

            case "Atk":
                currentAtk += effect.effectStrength;
                effects.Add(effect);
                break;

            case "Def":
                currentDef += effect.effectStrength;
                effects.Add(effect);
                break;

            case "Spd":
                currentSpd += effect.effectStrength;
                effects.Add(effect);
                break;

            case "Ant Gen":
                currentAntGen += effect.effectStrength;
                effects.Add(effect);
                break;

            case "Threat Level":
                currentThreatLevel += effect.effectStrength;
                effects.Add(effect);
                break;
        }
    }

    // Call if player has been debuffed()
    public void IsDebuffed(Effect effect)
    {
        int estimatedStat = 0;

        switch (effect.effectTarget)
        {
            case "HP":
                estimatedStat = CheckMin(currentHP - effect.effectStrength);
                currentHP = estimatedStat;
                effects.Add(effect);
                break;

            case "Ant":
                estimatedStat = CheckMin(currentAnt - effect.effectStrength);
                currentAnt = estimatedStat;
                effects.Add(effect);
                break;

            case "Atk":
                estimatedStat = CheckMin(currentAtk - effect.effectStrength);
                currentAtk = estimatedStat;
                effects.Add(effect);
                break;

            case "Def":
                estimatedStat = CheckMin(currentDef - effect.effectStrength);
                currentDef = estimatedStat;
                effects.Add(effect);
                break;

            case "Spd":
                estimatedStat = CheckMin(currentSpd - effect.effectStrength);
                currentSpd = estimatedStat;
                effects.Add(effect);
                break;

            case "Ant Gen":
                estimatedStat = CheckMin(currentAntGen - effect.effectStrength);
                currentAntGen = estimatedStat;
                effects.Add(effect);
                break;

            case "Threat Level":
                estimatedStat = CheckMin(currentThreatLevel - effect.effectStrength);
                currentThreatLevel = estimatedStat;
                effects.Add(effect);
                break;
        }
    }
}
