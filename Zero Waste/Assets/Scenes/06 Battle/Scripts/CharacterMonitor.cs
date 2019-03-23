using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMonitor : MonoBehaviour
{
    #region Private Variables

    private Player scavenger;

    public Player Scavenger
    {
        get { return scavenger; }
        set { scavenger = value; }
    }

    private Enemy mutant;

    public Enemy Mutant
    {
        get { return mutant; }
        set { mutant = value; }
    }

    private int position;

    public int Position
    {
        get { return position; }
        set { position = value; }
    }

    private string characterType;

    public string CharacterType
    {
        get { return characterType; }
        set { characterType = value; }
    }

    private int instanceId;

    public int InstanceId
    {
        get { return instanceId; }
        set { instanceId = value; }
    }

    private int currentMaxHealth;

    public int MaxHealth
    {
        get { return currentMaxHealth; }
        set { currentMaxHealth = value; }
    }

    private int currentHealth;

    public int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    private int currentMaxAnt;

    public int MaxAnt
    {
        get { return currentMaxAnt; }
        set { currentMaxAnt = value; }
    }

    private int currentAnt;

    public int CurrentAnt
    {
        get { return currentAnt; }
        set { currentAnt = value; }
    }

    private bool isAlive;

    public bool IsAlive
    {
        get { return isAlive; }
        set { isAlive = value; }
    }

    private bool endOfLoop;

    public bool EndOfLoop
    {
        get { return endOfLoop; }
        set { endOfLoop = value; }
    }

    #endregion

    #region Scripts
    private StatusManager statusManager;
    private ParticleManager particleManager;
    private AnimationManager animationManager;
    private CameraManager cameraManager;
    #endregion

    public bool dying;

    // <summary>
    // Called the moment character is rendered on screen.
    // Gets whatever the script needs to function.
    // </summary>
    public void InitializeMonitor()
    {
        endOfLoop = false;
        dying = false;

        // Get Managers
        statusManager = FindObjectOfType<StatusManager>();
        particleManager = FindObjectOfType<ParticleManager>();
        animationManager = FindObjectOfType<AnimationManager>();
        cameraManager = FindObjectOfType<CameraManager>();
    }

    // <summary>
    //
    // </summary>
    void Update()
    {
        // Constantly check players health so it could be pronounced
        // dead and ignored in queue
        if (isAlive)
        {
            if (!dying)
            {
                if (currentHealth <= (int)(currentMaxHealth * 0.25f))
                {
                    dying = true;
                    StartCoroutine(CharacterDying());
                }
            }
            
            if(dying)
            {
                if (currentMaxHealth > (int)(currentMaxHealth * 0.25))
                {
                    dying = false;
                    StartCoroutine(CharacterRevive());
                }
            }
            
            if (currentHealth <= 0)
            {
                isAlive = false;
                StartCoroutine(CharacterDead());
            }

            // Check dot effects every end of loop
            /*if (endOfLoop)
            {
                endOfLoop = false;
                StartCoroutine(UpdateScavengerEffects());
            }*/
        }
    }

    // <summary>
    // Sets character data, depends on which character is not empty
    // Always make sure to pass values first before invoking this function
    // </summary>
    public void SetCharacter()
    {
        // Set up Scavenger Properties
        if (scavenger != null)
        {
            instanceId = scavenger.GetInstanceID();
            characterType = "Scavenger";

            currentMaxHealth = GetScavengerMaxHealth();
            currentHealth = scavenger.currentHP;
            currentMaxAnt = scavenger.baseAnt;
            currentAnt = scavenger.currentAnt;

            isAlive = true;
        }

        // Set up Mutant Properties
        if (mutant != null)
        {
            instanceId = mutant.GetInstanceID();
            characterType = mutant.characterType;

            currentMaxHealth = mutant.maxPollutionLevel;
            currentHealth = mutant.currentPollutionLevel;

            isAlive = true;
        }
    }

    // <summary>
    // Gets scavengers max health, instead of the max health variable
    // which can be changed on runtime
    // </summary>
    public int GetScavengerMaxHealth()
    {
        return scavenger.baseHP + (int)((scavenger.currentLevel - 1) * scavenger.hpModifier);
    }

    // <summary>
    // Gets mutants max health, instead of the max health variable
    // which can be changed on runtime
    // </summary>
    public int GetMutantMaxHealth()
    {
        return mutant.maxPollutionLevel;
    }

    // <summary>
    // Returns gameObject of Character
    // </summary>
    public GameObject GetCharacterPrefab()
    {
        return gameObject;
    }

    // <summary>
    // Returns true if target type passed matches character type
    // </summary>
    public bool CheckCharacterType(string targetType)
    {
        return (characterType.Equals(targetType)) ? true : false;
    }

    public IEnumerator UpdateScavengerEffects()
    {
        List<Effect> effectsToRemove = new List<Effect>();

        // Check if there are effects applied to scavenger to update
        if (scavenger.effects.Count > 0)
        {
            // Loop through effect list, which are all status
            // so there is no need to check type
            foreach (Effect effect in scavenger.effects)
            {
                // There are two ways or timing to apply an effect
                // Effects applied every end of loop (eol)
                // Effects applied on cast (oc), lasts for n loops

                #region DOT
                // Always check first effects applied every end of loop
                if (effect.target.Equals("HP"))
                {
                    // Check state of effect, if its buff (positive, +)
                    // or debuff (negative, -)
                    if (effect.state.Equals("Buff"))
                    {
                        currentHealth = scavenger.CheckMax(currentHealth + effect.strength, "HP");
                        scavenger.currentHP = currentHealth;
                        StartCoroutine(statusManager.IncrementHealthBar(currentHealth, currentMaxHealth, position));
                    }
                    else
                    {
                        currentHealth = scavenger.CheckMin(currentHealth - effect.strength);
                        scavenger.currentHP = currentHealth;
                        StartCoroutine(statusManager.DecrementHealthBar(currentHealth, currentMaxHealth, position));
                    }

                    if (effect.application.Equals("Condition"))
                    {
                        EffectsAnimation(4, effect);
                        StartCoroutine(particleManager.PlayParticles(effect.particleIndex, gameObject.transform.position));
                        yield return new WaitForSeconds(1f);
                    }
                }
                else if (effect.target.Equals("ANT"))
                {
                    // Check state of effect, if its buff (positive, +)
                    // or debuff (negative, -)
                    if (effect.state.Equals("Buff"))
                    {
                        currentAnt = scavenger.CheckMax(currentHealth + effect.strength, "ANT");
                        scavenger.currentAnt = currentAnt;
                        StartCoroutine(statusManager.IncrementAntidoteBar(currentAnt, currentMaxAnt, position));
                    }
                    else
                    {
                        currentAnt = scavenger.CheckMin(currentHealth - effect.strength);
                        scavenger.currentAnt = currentAnt;
                        StartCoroutine(statusManager.DecrementAntidoteBar(currentAnt, currentMaxAnt, position));
                    }

                    if (effect.application.Equals("Condition"))
                    {
                        EffectsAnimation(4, effect);
                        StartCoroutine(particleManager.PlayParticles(effect.particleIndex, gameObject.transform.position));
                        yield return new WaitForSeconds(1f);
                    }
                }
                #endregion

                // Next to do is to decrement effect duration, whether its eol or oc
                // Before that, see first if character is still alive as damage over time effects
                // (previously check effect that targets hp) could've killed him/her
                if (isAlive)
                {
                    // If character is still alive, reduce duration of effect
                    if (effect.duration > 0)
                    {
                        effect.duration--;

                        Debug.Log("Update duration");
                        statusManager.UpdateEffectDuration(effect, position);
                        yield return new WaitForSeconds(2f);
                    }

                    #region Remove effect
                    // Revert back character stats to its original value if
                    // effects duration is finished
                    // Effects that are negative are infinite, stays as long as battle isn't over
                    if (effect.duration == 0)
                    {
                        switch (effect.target)
                        {
                            case "ATK":
                                {
                                    if (effect.state == "Buff")
                                        scavenger.currentAtk = scavenger.CheckMin(scavenger.currentAtk - effect.strength);
                                    else
                                        scavenger.currentAtk = scavenger.currentAtk + effect.strength;
                                    break;
                                }
                            case "DEF":
                                {
                                    if (effect.state == "Buff")
                                        scavenger.currentDef = scavenger.CheckMin(scavenger.currentDef - effect.strength);
                                    else
                                        scavenger.currentDef = scavenger.currentDef + effect.strength;
                                    break;
                                }
                            case "SPD":
                                {
                                    if (effect.state == "Buff")
                                        scavenger.currentSpd = scavenger.CheckMin(scavenger.currentSpd - effect.strength);
                                    else
                                        scavenger.currentSpd = scavenger.currentSpd + effect.strength;
                                    break;
                                }
                            case "ANTGEN":
                                {
                                    if (effect.state == "Buff")
                                        scavenger.currentAntGen = scavenger.CheckMin(scavenger.currentAntGen - effect.strength);
                                    else
                                        scavenger.currentAntGen = scavenger.currentAntGen + effect.strength;
                                    break;
                                }
                            case "TL":
                                {
                                    if (effect.state == "Buff")
                                        scavenger.currentThreatLevel = scavenger.CheckMin(scavenger.currentThreatLevel - effect.strength);
                                    else
                                        scavenger.currentThreatLevel = scavenger.currentThreatLevel + effect.strength;
                                    break;
                                }
                        }

                        // Then add it to effects that should be removed
                        effectsToRemove.Add(effect);
                    }
                    #endregion
                }
            }
        }

        yield return new WaitForSeconds(1f);

        // Just to be sure that character is still alive
        if (isAlive)
        {
            // Every end of loop, ant of scavengers generate
            IncrementAntidote(scavenger.currentAntGen);

            // Remove effects
            foreach (Effect effect in effectsToRemove)
                scavenger.effects.Remove(effect);
        }
    }

    #region Animations

    // <summary>
    // Transition from idle to fight when its character's turn
    // </summary>
    public void CharacterBattleStance()
    {
        // Trigger fight stance animation
        gameObject.GetComponent<Animator>().SetBool("Turn", true);
    }

    public void AbilityAnimations(Ability ability)
    {
        switch (ability.index)
        {
            case 0:
                {
                    if (characterType.Equals("Scavenger"))
                        StartCoroutine(ScavengerBasicAttack(ability.length));

                    if (characterType.Equals("Mutant"))
                        StartCoroutine(MutantBasicAttack(ability.length));
               
                    break;
                }
            case 1:
                {
                    if (characterType.Equals("Scavenger"))
                        StartCoroutine(ScavengerCharge(ability.length));

                    if (characterType.Equals("Mutant"))
                        StartCoroutine(MutantSkill01(ability.length));

                    break;
                }
            case 2:
                {
                    if (characterType.Equals("Scavenger"))
                        StartCoroutine(ScavengerSkill(ability.length));

                    if (characterType.Equals("Mutant"))
                        StartCoroutine(MutantSkill02(ability.length));

                    break;
                }
            case 3:
                {
                    if (characterType.Equals("Scavenger"))
                        StartCoroutine(ScavengerUltimate(ability.length));

                    if (characterType.Equals("Mutant"))
                        StartCoroutine(MutantUltimate(ability.length));

                    break;
                }
        }
    }

    public void EffectsAnimation(int animationIndex, Effect effect)
    {
        switch (animationIndex)
        {
            case 0:
                {
                    StartCoroutine(Hurt());
                    break;
                }
            case 1:
                {
                    StartCoroutine(Heal());
                    break;
                }
            case 2:
                {
                    StartCoroutine(Buff());
                    break;
                }
            case 3:
                {
                    StartCoroutine(Debuff());
                    break;
                }
            case 4:
                {
                    StartCoroutine(StatusEffect(effect));
                    break;
                }
        }
    }

    public IEnumerator ScavengerBasicAttack(float length)
    {
        gameObject.GetComponent<Animator>().SetBool("Idle", false);
        gameObject.GetComponent<Animator>().SetBool("Basic Attack", true);
        gameObject.GetComponent<Animator>().SetBool("Turn", false);
        yield return new WaitForSeconds(length);
        gameObject.GetComponent<Animator>().SetBool("Basic Attack", false);
        gameObject.GetComponent<Animator>().SetBool("Idle", true);
    }

    public IEnumerator ScavengerCharge(float length)
    {
        gameObject.GetComponent<Animator>().SetBool("Idle", false);
        gameObject.GetComponent<Animator>().SetBool("Charge", true);
        gameObject.GetComponent<Animator>().SetBool("Turn", false);
        yield return new WaitForSeconds(length);
        gameObject.GetComponent<Animator>().SetBool("Charge", false);
        gameObject.GetComponent<Animator>().SetBool("Idle", true);
    }

    public IEnumerator ScavengerSkill(float length)
    {
        gameObject.GetComponent<Animator>().SetBool("Idle", false);
        gameObject.GetComponent<Animator>().SetBool("Skill", true);
        gameObject.GetComponent<Animator>().SetBool("Turn", false);
        yield return new WaitForSeconds(length);
        gameObject.GetComponent<Animator>().SetBool("Skill", false);
    }

    public IEnumerator ScavengerUltimate(float length)
    {
        gameObject.GetComponent<Animator>().SetBool("Idle", false);
        gameObject.GetComponent<Animator>().SetBool("Ultimate", true);
        gameObject.GetComponent<Animator>().SetBool("Turn", false);
        yield return new WaitForSeconds(length);
        gameObject.GetComponent<Animator>().SetBool("Ultimate", false);
        gameObject.GetComponent<Animator>().SetBool("Idle", true);
    }

    public IEnumerator MutantBasicAttack(float length)
    {
        gameObject.GetComponent<Animator>().SetBool("Basic Attack", true);
        gameObject.GetComponent<Animator>().SetBool("Turn", false);
        yield return new WaitForSeconds(length);
        gameObject.GetComponent<Animator>().SetBool("Basic Attack", false);
    }

    public IEnumerator MutantSkill01(float length)
    {
        gameObject.GetComponent<Animator>().SetBool("Skill 01", true);
        gameObject.GetComponent<Animator>().SetBool("Turn", false);
        yield return new WaitForSeconds(length);
        gameObject.GetComponent<Animator>().SetBool("Skill 01", false);
    }

    public IEnumerator MutantSkill02(float length)
    {
        gameObject.GetComponent<Animator>().SetBool("Skill 01", true);
        gameObject.GetComponent<Animator>().SetBool("Turn", false);
        yield return new WaitForSeconds(length);
        gameObject.GetComponent<Animator>().SetBool("Skill 01", false);

    }

    public IEnumerator MutantUltimate(float length)
    {
        gameObject.GetComponent<Animator>().SetBool("Ultimate", true);
        gameObject.GetComponent<Animator>().SetBool("Turn", false);
        yield return new WaitForSeconds(length);
        gameObject.GetComponent<Animator>().SetBool("Ultimate", false);
    }

    public IEnumerator CharacterDead()
    {
        // Show dying animation
        yield return new WaitForSeconds(2f);
        GetComponent<Animator>().SetBool("Dead", true);
        yield return new WaitForSeconds(1f);
        GetComponent<Animator>().SetBool("Dead", false);

        // We need to turn off the box collider for this mutant
        // to not be part of the target selection but
        // turning off the box collider will make make mutant fall
        // indefinitely so we need to disable the rigidbody too
        // Another easy way is to just destroy the gameObject
        // but we will do this just in case of revive function
        /*Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.isKinematic = true;
        GetComponent<BoxCollider2D>().enabled = false;*/
    }

    public IEnumerator CharacterDying()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<Animator>().SetBool("Dying", true);
    }

    public IEnumerator CharacterRevive()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<Animator>().SetBool("Dying", false);
    }

    public IEnumerator Hurt()
    {
        gameObject.GetComponent<Animator>().SetBool("Hurt", true);
        yield return new WaitForSeconds(1.2f);
        gameObject.GetComponent<Animator>().SetBool("Hurt", false);
    }

    public IEnumerator Heal()
    {
        gameObject.GetComponent<Animator>().SetBool("Heal", true);
        yield return new WaitForSeconds(1.2f);
        gameObject.GetComponent<Animator>().SetBool("Heal", false);
    }

    public IEnumerator AntGen()
    {
        yield return null;
    }

    public IEnumerator Buff()
    {
        yield return null;
    }

    public IEnumerator Debuff()
    {
        yield return null;
    }

    public IEnumerator StatusEffect(Effect effect)
    {
        if (dying)
            gameObject.GetComponent<Animator>().SetBool("Dying Idle", false);
        else
            gameObject.GetComponent<Animator>().SetBool("Idle", false);

        gameObject.GetComponent<Animator>().SetInteger("Status", effect.effectIndex);
        gameObject.GetComponent<Animator>().SetBool(effect.state, true);
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<Animator>().SetInteger("Status", 0);
        gameObject.GetComponent<Animator>().SetBool(effect.state, false);
    }

    #endregion

    #region Scavenger Status Update Functions

    public void IncrementAntidote(int antGen)
    {
        scavenger.currentAnt = scavenger.CheckMax(scavenger.currentAnt + antGen, "ANT");
        currentAnt = scavenger.currentAnt;
        StartCoroutine(statusManager.IncrementAntidoteBar(scavenger.currentAnt, scavenger.baseAnt, position));
    }

    // <summary>
    // Decrement antidote value and updates antidote bar
    // </summary>
    public void DecrementAntidote(int antReq)
    {
        scavenger.currentAnt = scavenger.CheckMin(scavenger.currentAnt - antReq);
        currentAnt = scavenger.currentAnt;
        StartCoroutine(statusManager.DecrementAntidoteBar(scavenger.currentAnt, scavenger.baseAnt, position));
    }

    public int ScavengerDamaged(string targetStat, int statModifier, Enemy mutant)
    {
        scavenger.IsAttacked(targetStat, statModifier, mutant);

        if (targetStat.Equals("HP"))
        {
            int previousHealth = currentHealth;
            currentHealth = scavenger.currentHP;
            int damage = previousHealth - currentHealth;
            return damage;
        }
        else
        {
            int previousAnt = currentAnt;
            currentAnt = scavenger.currentAnt;
            int antLoss = previousAnt - currentAnt;
            return antLoss;
        }
        
    }

    public int ScavengerHealed(string targetStat, int statModifier)
    {
        scavenger.IsHealed(targetStat, statModifier);

        if (targetStat.Equals("HP"))
        {
            int previousHealth = currentHealth;
            currentHealth = scavenger.currentHP;
            int healValue = currentHealth - previousHealth;
            return healValue;
        }
        else
        {
            int previousAnt = currentAnt;
            currentAnt = scavenger.currentAnt;
            int antGain = currentAnt - previousAnt;
            return antGain;
        }
    }

    public void ScavengerBuffed(Effect effect)
    {
        scavenger.IsBuffed(effect);
        currentHealth = scavenger.currentHP;
        currentAnt = scavenger.currentAnt;
    }

    public void ScavengerDebuffed(Effect effect)
    {
        scavenger.IsBuffed(effect);
        currentHealth = scavenger.currentHP;
        currentAnt = scavenger.currentAnt;
    }

    #endregion

    #region Mutant Status Update Functions

    // <summary>
    // Decrement mutant's pollution level
    // </summary>
    public int MutantDamaged(int effectStrength, Player scavenger)
    {
        // Calculate damage taken from Scavenger's attack
        int previousHealth = currentHealth;
        mutant.IsAttacked(effectStrength, scavenger);

        currentHealth = mutant.currentPollutionLevel;
        int damage = previousHealth - currentHealth;

        return damage;
    }

    public int MutantHealed(int effectStrength)
    {
        // Calculate damage taken from Scavenger's attack
        int previousHealth = currentHealth;
        mutant.IsHealed(effectStrength);

        currentHealth = mutant.currentPollutionLevel;
        int healValue = currentHealth - previousHealth;

        return healValue;
    }

    public void MutantBuffed(Effect effect)
    {
        // Apply debuff and update values
        mutant.IsDebuffed(effect);
        currentHealth = mutant.currentPollutionLevel;
    }

    // <summary>
    // Decreases a mutant's stat
    // </summary>
    public void MutantDebuffed(Effect effect)
    {
        // Apply debuff and update values
        mutant.IsDebuffed(effect);
        currentHealth = mutant.currentPollutionLevel;
    }
    
    #endregion
}
