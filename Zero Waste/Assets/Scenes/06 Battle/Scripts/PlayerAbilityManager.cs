using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityManager : MonoBehaviour
{
    public int abilityIndex;

    #region Battle Managers
    private BattleController battleController;
    private AttackController attackController;
    private TargetManager targetManager;
    private StatusManager statusManager;
    private CameraManager cameraManager;
    private AnimationManager animationManager;
    private ParticleManager particleManager;
    private CharacterManager characterManager;
    #endregion

    #region Properties
    private Player scavenger;

    public Player Scavenger
    {
        get { return scavenger; }
        set { scavenger = value; }
    }

    private PlayerAbility ability;

    public PlayerAbility Ability
    {
        get { return ability; }
        set { ability = value; }
    }

    private int damageApplied;

    public int ChangeApplied
    {
        get { return damageApplied; }
        set { damageApplied = value; }
    }

    private GameObject scavengerPrefab;

    public GameObject ScavengerPrefab
    {
        get { return scavengerPrefab; }
        set { scavengerPrefab = value; }
    }

    private int position;

    public int Position
    {
        get { return position; }
        set { position = value; }
    }
    #endregion

    void Start()
    {
        battleController = FindObjectOfType<BattleController>();
        attackController = FindObjectOfType<AttackController>();
        targetManager = FindObjectOfType<TargetManager>();
        statusManager = FindObjectOfType<StatusManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        animationManager = FindObjectOfType<AnimationManager>();
        particleManager = FindObjectOfType<ParticleManager>();
        characterManager = FindObjectOfType<CharacterManager>();
    }

    // <summary>
    // Each ability has an ability manager, when player selects
    // an ability, other abilities are not aware of it
    // The attack controller binds all the abilities, so the system
    // knows when there is already a selected ability
    // </summary>
    public void ChooseAbility()
    {

        Debug.Log("Ability Chosen: " + ability.name);
        // Let attack controller know that player has chosen an ability
        // Note that you can't choose an ability that doesn't meet
        // ant requirement, in example Character Skill and Ultimate Move
        attackController.SetCurrentAbility(ability);

        // Checks if there's an ability to execute and cancels if none
        if (!attackController.IsCurrentAttackNull())
        {
            // If ability is switched, "Cancel Attack" message shouldn't be shown
            if (attackController.SwitchedAbility)
            {
                targetManager.CancelTargetSelection(attackController.SwitchedAbility);
            }
                
            targetManager.SelectTarget(ability, this);
        }
        else
        {
            targetManager.CancelTargetSelection(attackController.SwitchedAbility);
        }
            
    }

    // <summary>
    // Set up ability by getting ability in scavenger data through index
    // </summary>
    public void SetupAbility(int index)
    {
        ability = scavenger.abilities[index] as PlayerAbility;
    }

    // <summary>
    // Checks if ability can be used base on Ant, then enables button for that ability
    // </summary>
    public void IsAbilityAvailable()
    {
        if (scavenger.currentAnt >= ability.antRequirement)
            attackController.EnableAttackButton(1, abilityIndex);
    }

    // <summary>
    // </summary>
    public IEnumerator ExecuteSingleRangeAbility(GameObject selectedTarget, string targetType)
    {
        // Decrement antidote used by ability and update antidote bar
        scavengerPrefab.GetComponent<CharacterMonitor>().DecrementAntidote(ability.antRequirement);
        
        yield return null;

        // Apply effects if there's any on selected mutant
        ApplySingleRangeAbilityEffects(selectedTarget, targetType);
    }

    // <summary>
    // </summary>
    public IEnumerator ExecuteAOERangeAbility(string targetType)
    {
        // Decrement antidote used by ability and update antidote bar
        scavengerPrefab.GetComponent<CharacterMonitor>().DecrementAntidote(ability.antRequirement);

        yield return null;

        // Apply effects if there's any on selected mutant
        ApplyAOERangeAbilityEffects(targetType);
    }

    // <summary>
    // </summary>
    void ApplySingleRangeAbilityEffects(GameObject targetPrefab, string targetType)
    {
        // Scavenger to Scavenger
        if (targetType.Equals("Scavenger")) 
        {
            StartCoroutine(ScavengerToScavenger(targetPrefab));
        } 
        // Scavenger to Mutant
        else if(targetType.Equals("Mutant"))
        {
            StartCoroutine(ScavengerToMutant(targetPrefab));
        }
    }

    // <summary>
    // </summary>
    void ApplyAOERangeAbilityEffects(string targetType)
    {
        // Scavenger to Scavenger
        if (targetType.Equals("Scavenger"))
        {
            StartCoroutine(ScavengerToAllScavenger());
        }
        // Scavenger to Mutant
        else if (targetType.Equals("Mutant"))
        {
            StartCoroutine(ScavengerToAllMutant());
        }
    }

    // <summary>
    // </summary>
    IEnumerator ScavengerToScavenger(GameObject targetPrefab)
    {
        // Data of target is needed to update changes
        Player target = targetPrefab.GetComponent<CharacterMonitor>().Scavenger;

        // Bring back camera to its default position first
        // This avoids animation being out of place, like the position
        // of damage points and debuff/buff arrows
        cameraManager.FocusOnScavengers(false);
        cameraManager.FocusOnMutants(false);
        yield return new WaitForSeconds(0.3f);

        // 
        int regainedValue = 0;
        string targetStat = string.Empty;

        // This part is where we will apply changes on data
        // Just loop through all effects, differentiate if its direct or not,
        // then apply corresponding changes
        foreach (Effect effect in ability.effects)
        {
            if (effect.effectType.Equals("Direct")) 
            {
                regainedValue = targetPrefab.GetComponent<CharacterMonitor>().
                    ScavengerHealed(effect.effectTarget, effect.effectStrength);
                targetStat = effect.effectTarget;
            }
            else
            {     
                targetPrefab.GetComponent<CharacterMonitor>().ScavengerBuffed(Instantiate(effect));
            }
        }

        if (regainedValue > 0)
        {
            if(targetStat.Equals("HP"))
            {
                StartCoroutine(animationManager.HealScavenger(targetPrefab, regainedValue, 
                    abilityIndex, ability.abilityRange));
            }
            else
            {
                StartCoroutine(animationManager.ChargeAntidote(targetPrefab, 
                    abilityIndex, ability.abilityRange));
            }

            yield return new WaitForSeconds(1.5f);
        }

        foreach (Effect effect in ability.effects)
        {
            if (effect.effectType.Equals("Status"))
            {
                // Status effect that alters variables or stats of player
                if (effect.effectApplication.Equals("Variable"))
                {
                    StartCoroutine(statusManager.ShowBuff(targetPrefab, 1));
                }
                // Status effect that subjects character into a condition
                else
                {
                    // Each condition has different kind of effects
                    // Temporary use burst particles
                    StartCoroutine(particleManager.CircleBurst(targetPrefab.transform.position));
                }

                // Add status change to panel and list
                statusManager.AddEffectToStatusPanel("Scavenger", 
                    targetPrefab.GetComponent<CharacterMonitor>().Position, effect);
                statusManager.AddEffectsToStatusList("Scavenger",
                    targetPrefab.GetComponent<CharacterMonitor>().Position, effect, ability.abilityIcon);
                yield return new WaitForSeconds(0.75f);
            }
        }

        // If there isn't damage, only status effects, make scavenger go back
        // from fight stance to its default idle animation
        if (regainedValue == 0)
        {
            scavengerPrefab.GetComponent<CharacterMonitor>().Idle();
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator ScavengerToAllScavenger()
    {
        GameObject[] targetPrefabs = characterManager.GetAllCharacterPrefabs(1);

        // Bring back camera to its default position first
        // This avoids animation being out of place, like the position
        // of damage points and debuff/buff arrows
        cameraManager.FocusOnScavengers(false);
        cameraManager.FocusOnMutants(false);
        yield return new WaitForSeconds(0.3f);

        int totalRegainedValue = 0;
        string targetStat = string.Empty;

        foreach (GameObject targetPrefab in targetPrefabs)
        {
            // This part is where we will apply changes on data
            // Just loop through all effects, differentiate if its direct or not,
            // then apply corresponding changes
            foreach (Effect effect in ability.effects)
            {
                if (effect.effectType.Equals("Direct"))
                {
                    totalRegainedValue += targetPrefab.GetComponent<CharacterMonitor>().
                        ScavengerHealed(effect.effectTarget, effect.effectStrength);
                    targetStat = effect.effectTarget;
                }
                else
                {
                    targetPrefab.GetComponent<CharacterMonitor>().ScavengerBuffed(Instantiate(effect));
                }
            }
        }

        if (totalRegainedValue > 0)
        {
            if (targetStat.Equals("HP"))
            {
                StartCoroutine(animationManager.HealAllScavengers(targetPrefabs, totalRegainedValue,
                    abilityIndex, ability.abilityRange));
            }
            else
            {
                StartCoroutine(animationManager.ChargeAllAntidote(targetPrefabs,
                    abilityIndex, ability.abilityRange));
            }

            yield return new WaitForSeconds(1.5f);
        }

        foreach (GameObject targetPrefab in targetPrefabs)
        {
            foreach (Effect effect in ability.effects)
            {
                if (effect.effectType.Equals("Status"))
                {
                    // Status effect that alters variables or stats of player
                    if (effect.effectApplication.Equals("Variable"))
                    {
                        StartCoroutine(statusManager.ShowBuff(targetPrefab, 1));
                    }
                    // Status effect that subjects character into a condition
                    else
                    {
                        // Each condition has different kind of effects
                        // Temporary use burst particles
                        StartCoroutine(particleManager.CircleBurst(targetPrefab.transform.position));
                    }

                    // Add status change to panel and list
                    statusManager.AddEffectToStatusPanel("Scavenger",
                        targetPrefab.GetComponent<CharacterMonitor>().Position, effect);
                    statusManager.AddEffectsToStatusList("Scavenger",
                        targetPrefab.GetComponent<CharacterMonitor>().Position, effect, ability.abilityIcon);
                    yield return new WaitForSeconds(0.75f);
                }
            }
        }

        // If there isn't damage, only status effects, make scavenger go back
        // from fight stance to its default idle animation
        if (totalRegainedValue == 0)
        {
            scavengerPrefab.GetComponent<CharacterMonitor>().Idle();
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(EndOfTurn());
    }

    // <summary>
    // </summary>
    IEnumerator ScavengerToMutant(GameObject targetPrefab)
    {
        // Data of target is needed to update changes
        Enemy mutant = targetPrefab.GetComponent<CharacterMonitor>().Mutant;

        // Bring back camera to its default position first
        // This avoids animation being out of place, like the position
        // of damage points and debuff/buff arrows
        cameraManager.FocusOnScavengers(false);
        cameraManager.FocusOnMutants(false);
        yield return new WaitForSeconds(0.3f);

        // Direct effect animations only play once that's why we need
        // to know the total damage taken by mutant to show later
        int totalDamage = 0;

        // This part is where we will apply changes on data
        // Just loop through all effects, differentiate if its direct or not,
        // then apply corresponding changes
        foreach (Effect effect in ability.effects)
        {
            // There are two kinds of Effect Type, Direct and Status
            // Usually, direct effects for mutants are damage
            if (effect.effectType.Equals("Direct"))
            {
                // Calculate damage of effect and add to total damage
                // Definitely there are no double direct effect
                // But just to be safe, this condition gets total to show later
                totalDamage += targetPrefab.GetComponent<CharacterMonitor>().
                    MutantDamaged(effect.effectStrength, scavenger);
                damageApplied = totalDamage;
            }
            // If its not a direct effect, its a status change
            else
            {
                // Status effects for mutants are debuff
                targetPrefab.GetComponent<CharacterMonitor>().MutantDebuffed(Instantiate(effect));
            }
        }

        // After data's handled, its time to play animations
        // Avoid showing damage points if there's none
        if (totalDamage > 0)
        {
            StartCoroutine(animationManager.AttackMutant(scavengerPrefab, targetPrefab, 
                abilityIndex, totalDamage, ability.abilityRange));
            yield return new WaitForSeconds(1.5f);
        }
            
        // Unlike direct effects which animations will only play once,
        // each animation of status effect will be displayed
        foreach (Effect effect in ability.effects)
        {
            // Only do somehing if effect type is status
            if (effect.effectType.Equals("Status"))
            {
                // There are two ways to apply a status effect, one is through
                // variable and the other is condition
                // Variable is where a change directly in Stat happens like Atk Down
                if (effect.effectApplication.Equals("Variable"))
                {
                    StartCoroutine(statusManager.ShowBuff(targetPrefab, 0));
                }
                // Condition is where a character is subject to, ie. poisoned
                else
                {
                    StartCoroutine(particleManager.CircleBurst(targetPrefab.transform.position));
                }

                yield return new WaitForSeconds(0.75f);
            }
        }
        
        // If there isn't damage, only status effects, make scavenger go back
        // from fight stance to its default idle animation
        if (totalDamage == 0)
        {
            scavengerPrefab.GetComponent<CharacterMonitor>().Idle();
            yield return new WaitForSeconds(0.5f);
        }

        // Make way for dying animation if character is already dead
        if (!targetPrefab.GetComponent<CharacterMonitor>().IsAlive)
        {
            yield return new WaitForSeconds(1f);
        }

        // StartCoroutine(EndOfTurn());
    }

    IEnumerator ScavengerToAllMutant()
    {
        GameObject[] targetPrefabs = characterManager.GetAllCharacterPrefabs(0);

        // Bring back camera to its default position first
        // This avoids animation being out of place, like the position
        // of damage points and debuff/buff arrows
        cameraManager.FocusOnScavengers(false);
        cameraManager.FocusOnMutants(false);
        yield return new WaitForSeconds(0.3f);

        // Direct effect animations only play once that's why we need
        // to know the total damage taken by mutant to show later
        int totalDamage = 0;

        foreach (GameObject targetPrefab in targetPrefabs)
        {
            // This part is where we will apply changes on data
            // Just loop through all effects, differentiate if its direct or not,
            // then apply corresponding changes
            foreach (Effect effect in ability.effects)
            {
                // There are two kinds of Effect Type, Direct and Status
                // Usually, direct effects for mutants are damage
                if (effect.effectType.Equals("Direct"))
                {
                    // Calculate damage of effect and add to total damage
                    // Definitely there are no double direct effect
                    // But just to be safe, this condition gets total to show later
                    totalDamage += targetPrefab.GetComponent<CharacterMonitor>().
                        MutantDamaged(effect.effectStrength, scavenger);
                    damageApplied = totalDamage;
                }
                // If its not a direct effect, its a status change
                else
                {
                    // Status effects for mutants are debuff
                    targetPrefab.GetComponent<CharacterMonitor>().MutantDebuffed(Instantiate(effect));
                }
            }
        }

        // After data's handled, its time to play animations
        // Avoid showing damage points if there's none
        if (totalDamage > 0)
        {
            StartCoroutine(animationManager.AttackAllMutants(scavengerPrefab, targetPrefabs,
                abilityIndex, totalDamage, ability.abilityRange));
            yield return new WaitForSeconds(1.5f);
        }

        foreach (GameObject targetPrefab in targetPrefabs)
        {
            // Unlike direct effects which animations will only play once,
            // each animation of status effect will be displayed
            foreach (Effect effect in ability.effects)
            {
                // Only do somehing if effect type is status
                if (effect.effectType.Equals("Status"))
                {
                    // There are two ways to apply a status effect, one is through
                    // variable and the other is condition
                    // Variable is where a change directly in Stat happens like Atk Down
                    if (effect.effectApplication.Equals("Variable"))
                    {
                        StartCoroutine(statusManager.ShowBuff(targetPrefab, 0));
                    }
                    // Condition is where a character is subject to, ie. poisoned
                    else
                    {
                        StartCoroutine(particleManager.CircleBurst(targetPrefab.transform.position));
                    }

                    yield return new WaitForSeconds(0.75f);
                }
            }
        }

        // If there isn't damage, only status effects, make scavenger go back
        // from fight stance to its default idle animation
        if (totalDamage == 0)
        {
            scavengerPrefab.GetComponent<CharacterMonitor>().Idle();
            yield return new WaitForSeconds(0.5f);
        }

        // Make way for dying animation if character is already dead
        if (!characterManager.AllMutantsAlive)
        {
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(EndOfTurn());
    }

    public IEnumerator EndOfTurn()
    {
        // End loop and start again
        attackController.ClearCurrentAbility();
        attackController.EnableAttackButtons(0);
        yield return new WaitForSeconds(1f);
        battleController.NextTurn();
    }
}
