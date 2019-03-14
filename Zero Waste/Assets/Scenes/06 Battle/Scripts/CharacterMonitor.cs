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

    private double currentMaxHealth;

    public double MaxHealth
    {
        get { return currentMaxHealth; }
        set { currentMaxHealth = value; }
    }

    private double currentHealth;

    public double CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    private double currentMaxAnt;

    public double MaxAnt
    {
        get { return currentMaxAnt; }
        set { currentMaxAnt = value; }
    }

    private double currentAnt;

    public double CurrentAnt
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
    private bool characterTurn;
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
    // Transition from idle to fight when its character's turn
    // </summary>
    public void CharacterBattleStance()
    {
        // Get animator and trigger fight stance animation
        gameObject.GetComponent<Animator>().SetBool("Turn", true);
        characterTurn = true;
    }

    // <summary>
    // Gets scavengers max health, instead of the max health variable
    // which can be changed on runtime
    // </summary>
    public double GetScavengerMaxHealth()
    {
        return scavenger.baseHP + (int)((scavenger.currentLevel - 1) * scavenger.hpModifier);
    }

    // <summary>
    // Gets mutants max health, instead of the max health variable
    // which can be changed on runtime
    // </summary>
    public double GetMutantMaxHealth()
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

    public void IsHurt()
    {

    }

    public void IsHealed()
    {

    }
}
