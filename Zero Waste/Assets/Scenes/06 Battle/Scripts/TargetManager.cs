using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    #region Battle Managers
    private CameraManager cameraManager;
    private BattleInfoManager battleInfoManager;
    private TurnQueueManager turnQueueManager;
    private CharacterManager characterManager;
    private AttackController attackController;
    private StatusManager statusManager;
    private AudioManager audioManager;
    #endregion

    private PlayerAbilityManager abilityManager;
    private string targetType;

    private bool canSelectTarget = false;

    void Start()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        battleInfoManager = FindObjectOfType<BattleInfoManager>();
        turnQueueManager = FindObjectOfType<TurnQueueManager>();
        characterManager = FindObjectOfType<CharacterManager>();
        attackController = FindObjectOfType<AttackController>();
        statusManager = FindObjectOfType<StatusManager>();
        audioManager = FindObjectOfType<AudioManager>();
    }
   
    // <summary>
    // Deciphers ability to know if player needs to choose target or not
    // </summary>
    public void SelectTarget(PlayerAbility ability, PlayerAbilityManager abilityManager)
    {
        // Since abilities have their own managers, always need to
        // set the ability manager everytime player attacks
        this.abilityManager = abilityManager;

        // Determine which set of characters are possible targets base on range and ability type
        if (ability.type.Equals("Offensive"))
        {
            // Set what type of character should be the target
            targetType = "Mutant";

            // Check range of ability
            // If range is AOE, no need to target a specific mutant
            if (ability.range.Equals("AOE"))
            {
                PrepareForAbilityExecution(null);
            }
            // If range is Single, player needs to choose target
            else if(ability.range.Equals("Single"))
            {
                // Disable player's ability to use booster
                attackController.EnableAttackButton(0, 4);

                // Focus camera on possible targets
                cameraManager.FocusOnMutants(true);

                // Give player a sign that he is now allowed to choose a target
                battleInfoManager.SetMiddleMessage("Choose a waste to attack");
                turnQueueManager.ShowTurnQueue(0);
                turnQueueManager.HideTurnQueue(1);

                if (battleInfoManager.GetMiddleMessageState())
                    battleInfoManager.HideMiddleMessage(0);
                else if (!battleInfoManager.GetMiddleMessageState())
                    battleInfoManager.ShowMiddleMessage(1);

                // Set flag to true to allow target selection
                canSelectTarget = true;
                statusManager.HideScavengerStatusSection();
            }
        }  
        else if (ability.type.Equals("Defensive"))
        {
            // Set what type of character should be the target
            targetType = "Scavenger";

            // Check range of ability
            // If range is AOE, no need to target a specific enemy
            if (ability.range.Equals("AOE"))
            {
                PrepareForAbilityExecution(null);
            } 
            else if (ability.range.Equals("Self"))
            {
                PrepareForAbilityExecution(abilityManager.ScavengerPrefab);
            }
            // If range is Single, player needs to choose target
            else if (ability.range.Equals("Single"))
            {
                // Disable player's ability to use booster
                attackController.EnableAttackButton(0, 4);

                // Focus camera on possible targets
                cameraManager.FocusOnScavengers(true);

                // Give player a sign that he is now allowed to choose a target
                battleInfoManager.SetMiddleMessage("Choose a colleague to apply effect to");
                turnQueueManager.ShowTurnQueue(0);
                turnQueueManager.HideTurnQueue(1);

                if (battleInfoManager.GetMiddleMessageState())
                    battleInfoManager.HideMiddleMessage(0);
                else if (!battleInfoManager.GetMiddleMessageState())
                    battleInfoManager.ShowMiddleMessage(1);

                // Set flag to true to allow target selection
                canSelectTarget = true;
                statusManager.HideScavengerStatusSection();
            }
        }
    }

    // <summary>
    // Players can only cancel target selection if they haven't selected a target
    // </summary>
    public void CancelTargetSelection(bool switchedAbility)
    {
        // Enable use of booster
        attackController.EnableAttackButton(1, 4);

        // Prevent players from being able to select a target
        canSelectTarget = false;
        statusManager.ShowScavengerStatusSection();

        // Unfocus camera
        cameraManager.FocusOnScavengers(false);
        cameraManager.FocusOnMutants(false);

        // If ability is switched, no need to show cancel attack message
        if (!switchedAbility)
            battleInfoManager.SetMiddleMessage("Canceled Attack");

        // Hide message and show turn queue
        battleInfoManager.HideMiddleMessage(1);
        turnQueueManager.HideTurnQueue(0);
        turnQueueManager.ShowTurnQueue(1);
    }

    // <summary>
    // Everything that happens after a target is selected and before ability
    // is done such as checking if target selected is applicable
    // </summary>
    void PrepareForAbilityExecution(GameObject targetObject)
    {
        bool characterTypeMatch = false;

        if (abilityManager.Ability.range.Equals("AOE")) 
        {
            characterTypeMatch = true;
        } 
        else
        {
            characterTypeMatch = targetObject.
            GetComponent<CharacterMonitor>().CheckCharacterType(targetType);
        }

        // Make sure that the target chosen by player matches where
        // ability can be applied, if its offensive or defensive
        if (characterTypeMatch)
        {
            // After user has chosen a target, disable target selection
            canSelectTarget = false;
            statusManager.ShowScavengerStatusSection();
            
            // Hide message and show turn queue
            battleInfoManager.HideMiddleMessage(1);
            turnQueueManager.HideTurnQueue(0);
            turnQueueManager.ShowTurnQueue(1);

            // Execute ability by calling the function at it's own manager
            StartCoroutine(abilityManager.ExecuteAbility(targetObject, targetType));
        }
        // Just in case, ask player to choose again
        // Though this might not happen as camera is focused to possible targets
        else
        {

        }
    }

    // <summary>
    // Selection of target happens through Raycast because characters are outside of canvas
    // </summary>
    void Update()
    {
        // Can only select a target when ability chosen requires it
        if (canSelectTarget)
        {
            // Just one touch input needed to select a target
            if (Input.touchCount == 1)
            {
                // To prevent selection of target more than once in one click, 
                // only call functions once touch has began
                if (TouchPhase.Began == Input.GetTouch(0).phase)
                {
                    // Always get the first touch, convert to world point, and make a Vector2 variable
                    // Create a RaycastHit2D to capture objects on touch
                    Vector2 origin = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero);

                    // Checks if RaycastHit something
                    // If touch hits a target, it will be sent to hit object so we can access it
                    if (hit.collider != null)
                    {
                        // Get object caught by RaycastHit and prepare for ability execution
                        GameObject targetObject = hit.transform.gameObject;

                        if (targetObject.GetComponent<CharacterMonitor>() != null)
                        {
                            PrepareForAbilityExecution(targetObject);
                            audioManager.PlaySound("Click 02");
                        }
                        else
                        {
                            audioManager.PlaySound("Beep Denied");
                        }
                    }
                    else
                    {
                        audioManager.PlaySound("Beep Denied");
                    }
                }
            }
        }
    }
}
