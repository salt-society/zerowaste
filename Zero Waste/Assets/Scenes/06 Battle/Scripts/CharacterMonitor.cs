using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMonitor : MonoBehaviour
{
    #region Properties
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
    #endregion

    #region Private Variables
    private bool characterInitialized;
    private bool endOfTurn;
    #endregion

    #region Battle Managers
    private StatusManager statusManager;
    private ParticleManager particleManager;
    #endregion

    // <summary>
    // Called the moment character is rendered on screen.
    // Gets whatever the script needs to function.
    // </summary>
    public void InitializeMonitor()
    {
        // Set character init flag to false, since there isn't
        // any data yet the moment this function is called
        characterInitialized = false;
        endOfTurn = false;

        // Get Managers
        statusManager = FindObjectOfType<StatusManager>();
        particleManager = FindObjectOfType<ParticleManager>();
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
            if (currentHealth <= 0)
            {
                isAlive = false;
                StartCoroutine(MutantDead());
            }
        }

        string characterName = (scavenger != null) ? scavenger.characterName : mutant.characterName;
        // Debug.Log(characterName + ": " + isAlive + " | Health: " + currentHealth);

        // Check DOT effects every end of turn
        if (endOfTurn)
        {
            endOfTurn = false;
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

            characterInitialized = true;
            isAlive = true;
        }

        // Set up Mutant Properties
        if (mutant != null)
        {
            instanceId = mutant.GetInstanceID();
            characterType = "Mutant";

            currentMaxHealth = mutant.maxPollutionLevel;
            currentHealth = mutant.currentPollutionLevel;

            characterInitialized = true;
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

    #region Animations

    // <summary>
    // Transition from idle to fight when its character's turn
    // </summary>
    public void CharacterBattleStance()
    {
        // Trigger fight stance animation
        gameObject.GetComponent<Animator>().SetBool("Turn", true);
    }

    // <summary>
    // Play character's skeletal animation base on its ability index
    // So far function only works for Scavengers
    // </summary>
    public void CharacterAbilityAnimation(int abilityIndex)
    {
        switch (abilityIndex)
        {
                // Basic Attack
            case 0:
                {
                    StartCoroutine(BasicAttackAnimation());
                    break;
                }
                // Charge
            case 1:
                {
                    StartCoroutine(ChargeAnimation());
                    break;
                }
                // Character Skill
            case 2:
                {
                    StartCoroutine(CharacterSkillAnimation());
                    break;
                }
                // Special Move
            case 3:
                {
                    StartCoroutine(SpecialMoveAnimation());
                    break;
                }

        }
    }

    // <summary>
    // Play character's skeletal animation base on its status index
    // So far function only works for Scavengers
    // </summary>
    public void CharacterStatusChangeAnimation(int statusIndex)
    {
        switch (statusIndex)
        {
            // Direct Damage
            case 0:
                {
                    StartCoroutine(Damage());
                    break;
                }
            case 1:
                {
                    break;
                }
        }
    }

    public void Idle()
    {
        gameObject.GetComponent<Animator>().SetBool("Turn", false);
    }

    public IEnumerator BasicAttackAnimation()
    {
        gameObject.GetComponent<Animator>().SetBool("Attack 01", true);
        gameObject.GetComponent<Animator>().SetBool("Turn", false);
        yield return new WaitForSeconds(1.2f);
        gameObject.GetComponent<Animator>().SetBool("Attack 01", false);
    }

    public IEnumerator ChargeAnimation()
    {
        gameObject.GetComponent<Animator>().SetBool("Idle", true);
        gameObject.GetComponent<Animator>().SetBool("Heal", true);
        gameObject.GetComponent<Animator>().SetBool("Turn", false);
        yield return new WaitForSeconds(1.2f);
        gameObject.GetComponent<Animator>().SetBool("Idle", false);
        gameObject.GetComponent<Animator>().SetBool("Heal", false);
    }

    public IEnumerator CharacterSkillAnimation()
    {
        yield return null;
    }

    public IEnumerator SpecialMoveAnimation()
    {
        yield return null;
    }

    public IEnumerator Damage()
    {
        gameObject.GetComponent<Animator>().SetBool("Damaged 01", true);
        yield return new WaitForSeconds(1.2f);
        gameObject.GetComponent<Animator>().SetBool("Damaged 01", false);
    }

    #endregion

    #region Scavenger Status Update Functions

    // <summary>
    // Decrement antidote value and updates antidote bar
    // </summary>
    public void DecrementAntidote(int antRequirement)
    {
        scavenger.currentAnt = scavenger.CheckMin(scavenger.currentAnt - antRequirement);
        StartCoroutine(statusManager.DecrementAntidote(scavenger.currentAnt, scavenger.baseAnt, position));
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

    public void MutantHealed()
    {

    }

    public IEnumerator MutantDead()
    {
        // Show dying animation
        yield return new WaitForSeconds(2f);
        GetComponent<Animator>().SetBool("Dead", true);
        yield return new WaitForSeconds(0.75f);
        GetComponent<Animator>().SetBool("Dead", false);
        gameObject.SetActive(false);

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
