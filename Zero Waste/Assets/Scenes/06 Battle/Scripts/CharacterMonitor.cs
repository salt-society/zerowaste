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
    #endregion

    #region Battle Managers
    private StatusManager statusManager;
    #endregion

    // <summary>
    // Called the moment character is rendered on screen.
    // Gets whatever the script needs to function.
    // </summary>
    public void InitializeMonitor()
    {
        // Set character init flag to false, since there isn't
        // any data yet the moment this function is  called
        characterInitialized = false;

        // Get Managers
        statusManager = FindObjectOfType<StatusManager>();
    }

    // <summary>
    //
    // </summary>
    void Update()
    {
        // Constantly check players health so it could be pronounced
        // dead and ignored in queue
        if (currentHealth <= 0) 
        {
            isAlive = false;
        }
        else
        {
            // Update values
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
            characterType = scavenger.characterType;

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
            characterType = mutant.characterType;

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

    public void CharacterAbilityAnimation(int abilityIndex)
    {
        switch (abilityIndex)
        {
            case 0:
                {
                    StartCoroutine(Attack01());
                    break;
                }
            case 1:
                {
                    StartCoroutine(Charge());
                    break;
                }
            case 2:
                {
                    StartCoroutine(CharacterSkill());
                    break;
                }
            case 3:
                {
                    StartCoroutine(SpecialMove());
                    break;
                }

        }
    }

    public IEnumerator Attack01()
    {
        gameObject.GetComponent<Animator>().SetBool("Attack 01", true);
        gameObject.GetComponent<Animator>().SetBool("Turn", false);
        yield return new WaitForSeconds(1.2f);
        gameObject.GetComponent<Animator>().SetBool("Attack 01", false);
    }

    public IEnumerator Attack02()
    {
        yield return null;
    }

    public IEnumerator Charge()
    {
        yield return null;
    }

    public IEnumerator CharacterSkill()
    {
        yield return null;
    }

    public IEnumerator SpecialMove()
    {
        yield return null;
    }

    public IEnumerator Damaged01()
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

    #endregion
}
