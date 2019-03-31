using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private bool isDying;

    public bool IsDying
    {
        get { return isDying; }
        set { isDying = value; }
    }

    private float switchLength;

    public float SwitchLength
    {
        get { return switchLength; }
        set { switchLength = value; }
    }

    #endregion

    #region Scripts
    private StatusManager statusManager;
    private ParticleManager particleManager;
    private AnimationManager animationManager;
    private CameraManager cameraManager;
    private TurnQueueManager turnQueueManager;
    private BattleInfoManager battleInfoManager;
    #endregion

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Ground")
        {
            gameObject.GetComponent<Animator>().enabled = true;
        }
    }

    // <summary>
    // Called the moment character is rendered on screen.
    // Gets whatever the script needs to function.
    // </summary>
    public void InitializeMonitor()
    {
        isDying = false;

        // Get Managers
        statusManager = FindObjectOfType<StatusManager>();
        particleManager = FindObjectOfType<ParticleManager>();
        animationManager = FindObjectOfType<AnimationManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        turnQueueManager = FindObjectOfType<TurnQueueManager>();
        battleInfoManager = FindObjectOfType<BattleInfoManager>();
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
            // Update character
            if (characterType.Equals("Scavenger"))
            {
                statusManager.detailedScavengerStatusPanel[position].transform.GetChild(3).GetChild(2).
                    gameObject.GetComponent<TextMeshProUGUI>().text = scavenger.currentSpd.ToString();
                statusManager.detailedScavengerStatusPanel[position].transform.GetChild(4).GetChild(2).
                    gameObject.GetComponent<TextMeshProUGUI>().text = scavenger.currentAtk.ToString();
                statusManager.detailedScavengerStatusPanel[position].transform.GetChild(5).GetChild(2).
                    gameObject.GetComponent<TextMeshProUGUI>().text = scavenger.currentDef.ToString();

                currentHealth = scavenger.currentHP;
                currentAnt = scavenger.currentAnt;
            }
            else
            {
                currentHealth = mutant.currentPollutionLevel;
            }

            // Check if dying or revived
            if (!isDying)
            {
                if (currentHealth <= (int)(currentMaxHealth * 0.25f))
                {
                    isDying = true;
                    StartCoroutine(CharacterDying());
                }
            }
            
            if(isDying)
            {
                if (currentHealth > (int)(currentMaxHealth * 0.25f))
                {
                    isDying = false;
                    StartCoroutine(CharacterRevive());
                }
            }
            
            // Check if character is dead
            if (currentHealth <= 0)
            {
                isAlive = false;
                StartCoroutine(CharacterDead());
            }
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

    public IEnumerator UpdateEffects()
    {
        List<Effect> updatedEffectList = new List<Effect>();
        
        List<int> effectNos = new List<int>();
        List<Effect> effectsToRemove = new List<Effect>();

        bool switchPhase = false;
        int currentPhase = -1;
        int startIndex = 0, endIndex = 0;

        if (scavenger != null)
        {
            if (scavenger.effects != null)
            {
                int effectNo = 0;
                foreach (Effect effect in scavenger.effects)
                {
                    if (effect.target.Equals("HP"))
                    {
                        if (effect.state.Equals("Buff"))
                        {
                            int valueChanged = currentHealth;
                            currentHealth = scavenger.CheckMax(currentHealth + effect.strength, "HP");
                            valueChanged = currentHealth - valueChanged;
                            scavenger.currentHP = currentHealth;
                            
                            StartCoroutine(statusManager.IncrementHealthBar(currentHealth, currentMaxHealth, position));
                            StartCoroutine(statusManager.ShowValues(valueChanged.ToString(), currentMaxHealth, "Defensive", gameObject, 1));
                        }
                        else
                        {
                            int valueChanged = currentHealth;
                            currentHealth = scavenger.CheckMin(currentHealth - effect.strength);
                            valueChanged -= currentHealth;
                            scavenger.currentHP = currentHealth;

                            StartCoroutine(statusManager.DecrementHealthBar(currentHealth, currentMaxHealth, position));
                            StartCoroutine(statusManager.ShowValues(valueChanged.ToString(), currentMaxHealth, "Offensive", gameObject, 0));
                        }
                    }
                    else if (effect.target.Equals("ANT"))
                    {
                        if (effect.state.Equals("Buff"))
                        {
                            int valueChanged = currentAnt;
                            currentAnt = scavenger.CheckMax(currentHealth + effect.strength, "ANT");
                            valueChanged = currentAnt - valueChanged;
                            scavenger.currentAnt = currentAnt;

                            StartCoroutine(statusManager.IncrementAntidoteBar(currentAnt, currentMaxAnt, position));
                            StartCoroutine(statusManager.ShowValues(valueChanged.ToString(), currentMaxAnt, "Defensive", gameObject, 2));
                        }
                        else
                        {
                            int valueChanged = currentAnt;
                            currentAnt = scavenger.CheckMin(currentHealth - effect.strength);
                            valueChanged -= currentAnt;
                            scavenger.currentAnt = currentAnt;
                            StartCoroutine(statusManager.DecrementAntidoteBar(currentAnt, currentMaxAnt, position));
                            StartCoroutine(statusManager.ShowValues(valueChanged.ToString(), currentMaxAnt, "Offensive", gameObject, 0));
                        }
                    }

                    if (isAlive)
                    {
                        if (effect.duration > 0)
                        {
                            effect.duration--;
                            updatedEffectList.Add(effect);

                            statusManager.UpdateEffect(effectNo, effect.duration, position);
                        }
                            
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
                            effectsToRemove.Add(effect);
                            effectNos.Add(effectNo);
                        }
                    }

                    effectNo++;
                }

                if (isAlive)
                {
                    IncrementAntidote(scavenger.currentAntGen);

                    int i = 0;
                    foreach (Effect effect in effectsToRemove)
                    {
                        scavenger.effects.Remove(effect);
                        statusManager.RemoveEffect("Scavenger", position, effectNos[i]);
                        i++;
                    }
                        
                }
            }
        }
        else
        {
            if (mutant.effects != null)
            {
                int effectNo = 0;
                foreach (Effect effect in mutant.effects)
                {
                    if (effect.target.Equals("PL"))
                    {
                        if (effect.state.Equals("Buff"))
                        {
                            int valueChanged = currentHealth;
                            currentHealth = mutant.CheckMax(currentHealth + effect.strength);
                            valueChanged = currentHealth - valueChanged;
                            mutant.currentPollutionLevel = currentHealth;
                                                      
                            StartCoroutine(statusManager.IncrementPollutionBar(valueChanged));
                            StartCoroutine(statusManager.ShowValues(valueChanged.ToString(), mutant.maxPollutionLevel, "Defensive", gameObject, 1));
                        }
                        else
                        {
                            int valueChanged = currentHealth;
                            currentHealth = mutant.CheckMax(currentHealth - effect.strength);
                            valueChanged -= currentHealth;
                            mutant.currentPollutionLevel = currentHealth;

                            // Check if a phase of boss is done and show change of phase
                            // at the end of all the updates of effects
                            if (mutant is Boss)
                            {
                                Boss boss = mutant as Boss;
                                foreach (bool clearedPhase in boss.hasClearedPhase)
                                {
                                    if (clearedPhase)
                                        currentPhase++;
                                }

                                if ((currentPhase > -1) && (currentPhase < boss.hasClearedPhase.Length))
                                {
                                    switchPhase = boss.hasClearedPhase[currentPhase];

                                    if (currentPhase > 0)
                                    {
                                        for (int i = currentPhase; i > 0; i--)
                                            startIndex += boss.effectNumber[i];

                                        for (int i = currentPhase; i >= 0; i--)
                                            endIndex += boss.effectNumber[i];
                                    }
                                    else
                                    {
                                        endIndex = boss.effectNumber[currentPhase];
                                    }

                                    switchLength = (float)endIndex;
                                }
                            }

                            StartCoroutine(statusManager.IncrementPollutionBar(valueChanged));
                            StartCoroutine(statusManager.ShowValues(valueChanged.ToString(), mutant.maxPollutionLevel, "Offensive", gameObject, 0));
                        }
                    }

                    if (isAlive)
                    {
                        if (effect.duration > 0)
                        {
                            effect.duration--;
                            updatedEffectList.Add(effect);

                            statusManager.UpdateEffect(effectNo, effect.duration, position);
                        }

                        if (effect.duration == 0)
                        {
                            switch (effect.target)
                            {
                                case "ATK":
                                    {
                                        if (effect.state == "Buff")
                                            mutant.currentAtk = mutant.CheckMin(mutant.currentAtk - effect.strength);
                                        else
                                            mutant.currentAtk = mutant.currentAtk + effect.strength;
                                        break;
                                    }
                                case "DEF":
                                    {
                                        if (effect.state == "Buff")
                                            mutant.currentDef = mutant.CheckMin(mutant.currentDef - effect.strength);
                                        else
                                            mutant.currentDef = mutant.currentDef + effect.strength;
                                        break;
                                    }
                                case "SPD":
                                    {
                                        if (effect.state == "Buff")
                                            mutant.currentSpd = mutant.CheckMin(mutant.currentSpd - effect.strength);
                                        else
                                            mutant.currentSpd = mutant.currentSpd + effect.strength;
                                        break;
                                    }
                            }
                            effectsToRemove.Add(effect);
                        }
                    }
                    effectNo++;
                }

                if (isAlive)
                {
                    foreach (EnemyAbility mutantAbility in mutant.instanceAbilities)
                    {
                        if (mutantAbility.turnTillActive > 0)
                            mutantAbility.turnTillActive--;
                    }

                    foreach (Effect effect in effectsToRemove)
                    {
                        mutant.effects.Remove(effect);
                    }
                        
                }
            }
        }

        foreach (Effect effect in updatedEffectList)
        {
            if (effect.application.Equals("Condition"))
            {
                EffectsAnimation(effect.effectIndex, effect);
                StartCoroutine(particleManager.PlayParticles(effect.particleIndex, gameObject.GetComponent<BoxCollider2D>().bounds.center));
                yield return new WaitForSeconds(effect.animationLength);
            }
        }

        if (switchPhase)
        {
            yield return new WaitForSeconds(2f);

            Boss boss = mutant as Boss;
            BoxCollider2D bossCollider = gameObject.GetComponent<BoxCollider2D>();
            StartCoroutine(particleManager.PlayParticles(0, bossCollider.bounds.center));
            while (startIndex < endIndex)
            {
                Debug.Log("Apply " + boss.phaseEffects[startIndex].effectName);
                cameraManager.Shake(true, 2);
                if (boss.phaseEffects[startIndex].type.Equals("Status"))
                {
                    if (boss.phaseEffects[startIndex].application.Equals("CharStats"))
                    {
                        EffectsAnimation(4, boss.phaseEffects[startIndex]);
                        StartCoroutine(statusManager.ShowBuff(gameObject, boss.phaseEffects[startIndex].state));
                    }

                    if (boss.phaseEffects[startIndex].application.Equals("Condition"))
                    {
                        EffectsAnimation(4, boss.phaseEffects[startIndex]);
                        StartCoroutine(particleManager.PlayParticles(boss.phaseEffects[startIndex].particleIndex, bossCollider.bounds.center));
                    }

                    yield return new WaitForSeconds(1.2f);
                    startIndex++;
                }
            }

            cameraManager.Shake(false, 2);
        }
    }

    #region Animations

    // <summary>
    // Transition from idle to fight when its character's turn
    // </summary>
    public void CharacterBattleStance()
    {
        // Trigger fight stance animation
        if (!isDying)
            gameObject.GetComponent<Animator>().SetBool("Turn", true);
        else
            gameObject.GetComponent<Animator>().SetBool("Dying Turn", true);
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
        if (!isDying)
        {
            gameObject.GetComponent<Animator>().SetBool("Idle", false);
            gameObject.GetComponent<Animator>().SetBool("Basic Attack", true);
            gameObject.GetComponent<Animator>().SetBool("Turn", false);
            yield return new WaitForSeconds(length);
            gameObject.GetComponent<Animator>().SetBool("Basic Attack", false);
            gameObject.GetComponent<Animator>().SetBool("Idle", true);
        }
        else
        {
            gameObject.GetComponent<Animator>().SetBool("Basic Attack", true);
            yield return new WaitForSeconds(length);
            gameObject.GetComponent<Animator>().SetBool("Basic Attack", false);
            yield return new WaitForSeconds(0.5f);
            gameObject.GetComponent<Animator>().SetBool("Dying Turn", false);
        }
        
    }

    public IEnumerator ScavengerCharge(float length)
    {
        if (!isDying)
        {
            gameObject.GetComponent<Animator>().SetBool("Idle", false);
            gameObject.GetComponent<Animator>().SetBool("Charge", true);
            gameObject.GetComponent<Animator>().SetBool("Turn", false);
            yield return new WaitForSeconds(length);
            gameObject.GetComponent<Animator>().SetBool("Charge", false);
            gameObject.GetComponent<Animator>().SetBool("Idle", true);
        }
        else
        {
            gameObject.GetComponent<Animator>().SetBool("Charge", true);
            yield return new WaitForSeconds(length);
            gameObject.GetComponent<Animator>().SetBool("Charge", false);
            yield return new WaitForSeconds(0.5f);
            gameObject.GetComponent<Animator>().SetBool("Dying Turn", false);
        }
        
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

        yield return new WaitForSeconds(1f);

        if (characterType.Equals("Scavenger")) 
        {
            turnQueueManager.CharacterDead(scavenger);
            statusManager.HideStatusPanel(position);
        }

        if (characterType.Equals("Mutant"))
        {
            turnQueueManager.CharacterDead(mutant);
            battleInfoManager.AddScrap(mutant.currentScrapReward);
            battleInfoManager.AddExp(mutant.currentEXPReward);
        }

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
        Debug.Log("Character dying...");
        gameObject.GetComponent<Animator>().SetBool("Idle", false);
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<Animator>().SetBool("Dying", true);

    }

    public IEnumerator CharacterRevive()
    {
        Debug.Log("Character revived...");
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<Animator>().SetBool("Dying", false);
        gameObject.GetComponent<Animator>().SetBool("Dying Turn", false);
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
        Debug.Log("Status effect applied to: " + ((scavenger != null) ? scavenger.characterName : mutant.characterName));
        Debug.Log("Effect Name: " + effect.effectName);
        Debug.Log("Animation: " + effect.receiveAnim);

        gameObject.GetComponent<Animator>().SetInteger("Status Effect", effect.effectIndex);
        gameObject.GetComponent<Animator>().SetBool(effect.receiveAnim, true);
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<Animator>().SetInteger("Status Effect", 0);
        gameObject.GetComponent<Animator>().SetBool(effect.receiveAnim, false);
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
        int damage = scavenger.IsAttacked(targetStat, statModifier, mutant);
        currentHealth = scavenger.currentHP;
        currentAnt = scavenger.currentAnt;

        return damage;
    }

    public int ScavengerHealed(string targetStat, int statModifier)
    {
        int heal = scavenger.IsHealed(targetStat, statModifier);
        currentHealth = scavenger.currentHP;
        currentAnt = scavenger.currentAnt;

        return heal;
    }

    public void ScavengerBuffed(Effect effect)
    {
        scavenger.IsBuffed(Instantiate(effect));
        currentHealth = scavenger.currentHP;
        currentAnt = scavenger.currentAnt;
    }

    public void ScavengerDebuffed(Effect effect)
    {
        scavenger.IsDebuffed(Instantiate(effect));
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
        mutant.IsDebuffed(Instantiate(effect));
        currentHealth = mutant.currentPollutionLevel;
    }

    // <summary>
    // Decreases a mutant's stat
    // </summary>
    public void MutantDebuffed(Effect effect)
    {
        // Apply debuff and update values
        mutant.IsDebuffed(Instantiate(effect));
        currentHealth = mutant.currentPollutionLevel;
    }
    
    #endregion
}
