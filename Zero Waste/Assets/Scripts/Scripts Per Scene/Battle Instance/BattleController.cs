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
    public ItemManager itemManager;
    public AttackController attackController;

    private string battleNo;
    private string nodeName;

    [Space]
    [SerializeField] private Player[] scavengers;
    [SerializeField] private Enemy[] mutants;
    private Player[] scavengerTeam;
    private Enemy[] mutantTeam;

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
        Player[] scavengerTemp = (Player[])scavengers.Clone();
        Enemy[] mutantTemp = (Enemy[])mutants.Clone();

        scavengerTeam = new Player[scavengerTemp.Length];
        scavengerTeam = characterManager.InstantiateCharacterData(scavengerTemp);

        mutantTeam = new Enemy[mutantTemp.Length];
        mutantTeam = characterManager.InstantiateCharacterData(mutantTemp);

        scavengerTeam = characterManager.InitializeScavengers(scavengerTeam);
        mutantTeam = characterManager.InitializeMutants(mutantTeam);

        battleNo = "Battle 1";
        nodeName = "Misty Forest";
    }

    // 
    // Sets up the Battle Environment by changing info,
    // background, sprites, stats, etc.
    // 
    void SetupBattle()
    {
        turnCount = 0;
        firstLoop = true; 
        loopDone = true;

        statusManager.SetCharacterCount(scavengerTeam.Length, mutantTeam.Length);
        
        environmentManager.SetBackground(backgroundSprite);

        characterManager.InstantiateCharacterPrefab(scavengerTeam);
        characterManager.InstantiateCharacterPrefab(mutantTeam);

        battleInfoManager.SetBattleDetails(battleNo, nodeName);

        statusManager.SetCharactersStatistics(scavengerTeam, mutantTeam);
        statusManager.SetScavengerDetails(scavengerTeam);
        
        ProcessTurn();
    }

    void ProcessTurn()
    {
        turnQueueManager.CalculateTurn(scavengerTeam, mutantTeam);

        characterQueue = new List<Character>();
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
        yield return new WaitForSeconds(.5f);

        statusManager.DisplayPollutionBars(1);
        // itemManager.DisplayTrashcanBox(1);
        attackController.DisplayAttackButtons(1);
        attackController.EnableAttackButtons(0);

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

        if(turnCount < 6)
        {
            if (loopDone)
            {
                turnQueueManager.ShowPointer(0);

                battleInfoManager.SetMiddleMessage("Processing Turn");
                battleInfoManager.DisplayMiddleMessage(1);
                yield return new WaitForSeconds(1f);

                StartCoroutine(turnQueueManager.DisplayTurnQueue(1));

                if (turnCount == 0)
                    yield return new WaitForSeconds(1.5f);

                loopDone = false;
            }

            battleInfoManager.SetCurrentTurn(characterQueue[turnCount].characterThumb, 
                characterQueue[turnCount].characterName);

            battleInfoManager.DisplayNextTurnPanel(1);
            yield return new WaitForSeconds(.5f);
            battleInfoManager.DisplayNextTurnSign(1);
            yield return new WaitForSeconds(1f);

            battleInfoManager.DisplayNextTurnSign(0);
            battleInfoManager.DisplayNextTurnPanel(0);

            turnQueueManager.ShowPointer(1);
            turnQueueManager.PointCurrentCharacter(turnCount);

            if (characterQueue[turnCount] is Player)
            {
                Debug.Log("Player's Turn");

                turnQueueManager.SetCurrentCharacter(turnCount);
                attackController.PlayerAttackSetup();

                //turnCount++;
                //yield return new WaitForSeconds(3f);

                //StartCoroutine(BattleLoop());
            } 
            else
            {
                Debug.Log("Enemy's Turn");

                attackController.EnableAttackButtons(0);
                turnQueueManager.SetCurrentCharacter(turnCount);

                //turnCount++;
                //yield return new WaitForSeconds(3f);

                //StartCoroutine(BattleLoop());
            }
        }
        else
        {
            // turnCount = 1;
            // loopDone = true;
            // StartCoroutine(turnQueueManager.DisplayTurnQueue(0));

            // ProcessTurn();
        }

        yield return null;
    }
}
