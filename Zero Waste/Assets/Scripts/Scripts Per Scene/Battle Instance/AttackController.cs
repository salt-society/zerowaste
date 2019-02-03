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

    public void DisplayAttackButtons(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        attackButtonPanel.SetActive(showComponent);
    }

    public void EnableAttackButtons(int enable)
    {
        bool showComponent = (enable > 0) ? true : false;

        foreach (Button attackButton in attackButtons)
        {
            Debug.Log(attackButton.name);
            attackButton.enabled = showComponent;
            Image[] buttons = attackButton.GetComponentsInChildren<Image>();

            foreach (Image button in buttons)
            {
                if (button.name.Equals("Overlay"))
                {
                    Debug.Log(button.GetInstanceID());
                    button.gameObject.SetActive(!showComponent);
                }
            }
        }
    }

    public void EnableAttackButton(int enable, int buttonNo)
    {
        bool showComponent = (enable > 0) ? true : false;
        Image[] buttons = attackButtons[buttonNo].GetComponentsInChildren<Image>();

        foreach (Image button in buttons)
        {
            if (button.name.Equals("Overlay"))
            {
                button.gameObject.SetActive(!showComponent);
                break;
            }
        }

        attackButtons[buttonNo].enabled = showComponent;
    }

    public void PlayerAttackSetup()
    {
        Player currentCharacter = turnQueueManager.GetCurrentCharacter() as Player;

        int i = 0;
        foreach (Button attackButton in attackButtons)
        {
            if (i == attackButtons.Length - 1)
            {
                EnableAttackButton(1, i);
            }
            else
            {
                attackButton.GetComponent<AbilityManager>().SetUpAbility(currentCharacter.abilities[i] as PlayerAbility);
                attackButton.GetComponent<AbilityManager>().IsAbilityAvailable(currentCharacter);
            }
            
            i++;
        }

    }
}
