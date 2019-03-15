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

    [Space]
    public float[] scavengerEntranceDelay;
    public float[] mutantEntranceDelay;
    public float[] renderDelay;

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
    
    void Start()
    {
        // Get reference to data controller
        dataController = GameObject.FindObjectOfType<DataController>();
        // MarkAsPlayed();

        // Get all details needed for battle and set it up
        GetBattleData();
        BattleSetup();
    }

    void MarkAsPlayed()
    {
        if (dataController != null)
        {
            dataController.currentSaveData.PlayBattle(dataController.currentBattle.battleId);
            dataController.SaveSaveData();
            dataController.SaveGameData();
        }
    }

    // Gets data from previous scene, which is the Map Screen,
    // that has the Team Select function.
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

    // Sets up the Battle Environment by changing info,
    // background, sprites, stats, etc.
    void BattleSetup()
    {
        if (dataController != null)
        {
            turnCount = 0;
            firstLoop = true;
            loopDone = true;

            environmentManager.SetBackground(backgroundSprite);

            characterManager.InstantiateCharacterPrefab(scavengerTeamArray);
            characterManager.InstantiateCharacterPrefab(mutantTeamArray);

            battleInfoManager.SetBattleDetails(battleNo, nodeName);

            statusManager.SetScavengerDetails(scavengerTeamArray);
            statusManager.SetMutantDetails(mutantTeamArray);

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

    IEnumerator RenderBattleComponents()
    {
        battleInfoManager.ShowStartAnimation(1);
        yield return new WaitForSeconds(1f);
        battleInfoManager.ShowStartAnimation(0);

        StartCoroutine(characterManager.ScavengersEntrance());
        yield return new WaitForSeconds(scavengerEntranceDelay[dataController.scavengerCount - 1]);
        StartCoroutine(characterManager.MutantEntrance());
        yield return new WaitForSeconds(mutantEntranceDelay[dataController.wasteCount - 1]);

        battleInfoManager.DisplayBattleDetails(1);
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(statusManager.DisplayScavengerStatusSection(scavengerTeamArray));
        StartCoroutine(statusManager.DisplayMutantStatusSection(mutantTeamArray));
        yield return new WaitForSeconds(3f);

        attackController.DisplayAttackButtons(1);
        // attackController.EnableAttackButtons(0);

        firstLoop = false;
        yield return null;
    }

    // <summary>
    // </summary>
    public IEnumerator BattleLoop()
    {
        if (firstLoop)
        {
            StartCoroutine(RenderBattleComponents());
            yield return new WaitForSeconds(renderDelay[dataController.scavengerCount - 1]);
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
                    yield return new WaitForSeconds(2f);

                loopDone = false;
            }

            // See if character is still alive to proceed or skip
            bool continueLoop = false;
            if (characterQueue[turnCount] is Player)
            {
                GameObject scavengerPrefab = characterManager.
                    GetScavengerPrefab(characterQueue[turnCount] as Player);
                continueLoop = scavengerPrefab.GetComponent<CharacterMonitor>().IsAlive;
            }
            else
            {
                GameObject mutantPrefab = characterManager.
                    GetMutantPrefab(characterQueue[turnCount] as Enemy);
                continueLoop = mutantPrefab.GetComponent<CharacterMonitor>().IsAlive;
            }
            Debug.Log(continueLoop);

            if (continueLoop)
            {
                battleInfoManager.SetCurrentTurn(characterQueue[turnCount].characterThumb,
                characterQueue[turnCount].characterName);

                battleInfoManager.DisplayNextTurnPanel(1);
                yield return new WaitForSeconds(0.5f);
                battleInfoManager.DisplayNextTurnSign(1);
                yield return new WaitForSeconds(1f);

                battleInfoManager.DisplayNextTurnSign(0);
                battleInfoManager.DisplayNextTurnPanel(0);

                turnQueueManager.ShowPointer(1);
                turnQueueManager.PointCurrentCharacter(turnCount);

                if (characterQueue[turnCount] is Player)
                {
                    Debug.Log("Player's Turn");

                    // Get scavenger prefab
                    GameObject scavengerPrefab = characterManager.
                        GetScavengerPrefab(characterQueue[turnCount] as Player);

                    // If not null, trigger its fight instance
                    if (scavengerPrefab != null)
                    {
                        scavengerPrefab.GetComponent<CharacterMonitor>().CharacterBattleStance();
                    }

                    turnQueueManager.SetCurrentCharacter(turnCount);
                    attackController.ScavengerAttackSetup();
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
                turnCount++;
                StartCoroutine(BattleLoop());
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

    // <summer>
    // </summer>
    public void NextTurn()
    {
        StopAllCoroutines();

        turnCount++;
        StartCoroutine(BattleLoop());
    }
}
