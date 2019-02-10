using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [Header("Managers")]
    public BattleInfoManager battleInfoManager;
    public TurnQueueManager turnQueueManager;
    public CharacterManager characterManager;

    private string targetType;
    private bool canSelectTarget = false;

    private AbilityManager abilityManager;
   
    public void SelectTarget(PlayerAbility ability, AbilityManager abilityManager)
    {
        this.abilityManager = abilityManager;

        // Determine which characters are selectable base on Ability Type
        // Offensive = Attack Enemy
        // Defensive = Apply Buff to Self or Team Mates
        if (ability.abilityType.Equals("Offensive"))
        {
            // Set what type of character should be the target
            targetType = "Mutant";

            // Check range of ability
            // If range is AOE, no need to target a specific enemy
            if (ability.abilityRange.Equals("AOE"))
            {
                Debug.Log("AOE");
            }

            // If range is Single, player needs to choose target
            if(ability.abilityRange.Equals("Single"))
            {
                // Highlight possible targets


                // Give player a sign to choose target
                battleInfoManager.SetMiddleMessage("Choose a waste to attack");

                turnQueueManager.ShowTurnQueue(0);
                turnQueueManager.HideTurnQueue(1);

                if (battleInfoManager.GetMiddleMessageState())
                    battleInfoManager.HideMiddleMessage(0);
                else if (!battleInfoManager.GetMiddleMessageState())
                    battleInfoManager.ShowMiddleMessage(1);

                canSelectTarget = true;
            }
        } 

        if (ability.abilityType.Equals("Defensive"))
        {
            // Set what type of character should be the target
            targetType = "Scavenger";

            // Check range of ability
            // If range is AOE, no need to target a specific enemy
            if (ability.abilityRange.Equals("AOE"))
            {
                Debug.Log("AOE");
            }

            // If range is Single, player needs to choose target
            if (ability.abilityRange.Equals("Single"))
            {
                // Highlight possible targets


                // Give player a sign to choose target
                battleInfoManager.SetMiddleMessage("Select target co-scavenger");

                turnQueueManager.ShowTurnQueue(0);
                turnQueueManager.HideTurnQueue(1);

                if (battleInfoManager.GetMiddleMessageState())
                    battleInfoManager.HideMiddleMessage(0);
                else if (!battleInfoManager.GetMiddleMessageState())
                    battleInfoManager.ShowMiddleMessage(1);

                canSelectTarget = true;
            }
        }
        
    }

    // Players can only cancel target selection if they
    // haven't selected a target
    public void CancelTargetSelection()
    {
        canSelectTarget = false;

        battleInfoManager.SetMiddleMessage("Canceled Attack");
        battleInfoManager.HideMiddleMessage(1);

        turnQueueManager.HideTurnQueue(0);
        turnQueueManager.ShowTurnQueue(1);
    }

    void Update()
    {
        // Can only select a target when its possible
        if (canSelectTarget)
        {
            // Monitors player touch input
            if (Input.touchCount > 0)
            {
                // To prevent selection of target more than once in one click, 
                // only call functions once touch has began
                if (TouchPhase.Began == Input.GetTouch(0).phase)
                {
                    // Always get the first touch, convert to world point, and make a Vector2 variable
                    // Create a RaycastHit2D to capture objects on touch
                    Vector2 origin = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero);

                    // Checks if Raycast hit something and has a value
                    if (hit.collider != null)
                    {
                        GameObject selectedTarget = hit.transform.gameObject;
                        if (selectedTarget.GetComponent<CharacterMonitor>().CheckCharacterType(targetType))
                        {
                            battleInfoManager.HideMiddleMessage(1);

                            turnQueueManager.HideTurnQueue(0);
                            turnQueueManager.ShowTurnQueue(1);

                            StartCoroutine(abilityManager.ExecuteAbility(selectedTarget, targetType));

                            canSelectTarget = false;
                        }
                    }
                }
            }
        }
    }
}
