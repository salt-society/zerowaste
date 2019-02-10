using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class Enemy : Character {

    [Header("Enemy Statistics")]
    public int maxPollutionLevel;
    public int maxAtk;
    public int maxDef;
    public int baseScrapReward;
    public int baseEXPReward;

    [HideInInspector] public Ability[] instanceAbilities;

    [HideInInspector] public int currentPollutionLevel;
    [HideInInspector] public int currentAtk;
    [HideInInspector] public int currentDef;
    [HideInInspector] public int currentScrapReward;
    [HideInInspector] public int currentEXPReward;

    // Initialize currentStats to be equal to maxStats
    public void OnInitialize()
    {
        currentPollutionLevel = maxPollutionLevel;
        currentAtk = maxAtk;
        currentDef = maxDef;
        currentSpd = baseSpd;
        currentScrapReward = baseScrapReward;
        currentEXPReward = baseEXPReward;

        InitializeAbilities();
    }

    // Initialize each ability so that changes made will not be saved
    private void InitializeAbilities()
    {
        for(int CTR = 0; CTR < abilities.Length; CTR++)
            instanceAbilities[CTR] = Instantiate(abilities[CTR]);
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
    public void IsAttacked(int statModifier, Player attacker)
    {
        int estimatedStat = 0;
        int damage = 0;
        damage = (attacker.currentAtk + statModifier) - currentDef;
        estimatedStat = CheckMin(currentPollutionLevel - damage);
        currentPollutionLevel = estimatedStat;
        Debug.Log("1: " + currentPollutionLevel);
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
        switch (effect.effectTarget)
        {
            case "PL":
                currentPollutionLevel = CheckMax(currentPollutionLevel + effect.effectStrength);
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
        }
    }

    // Call if enemy has been debuffed()
    public void IsDebuffed(Effect effect)
    {
        switch (effect.effectTarget)
        {
            case "PL":
                currentPollutionLevel = CheckMin(currentPollutionLevel - effect.effectStrength);
                effects.Add(effect);
                break;

            case "Atk":
                currentAtk = CheckMin(currentAtk - effect.effectStrength);
                effects.Add(effect);
                break;

            case "Def":
                currentDef  = CheckMin(currentDef - effect.effectStrength);
                effects.Add(effect);
                break;

            case "Spd":
                currentSpd = CheckMin(currentSpd - effect.effectStrength);
                effects.Add(effect);
                break;

            case "Scrap Reward":
                currentScrapReward += effect.effectStrength;
                effects.Add(effect);
                break;

            case "Exp Reward":
                currentEXPReward += effect.effectStrength;
                effects.Add(effect);
                break;
        }
    }
}
