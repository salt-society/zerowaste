using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackController : MonoBehaviour
{
    [Header("Attack Panel")]
    public GameObject attackButtonPanel;
    public Button[] attackButtons;

    [Header("Manager")]
    public TurnQueueManager turnQueueManager;
    public CharacterManager characterManager;

    private PlayerAbility chosenAbility;
    private bool switchedAbility;

    public bool SwitchedAbility
    {
        get { return switchedAbility; }
        set { switchedAbility = value; }
    }

    // <summary>
    // Displays attack buttons
    // </summary>
    public void DisplayAttackButtons(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        attackButtonPanel.SetActive(showComponent);
    }

    public void ShowAttackButtons()
    {
        attackButtonPanel.GetComponent<Animator>().SetBool("Exit", false);
    }

    public void HideAttackButtons()
    {
        attackButtonPanel.GetComponent<Animator>().SetBool("Exit", true);
    }

    // <summary>
    // Displays attack buttons
    // </summary>
    public void EnableAttackButtons(int enable)
    {
        bool showComponent = (enable > 0) ? true : false;
        foreach (Button attackButton in attackButtons)
        {
            attackButton.transform.GetChild(1).gameObject.SetActive(!showComponent);
            attackButton.enabled = showComponent;
        }
    }

    public void EnableAttackButton(int enable, int buttonNo)
    {
        bool showComponent = (enable > 0) ? true : false;
        attackButtons[buttonNo].transform.GetChild(1).gameObject.SetActive(!showComponent);
        attackButtons[buttonNo].enabled = showComponent;
    }

    // <summary>
    // Sets up scavenger abilities by calling each ability's manger script
    // to pass current character's ability set
    // </summary>
    public void ScavengerAttackSetup()
    {
        // Get current Scavenger, his data, prefab, and position
        Player currentCharacter = turnQueueManager.GetCurrentCharacter() as Player;
        GameObject currentCharacterPrefab = characterManager.GetScavengerPrefab(currentCharacter);
        int position = characterManager.GetScavengerPosition(currentCharacter);

        // Set up ability
        int i = 0;
        foreach (Button attackButton in attackButtons)
        {
            // Booster is part of the attack buttons array, but is not an ability
            // therefore it should be isolated from the rest, always first on array, and be enabled
            if (i == attackButtons.Length - 1)
            {
                EnableAttackButton(1, i);
            }
            // Rest of buttons in array are the ability or attack buttons
            else
            {
                // Get ability manager attached to each button and send scavenger's prefab,
                // ability, check if ability is available, and send position of scavenger
                attackButton.GetComponent<PlayerAbilityManager>().Scavenger = currentCharacter;
                attackButton.GetComponent<PlayerAbilityManager>().ScavengerPrefab = currentCharacterPrefab;
                attackButton.GetComponent<PlayerAbilityManager>().SetupAbility(i);
                attackButton.GetComponent<PlayerAbilityManager>().IsAbilityAvailable();
                attackButton.GetComponent<PlayerAbilityManager>().Position = position;
            }
            
            i++;
        }
    }

    // <summary>
    // Sets current ability chosen by user to execute
    // </summary>
    public void SetCurrentAbility(PlayerAbility chosenAbility)
    {
        // Set ability if empty
        if (this.chosenAbility == null)
        {
            this.chosenAbility = chosenAbility;
            switchedAbility = false;
        }
        // Selecting same ability cancels its execution
        // if that ability requires target selection
        else if (this.chosenAbility.Equals(chosenAbility))
        {
            this.chosenAbility = null;
            switchedAbility = false;
        }
        // Switch from one ability to another
        else if (this.chosenAbility != null)
        {
            this.chosenAbility = chosenAbility;
            switchedAbility = true;
        }
    }

    // <summary>
    // Removes chosen ability
    // </summary>
    public void ClearCurrentAbility()
    {
        chosenAbility = null;
    }

    // <summary>
    // Returns whether there's already an ability chosen or none
    // </summary>
    public bool IsCurrentAttackNull()
    {
        return (chosenAbility == null) ? true: false;
    }
}
