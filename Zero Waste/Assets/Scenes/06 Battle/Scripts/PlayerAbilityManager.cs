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
    private TurnQueueManager turnQueueManager;
    private AudioManager audioManager;
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

    private int changeApplied;

    public int ChangeApplied
    {
        get { return changeApplied; }
        set { changeApplied = value; }
    }

    private GameObject scavengerObj;

    public GameObject ScavengerPrefab
    {
        get { return scavengerObj; }
        set { scavengerObj = value; }
    }

    private int position;

    public int Position
    {
        get { return position; }
        set { position = value; }
    }
    #endregion

    private bool switchingPhase;
    private float switchLength;

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
        turnQueueManager = FindObjectOfType<TurnQueueManager>();
        audioManager = FindObjectOfType<AudioManager>();

        switchingPhase = false;
    }

    // <summary>
    // Each ability has an ability manager, when player selects
    // an ability, other abilities are not aware of it
    // The attack controller binds all the abilities, so the system
    // knows when there is already a selected ability
    // </summary>
    public void ChooseAbility()
    {
        audioManager.PlaySound("Click 01");

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
                scavengerObj.GetComponent<Animator>().SetBool("Idle", true);
                scavengerObj.GetComponent<Animator>().SetBool("Turn", false);
                targetManager.CancelTargetSelection(attackController.SwitchedAbility);
            }
                
            targetManager.SelectTarget(ability, this);
        }
        else
        {
            scavengerObj.GetComponent<Animator>().SetBool("Idle", true);
            scavengerObj.GetComponent<Animator>().SetBool("Turn", false);
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
    public IEnumerator ExecuteAbility(GameObject selectedTarget, string targetType)
    {
        // Decrement antidote used by ability and update antidote bar
        scavengerObj.GetComponent<CharacterMonitor>().DecrementAntidote(ability.antRequirement);
        
        yield return null;

        // Apply effects if there's any on selected mutant
        ApplyAbilityEffects(selectedTarget, targetType);
    }

    // <summary>
    // </summary>
    void ApplyAbilityEffects(GameObject targetPrefab, string targetType)
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
    IEnumerator ScavengerToScavenger(GameObject targetObj)
    {
        changeApplied = 0;

        cameraManager.FocusOnScavengers(false);
        cameraManager.FocusOnMutants(false);
        yield return new WaitForSeconds(0.3f);

        if (ability.range.Equals("Self") || ability.range.Equals("Single"))
        {
            StartCoroutine(ToScavenger(targetObj));
            yield return new WaitForSeconds(1f);
        }
        else if (ability.range.Equals("AOE"))
        {
            List<GameObject> aliveScavengers = new List<GameObject>();

            foreach (GameObject scavengerObject in characterManager.GetAllCharacterPrefabs(1))
            {
                if (scavengerObject.GetComponent<CharacterMonitor>().IsAlive)
                    aliveScavengers.Add(scavengerObject);
            }

            foreach (GameObject scavengerObj in aliveScavengers)
            {
                StartCoroutine(ToScavenger(scavengerObj));

                if (ability.repeatAnimation)
                    yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(1f);
        }

        yield return null;
        turnQueueManager.FinishedTurn(scavenger);
        StartCoroutine(EndTurn());
    }

    IEnumerator ToScavenger(GameObject targetObj)
    {
        animationManager.PlayAnimation(ability, scavengerObj, targetObj, scavenger.characterType);

        foreach (Effect effect in ability.effects)
        {
            if (effect.type.Equals("Direct"))
            {
                int valueChanged = targetObj.GetComponent<CharacterMonitor>().ScavengerHealed(effect.target, effect.strength);
                changeApplied += valueChanged;

                // HP - Skill, Ult | ANT - Charge, Skill, Ult
                if (effect.target.Equals("HP"))
                {
                    CharacterMonitor scavMonitor = targetObj.GetComponent<CharacterMonitor>();
                    scavMonitor.EffectsAnimation(effect.effectIndex, effect);
                    StartCoroutine(statusManager.IncrementHealthBar(scavMonitor.CurrentHealth, scavMonitor.MaxHealth, scavMonitor.Position));
                    yield return new WaitForSeconds(1f);
                    StartCoroutine(statusManager.ShowValues(valueChanged.ToString(), scavMonitor.MaxHealth, ability.type, targetObj));
                }
                else
                {
                    CharacterMonitor scavMonitor = targetObj.GetComponent<CharacterMonitor>();
                    StartCoroutine(statusManager.IncrementAntidoteBar(scavMonitor.CurrentAnt, scavMonitor.MaxAnt, scavMonitor.Position));
                    StartCoroutine(particleManager.PlayParticles(effect.particleIndex, targetObj.transform.position));

                    if (ability.showValue)
                        StartCoroutine(statusManager.ShowValues(valueChanged.ToString(), scavMonitor.MaxHealth, ability.type, targetObj));

                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                // Buff Scavengers
                targetObj.GetComponent<CharacterMonitor>().ScavengerBuffed(Instantiate(effect));

                if (effect.application.Equals("CharStats"))
                {
                    targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(effect.effectIndex, effect);
                    StartCoroutine(statusManager.ShowBuff(targetObj, effect.state));
                    yield return new WaitForSeconds(1f);
                }

                if (effect.application.Equals("Condition"))
                {
                    targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(effect.effectIndex, effect);
                    
                    if(effect.particleIndex != -1)
                        StartCoroutine(particleManager.PlayParticles(effect.particleIndex, targetObj.transform.position));
                    yield return new WaitForSeconds(1f);
                }

                statusManager.AddEffectToStatusPanel("Scavenger", targetObj.GetComponent<CharacterMonitor>().Position, effect);
                statusManager.AddEffectsToStatusList("Scavenger", targetObj.GetComponent<CharacterMonitor>().Position, effect, ability.icon);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    // <summary>
    // </summary>
    IEnumerator ScavengerToMutant(GameObject targetObj)
    {
        changeApplied = 0;

        cameraManager.FocusOnScavengers(false);
        cameraManager.FocusOnMutants(false);
        yield return new WaitForSeconds(0.3f);

        if (ability.range.Equals("Single"))
        {
            StartCoroutine(ToMutant(targetObj));
            yield return new WaitForSeconds(3f);

            if (!targetObj.GetComponent<CharacterMonitor>().IsAlive)
                yield return new WaitForSeconds(1f);

            if (switchingPhase)
            {
                yield return new WaitForSeconds(switchLength + 0.5f);
                switchingPhase = false;
                switchLength = 0f;
            }

        }
        else if (ability.range.Equals("AOE"))
        {
            List<GameObject> aliveMutants = new List<GameObject>();

            foreach (GameObject mutantObject in characterManager.GetAllCharacterPrefabs(0))
            {
                if (mutantObject.GetComponent<CharacterMonitor>().IsAlive)
                    aliveMutants.Add(mutantObject);
            }

            int totalChangeApplied = 0;
            foreach (GameObject mutantObj in aliveMutants)
            {
                StartCoroutine(ToMutant(mutantObj));
                totalChangeApplied += changeApplied;

                if (ability.repeatAnimation)
                    yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(1f);
        }

        yield return null;
        turnQueueManager.FinishedTurn(scavenger);
        StartCoroutine(EndTurn());
    }

    IEnumerator ToMutant(GameObject targetObj)
    {
        bool switchPhase = false;
        int currentPhase = -1;
        int startIndex = 0;
        int endIndex = 0;
        
        animationManager.PlayAnimation(ability, scavengerObj, targetObj, scavenger.characterType);
        foreach (Effect effect in ability.effects)
        {
            // PL
            if (effect.type.Equals("Direct"))
            {
                int valueChanged = targetObj.GetComponent<CharacterMonitor>().MutantDamaged(effect.strength, scavenger);
                changeApplied += valueChanged;

                if (targetObj.GetComponent<CharacterMonitor>().Mutant is Boss)
                {
                    Boss boss = targetObj.GetComponent<CharacterMonitor>().Mutant as Boss;
                    foreach (bool clearedPhase in boss.hasClearedPhase)
                    {
                        if (clearedPhase)
                            currentPhase++;
                    }

                    if ((currentPhase > -1) && (currentPhase < boss.hasClearedPhase.Length))
                    {
                        switchPhase = boss.hasClearedPhase[currentPhase];
                        switchingPhase = switchPhase;

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

                StartCoroutine(statusManager.DecrementPollutionBar(valueChanged));
                yield return new WaitForSeconds(1f);
                StartCoroutine(statusManager.ShowValues(valueChanged.ToString(), targetObj.GetComponent<CharacterMonitor>().MaxHealth,
                    ability.type, targetObj));
            }

            if (effect.type.Equals("Status"))
            {
                // Debuff Mutants
                targetObj.GetComponent<CharacterMonitor>().MutantDebuffed(Instantiate(effect));

                if (effect.application.Equals("CharStats"))
                {
                    targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(effect.effectIndex, effect);
                    StartCoroutine(statusManager.ShowBuff(targetObj, effect.state));
                }

                if (effect.application.Equals("Condition"))
                {
                    targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(effect.effectIndex, effect);

                    if(effect.particleIndex != -1)
                        StartCoroutine(particleManager.PlayParticles(effect.particleIndex, targetObj.GetComponent<BoxCollider2D>().bounds.center));
                }
                yield return new WaitForSeconds(1f);
            }
        }

        if (targetObj.GetComponent<CharacterMonitor>().IsAlive)
        {
            if (switchPhase)
            {
                yield return new WaitForSeconds(2f);

                Boss boss = targetObj.GetComponent<CharacterMonitor>().Mutant as Boss;
                BoxCollider2D bossCollider = targetObj.GetComponent<BoxCollider2D>();
                StartCoroutine(particleManager.PlayParticles(0, bossCollider.bounds.center));
                while (startIndex < endIndex)
                {
                    Debug.Log("Apply " + boss.phaseEffects[startIndex].effectName);
                    cameraManager.Shake(true, 2);
                    if (boss.phaseEffects[startIndex].type.Equals("Status"))
                    {
                        if (boss.phaseEffects[startIndex].application.Equals("CharStats"))
                        {
                            targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(4, boss.phaseEffects[startIndex]);
                            StartCoroutine(statusManager.ShowBuff(targetObj, boss.phaseEffects[startIndex].state));
                        }

                        if (boss.phaseEffects[startIndex].application.Equals("Condition"))
                        {
                            targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(4, boss.phaseEffects[startIndex]);
                            StartCoroutine(particleManager.PlayParticles(boss.phaseEffects[startIndex].particleIndex, targetObj.transform.position));
                        }

                        yield return new WaitForSeconds(1.2f);
                        startIndex++;
                    }
                }

                cameraManager.Shake(false, 2);
            }
        }
        
    }

    public IEnumerator EndTurn()
    {
        // End loop and start again
        attackController.ClearCurrentAbility();
        attackController.EnableAttackButtons(0);
        yield return new WaitForSeconds(1f);
        battleController.NextTurn();
    }
}
