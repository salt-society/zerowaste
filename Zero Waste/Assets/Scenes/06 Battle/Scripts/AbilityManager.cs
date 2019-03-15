using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public int abilityIndex;

    #region Battle Managers
    private BattleController battleController;
    private AttackController attackController;
    private TargetManager targetManager;
    private StatusManager statusManager;
    private CameraManager cameraManager;
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
    }

    // <summary>
    // Each ability has an ability manager, when player selects
    // an ability, other abilities are not aware of it
    // The attack controller binds all the abilities, so the system
    // knows when there is already a selected ability
    // </summary>
    public void ChooseAbility()
    {
        // Let attack controller know that player has chosen an ability
        // Note that you can't choose an ability that doesn't meet
        // ant requirement, in example Character Skill and Ultimate Move
        attackController.SetCurrentAbility(ability);

        // Checks if there's an ability to execute and cancels if none
        if (!attackController.IsCurrentAttackNull())
        {
            // If ability is switched, "Cancel Attack" message shouldn't be shown
            if (attackController.SwitchedAbility)
                targetManager.CancelTargetSelection(attackController.SwitchedAbility);

            targetManager.SelectTarget(ability, this);
        }
        else
            targetManager.CancelTargetSelection(attackController.SwitchedAbility);
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

        Debug.Log(scavenger.currentAnt + " >= " + ability.antRequirement + " : " + (scavenger.currentAnt >= ability.antRequirement));
    }

    // <summary>
    // </summary>
    public IEnumerator ExecuteAbility(GameObject selectedTarget, string targetType)
    {
        // Decrement antidote used by ability and update antidote bar
        scavengerPrefab.GetComponent<CharacterMonitor>().DecrementAntidote(ability.antRequirement);
        
        yield return null;

        // Apply effects if there's any on selected mutant
        ApplyEffects(selectedTarget, targetType);
    }

    // <summary>
    // </summary>
    void ApplyEffects(GameObject targetPrefab, string targetType)
    {
        // Scavenger to Scavenger
        if (targetType.Equals("Scavenger")) 
        {
            ScavengerToScavenger(targetPrefab);
        } 
        // Scavenger to Mutant
        else if(targetType.Equals("Mutant"))
        {
            StartCoroutine(ScavengerToMutant(targetPrefab));
        }
    }

    IEnumerator ScavengerToScavenger(GameObject targetPrefab)
    {
        // Data of target and it's position on battle ground
        Player target = targetPrefab.GetComponent<CharacterMonitor>().Scavenger;
        int position = targetPrefab.GetComponent<CharacterMonitor>().Position;
        
        // Apply each effect chosen ability have
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

        yield return null;
    }

    IEnumerator ScavengerToMutant(GameObject targetPrefab)
    {
        // Data of target is needed to update changes as well as its position
        Enemy mutant = targetPrefab.GetComponent<CharacterMonitor>().Mutant;
        int position = targetPrefab.GetComponent<CharacterMonitor>().Position;

        // Loop through effects ability have and apply each one of it
        int totalDamage = 0;
        int noOfDirectEffects = 0;
        foreach (Effect effect in ability.effects)
        {
            // There are two kinds of Effect Type, Direct and Status
            // Apply effects according to their type
            if (effect.effectType.Equals("Direct"))
            {

                // Calculate damage of effect
                int damage = targetPrefab.GetComponent<CharacterMonitor>().
                    MutantDamaged(effect.effectStrength, scavenger);
                noOfDirectEffects++;
                totalDamage += damage;
            }
            else if (effect.effectType.Equals("Status"))
            {
                Debug.Log("Status");

                int previousHealth = mutant.currentPollutionLevel;
                mutant.IsDebuffed(Instantiate(effect));

                if (effect.effectTarget.Equals("PL"))
                {
                    int currentHealth = mutant.currentPollutionLevel;
                    int damage = previousHealth - currentHealth;

                   
                }
                else
                {

                }
            }

        }

        // Unfocus camera
        cameraManager.FocusOnScavengers(false);
        cameraManager.FocusOnMutants(false);

        yield return new WaitForSeconds(0.5f);

        // Only apply this animation if there is a Direct
        if (noOfDirectEffects > 0)
        {
            // Effects to show simultaneously are the following
            // Damage Points, Attack Animation, Target Damage Animation, Camera Shake
            scavengerPrefab.GetComponent<CharacterMonitor>().CharacterAbilityAnimation(abilityIndex);
            StartCoroutine(targetPrefab.GetComponent<CharacterMonitor>().Damaged01());
            cameraManager.Shake(true);
            statusManager.ShowDamagePoints(totalDamage.ToString(), targetPrefab);
            StartCoroutine(statusManager.DecrementPollutionBar(totalDamage));

            // Delay
            yield return new WaitForSeconds(1f);

            // End effects and animations
            StartCoroutine(statusManager.HideDamagePoints());
            cameraManager.Shake(false);
        }
       

        // End battle loop and start again
        attackController.ClearCurrentAbility();
        attackController.EnableAttackButtons(0);
        yield return null;
        battleController.NextTurn();
    }
}
