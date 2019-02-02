using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {

    [Header("Managers")]
    public BattleInfoManager battleInfoManager;
    public EnvironmentManager environmentManager;
    public StatusManager statusManager;
    public TurnQueueManager turnQueueManager;
    public CharacterManager characterManager;

    private string battleNo;
    private string nodeName;
    private Dictionary<string, Character[]> characters;

    [Space]
    [SerializeField] private Player[] scavengers;
    [SerializeField] private Enemy[] mutants;

    [Space]
    [SerializeField] private Sprite backgroundSprite;

    private List<Character> characterQueue;

    private int turnCount;
    private bool firstLoop;
    private bool loopDone;
    
    //
    // Called from the moment the scene loaded.
    //
    void Start()
    {
        GetBattleData();
        SetupBattle();
    }

    //
    // Gets data from previous scene, which is the Map Screen,
    // that has the Team Select function.
    // 
    void GetBattleData()
    {
        // TEMPORARY
        battleNo = "Battle 1";
        nodeName = "Misty Forest";

        characters = characterManager.CloneCharacters(scavengers, mutants);
        scavengers = characterManager.InitializeScavengers(characters["Scavenger"] as Player[]);
        mutants = characterManager.InitializeMutants(characters["Mutants"] as Enemy[]);
    }

    // 
    // Sets up the Battle Environment by changing info,
    // background, sprites, stats, etc.
    // 
    void SetupBattle()
    {
        firstLoop = true; loopDone = true;
        turnCount = 1;
        
        environmentManager.ChangeBackground(backgroundSprite);
        characterManager.ChangeSprite(scavengers, mutants);
        battleInfoManager.SetBattleDetails(battleNo, nodeName);

        statusManager.SetCharacterCount(scavengers.Length, mutants.Length);
        statusManager.SetCharactersStatistics(scavengers, mutants);
        statusManager.SetScavengerDetails(scavengers);
        
        ProcessTurn();
    }

    void ProcessTurn()
    {
        turnQueueManager.CalculateTurn(scavengers, mutants);
        characterQueue = turnQueueManager.GetCharacterQueue();

        StartCoroutine(BattleLoop());
    }

    IEnumerator RenderVisuals()
    {
        battleInfoManager.ShowStartAnimation(1);
        yield return new WaitForSeconds(1f);
        battleInfoManager.ShowStartAnimation(0);

        StartCoroutine(characterManager.ScavengersEntrance());
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(characterManager.MutantEntrance());
        yield return new WaitForSeconds(2.5f);

        battleInfoManager.DisplayBattleDetails(1);
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(statusManager.DisplayScavengerDetails());

        firstLoop = false;
        yield return null;
    }

    IEnumerator BattleLoop()
    {
        if (firstLoop)
        {
            StartCoroutine(RenderVisuals());
            yield return new WaitForSeconds(6.5f);
        }

        if(characterQueue.Count > 0)
        {
            if (loopDone)
            {
                // Turn Queue
                battleInfoManager.SetMiddleMessage("Processing Turn");
                battleInfoManager.DisplayMiddleMessage(1);
                yield return new WaitForSeconds(1f);
                battleInfoManager.DisplayMiddleMessage(0);

                StartCoroutine(turnQueueManager.DisplayTurnQueue());
                yield return new WaitForSeconds(3f);

                loopDone = false;
            }

            if (characterQueue[turnCount - 1] is Player)
            {

            }

            if (characterQueue[turnCount - 1] is Enemy)
            {

            }
        }
        else
        {
            loopDone = true;
            firstLoop = true;
            
            ProcessTurn();
        }

        yield return null;
    }

    /*IEnumerator SetUpBattle()
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
    }*/
}
