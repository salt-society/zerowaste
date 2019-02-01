using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {

	// Data from Map Scene
    private string battleNo;
    private string nodeName;
    [SerializeField] private Player[] players;
    [SerializeField] private Enemy[] enemies;

    // Data Here
    private int numberOfTurns;

    [Header("Managers")]
    public BattleInfoManager battleInfoManager;
    public StatManager statManager;
    public TurnQueueManager turnQueueManager;
    public CharacterManager characterManager;
    
    void Start()
    {
        // Get Battle Data from Data Controller
        GetBattleData();
        StartCoroutine(SetUpBattle());
    }

    void GetBattleData()
    {
        // Temp Values
        battleNo = "Battle 1";
        nodeName = "Into the Woods";
    }

    IEnumerator SetUpBattle()
    {
        // Battle Start
        battleInfoManager.DisplayStart(true);
        yield return new WaitForSeconds(1f);
        battleInfoManager.DisplayStart(false);

        // Character Sprites
        characterManager.ChangeSprite(players, enemies);
        StartCoroutine(characterManager.ScavengersEntrance());
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(characterManager.MutantEntrance());
        yield return new WaitForSeconds(2.5f);

        // Battle Info
        battleInfoManager.DisplayBattleDetails(battleNo, nodeName);
        yield return new WaitForSeconds(.5f);

        // Turn Queue
        battleInfoManager.DisplayTurnProcess(true);
        yield return new WaitForSeconds(1f);
        battleInfoManager.DisplayTurnProcess(false);

        turnQueueManager.CalculateTurn(players, enemies);
        StartCoroutine(turnQueueManager.DisplayTurnQueue());
        yield return new WaitForSeconds(3f);

        // Scavenger Status Panel

        // Booster Panel

        // Attack Panel

        yield return null;
    }
}
