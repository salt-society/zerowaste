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
    #endregion

    private AbilityManager abilityManager;
    private string targetType;
    private bool canSelectTarget = false;

    void Start()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        battleInfoManager = FindObjectOfType<BattleInfoManager>();
        turnQueueManager = FindObjectOfType<TurnQueueManager>();
        characterManager = FindObjectOfType<CharacterManager>();
        attackController = FindObjectOfType<AttackController>();
    }
   
    // <summary>
    // Deciphers ability to know if player needs to choose target or not
    // </summary>
    public void SelectTarget(PlayerAbility ability, AbilityManager abilityManager)
    {
        // Since abilities have their own managers, always need to
        // set the ability manager everytime player attacks
        this.abilityManager = abilityManager;

        // Determine which set of characters are possible targets base on range and ability type
        if (ability.abilityType.Equals("Offensive"))
        {
            // Set what type of character should be the target
            targetType = "Mutant";

            // Check range of ability
            // If range is AOE, no need to target a specific mutant
            if (ability.abilityRange.Equals("AOE"))
            {
                Debug.Log("AOE");
            }
            // If range is Single, player needs to choose target
            else if(ability.abilityRange.Equals("Single"))
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
            }
        }  
        else if (ability.abilityType.Equals("Defensive"))
        {
            // Set what type of character should be the target
            targetType = "Scavenger";

            // Check range of ability
            // If range is AOE, no need to target a specific enemy
            if (ability.abilityRange.Equals("AOE"))
            {
                Debug.Log("AOE");
            } 
            else if (ability.abilityRange.Equals("Self"))
            {
                Debug.Log("Range: Self");
                PrepareForAbilityExecution(abilityManager.ScavengerPrefab);
                Debug.Log(abilityManager.ScavengerPrefab.GetComponent<CharacterMonitor>().Scavenger.characterName);
            }
            // If range is Single, player needs to choose target
            else if (ability.abilityRange.Equals("Single"))
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
        Debug.Log("Target: " + targetType);
        bool characterTypeMatch = targetObject.
            GetComponent<CharacterMonitor>().CheckCharacterType(targetType);
        Debug.Log(targetObject.GetComponent<CharacterMonitor>().CharacterType);
        Debug.Log("Matched: " + characterTypeMatch);

        // Make sure that the target chosen by player matches where
        // ability can be applied, if its offensive or defensive
        if (characterTypeMatch)
        {
            Debug.Log("Character Matched.");
            // After user has chosen a target, disable target selection
            canSelectTarget = false;
            
            // Hide message and show turn queue
            battleInfoManager.HideMiddleMessage(1);
            turnQueueManager.HideTurnQueue(0);
            turnQueueManager.ShowTurnQueue(1);

            // Execute ability by calling the function at it's own manager
            StartCoroutine(abilityManager.ExecuteAbility(targetObject, targetType));
            Debug.Log("Target: " + targetObject.name);
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
                        PrepareForAbilityExecution(targetObject);
                    }
                }
            }
        }
    }
}
