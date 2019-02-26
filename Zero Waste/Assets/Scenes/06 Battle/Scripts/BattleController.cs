using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {

    [Header("Managers")]
    public DataController dataController;
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
    [SerializeField] private Player[] scavengerTeamArray;
    [SerializeField] private Enemy[] mutantTeamArray;
    private List<Player> scavengerTeam;
    private List<Enemy> mutantTeam;

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
        dataController = GameObject.FindObjectOfType<DataController>();

        GetBattleData();
        SetupBattle();
    }

    //
    // Gets data from previous scene, which is the Map Screen,
    // that has the Team Select function.
    // 
    void GetBattleData()
    {
        if (dataController != null)
        {
            scavengerTeam = new List<Player>();
            foreach (Player scavenger in dataController.scavengerTeam)
            {
                if (scavenger != null)
                    scavengerTeam.Add(scavenger);
            }

            mutantTeam = new List<Enemy>();
            foreach (Enemy mutant in dataController.wasteTeam)
            {
                if (mutant != null)
                    mutantTeam.Add(mutant);
            }

            scavengerTeamArray = new Player[scavengerTeam.Count];
            scavengerTeamArray = scavengerTeam.ToArray();
            scavengerTeamArray = characterManager.InstantiateCharacterData(scavengerTeamArray);
            scavengerTeamArray = characterManager.InitializeScavengers(scavengerTeamArray);

            mutantTeamArray = new Enemy[mutantTeam.Count];
            mutantTeamArray = mutantTeam.ToArray();
            mutantTeamArray = characterManager.InstantiateCharacterData(mutantTeamArray);
            mutantTeamArray = characterManager.InitializeMutants(mutantTeamArray);

            battleNo = "Battle " + (dataController.currentBattle.battleId + 1);
            nodeName = dataController.currentBattle.battleName;
        }
    }

    // 
    // Sets up the Battle Environment by changing info,
    // background, sprites, stats, etc.
    // 
    void SetupBattle()
    {
        if (dataController != null)
        {
            turnCount = 0;
            firstLoop = true;
            loopDone = true;

            statusManager.SetCharacterCount(scavengerTeamArray.Length, mutantTeamArray.Length);

            environmentManager.SetBackground(backgroundSprite);

            characterManager.InstantiateCharacterPrefab(scavengerTeamArray);
            characterManager.InstantiateCharacterPrefab(mutantTeamArray);

            battleInfoManager.SetBattleDetails(battleNo, nodeName);

            statusManager.SetCharactersStatistics(scavengerTeamArray, mutantTeamArray);
            statusManager.SetScavengerDetails(scavengerTeamArray);

            ProcessTurn();
        } 
    }

    void ProcessTurn()
    {
        turnQueueManager.CalculateTurn(scavengerTeamArray, mutantTeamArray);

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
