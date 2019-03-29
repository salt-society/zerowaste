using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {

    private DataController dataController;
    private AudioManager audioManager;
    private BattleInfoManager battleInfoManager;
    private EnvironmentManager environmentManager;
    private StatusManager statusManager;
    private TurnQueueManager turnQueueManager;
    private CharacterManager characterManager;
    private ItemManager itemManager;
    private AttackController attackController;
    private EnemyAbilityManager enemyAbilityManager;
    private CameraManager cameraManager;

    [Space]
    public float[] scavengerEntranceDelay;
    public float[] mutantEntranceDelay;
    public float[] renderDelay;

    private string battleNo;
    private string nodeName;

    [Space]
    [SerializeField] private Player[] scavengerTeamArray;
    [SerializeField] private Enemy[] mutantTeamArray;
    private List<Player> scavengerTeamList;
    private List<Enemy> mutantTeamList;

    private List<Character> characterQueue;

    private int turnCount;
    private bool firstLoop;
    private bool loopDone;
    private bool endOfLoop;
    private bool battleEnd;
    
    void Start()
    {
        // Get references to battle managers
        dataController = FindObjectOfType<DataController>();
        audioManager = FindObjectOfType<AudioManager>();
        battleInfoManager = FindObjectOfType<BattleInfoManager>();
        environmentManager = FindObjectOfType<EnvironmentManager>();
        statusManager = FindObjectOfType<StatusManager>();
        turnQueueManager = FindObjectOfType<TurnQueueManager>();
        characterManager = FindObjectOfType<CharacterManager>();
        itemManager = FindObjectOfType<ItemManager>();
        attackController = FindObjectOfType<AttackController>();
        cameraManager = FindObjectOfType<CameraManager>();
        enemyAbilityManager = FindObjectOfType<EnemyAbilityManager>();

        // MarkAsPlayed();

        // Get all details needed for battle and set it up
        GetBattleData();
        BattleSetup();
    }

    // <summary>
    // </summary>
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
            scavengerTeamList = new List<Player>();
            foreach (Player scavenger in dataController.scavengerTeam)
            {
                if (scavenger != null)
                    scavengerTeamList.Add(scavenger);
            }

            mutantTeamList = new List<Enemy>();
            foreach (Enemy mutant in dataController.wasteTeam)
            {
                if (mutant != null)
                    mutantTeamList.Add(mutant);
            }

            scavengerTeamArray = new Player[scavengerTeamList.Count];
            scavengerTeamArray = scavengerTeamList.ToArray();
            scavengerTeamArray = characterManager.InstantiateCharacterData(scavengerTeamArray);
            scavengerTeamArray = characterManager.InitializeScavengers(scavengerTeamArray);

            mutantTeamArray = new Enemy[mutantTeamList.Count];
            mutantTeamArray = mutantTeamList.ToArray();
            mutantTeamArray = characterManager.InstantiateCharacterData(mutantTeamArray);
            mutantTeamArray = characterManager.InitializeMutants(mutantTeamArray);
        }
    }

    void BattleSetup()
    {
        if (dataController != null)
        {
            // Set Flags to Default Values
            firstLoop = true;
            loopDone = true;
            battleEnd = false;
            endOfLoop = false;
            turnCount = 0;
            
            // Set Background
            environmentManager.SetBackground(dataController.currentBattle.background);

            // Set Character Objects
            characterManager.InstantiateCharacterPrefab(scavengerTeamArray);
            characterManager.InstantiateCharacterPrefab(mutantTeamArray);

            // Set Character Details
            statusManager.SetScavengerDetails(scavengerTeamArray);
            statusManager.SetMutantDetails(mutantTeamArray);

            // Start Battle Loop
            if (!dataController.currentBattle.isBossBattle)
            {
                StartCoroutine(BattleLoop());
            }
            else
            {

            }
            
        } 
    }

    void ProcessTurn()
    {
        turnQueueManager.CalculateTurn(scavengerTeamArray, mutantTeamArray);

        characterQueue = new List<Character>();
        characterQueue = turnQueueManager.GetCharacterQueue();

        Debug.Log("Processing Turns");
    }

    IEnumerator RenderBattleComponents()
    {
        battleInfoManager.ShowStartAnimation(1);
        yield return new WaitForSeconds(1f);
        battleInfoManager.ShowStartAnimation(0);

        StartCoroutine(characterManager.ScavengersEntrance());
        yield return new WaitForSeconds(scavengerEntranceDelay[dataController.scavengerCount - 1]);
        StartCoroutine(characterManager.MutantEntrance());
        yield return new WaitForSeconds(mutantEntranceDelay[dataController.mutantCount - 1]);

        StartCoroutine(statusManager.DisplayScavengerStatusSection(scavengerTeamArray));
        StartCoroutine(statusManager.DisplayMutantStatusSection(mutantTeamArray));
        yield return new WaitForSeconds(3f);

        firstLoop = false;
        yield return null;
    }

    // <summary>
    // </summary>
    public IEnumerator BattleLoop()
    {
        if (!battleEnd)
        {
            if (firstLoop)
            {
                StartCoroutine(RenderBattleComponents());
                yield return new WaitForSeconds(renderDelay[dataController.scavengerCount - 1]);
            }

            if (turnCount < (dataController.scavengerCount + dataController.mutantCount))
            {
                if (loopDone)
                {
                    ProcessTurn();
                    turnQueueManager.ShowPointer(0);

                    battleInfoManager.SetMiddleMessage("Processing Turn");
                    battleInfoManager.DisplayMiddleMessage(1);
                    battleInfoManager.HideMiddleMessage(0);
                    yield return new WaitForSeconds(1f);
                    battleInfoManager.HideMiddleMessage(1);

                    StartCoroutine(turnQueueManager.DisplayTurnQueue());

                    if (turnCount == 0)
                        yield return new WaitForSeconds(2f);

                    loopDone = false;
                }

                // See if character is still alive to decide if you should skip this turn
                bool continueLoop = IsCurrentCharacterAlive();
                if (continueLoop && !battleEnd)
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
                        

                        attackController.DisplayAttackButtons(1);
                        yield return new WaitForSeconds(0.5f);
                        attackController.ShowAttackButtons();

                        // Get scavenger prefab
                        GameObject scavengerPrefab = characterManager.
                            GetScavengerPrefab(characterQueue[turnCount] as Player);

                        // If not null, trigger its fight instance
                        if (scavengerPrefab != null)
                        {
                            scavengerPrefab.GetComponent<CharacterMonitor>().CharacterBattleStance();
                            itemManager.SetCurrentScavenger(scavengerPrefab.GetComponent<CharacterMonitor>().Position);
                        }

                        turnQueueManager.SetCurrentCharacter(turnCount);
                        attackController.ScavengerAttackSetup();
                    }
                    else
                    {
                        attackController.EnableAttackButtons(0);
                        attackController.HideAttackButtons();

                        GameObject mutantObject = characterManager.GetMutantPrefab(characterQueue[turnCount] as Enemy);
                        if (mutantObject != null)
                        {
                            mutantObject.GetComponent<CharacterMonitor>().CharacterBattleStance();
                            yield return new WaitForSeconds(1f);
                        }

                        turnQueueManager.SetCurrentCharacter(turnCount);
                        enemyAbilityManager.SetupEnemyAttack(characterQueue[turnCount] as Enemy, mutantObject);
                    }
                }
                else
                {
                    turnCount++;
                    StopAllCoroutines();
                    StartCoroutine(BattleLoop());
                }

            }
            else
            {
                loopDone = true;
                endOfLoop = true;
                turnCount = 0;

                attackController.HideAttackButtons();
                turnQueueManager.ShowTurnQueue(0);
                turnQueueManager.HideTurnQueue(1);

                yield return new WaitForSeconds(1f);
                StartCoroutine(turnQueueManager.HideTurnQueue());

                for (int i = 0; i < 2; i++)
                {
                    foreach (GameObject characterObj in characterManager.GetAllCharacterPrefabs(i))
                    {
                        if (characterObj.GetComponent<CharacterMonitor>().IsAlive)
                            StartCoroutine(characterObj.GetComponent<CharacterMonitor>().UpdateEffects());
                    }

                    if (i == 1)
                    {
                        yield return new WaitForSeconds(1f);
                        StartCoroutine(BattleLoop());
                    }
                }
            }
        }
        
    }

    bool IsCurrentCharacterAlive()
    {
        if (characterQueue[turnCount] is Player)
        {
            GameObject scavengerPrefab = characterManager.
                GetScavengerPrefab(characterQueue[turnCount] as Player);
            return scavengerPrefab.GetComponent<CharacterMonitor>().IsAlive;
        }
        else
        {
            GameObject mutantPrefab = characterManager.
                GetMutantPrefab(characterQueue[turnCount] as Enemy);
            return mutantPrefab.GetComponent<CharacterMonitor>().IsAlive;
        }
    }

    // <summer>
    // </summer>
    public void NextTurn()
    {
        StopAllCoroutines();

        turnCount++;
        StartCoroutine(BattleLoop());
    }

    IEnumerator DisplayBattleResult(bool victory)
    {
        cameraManager.Shake(true, 1);
        yield return new WaitForSeconds(3f);
        cameraManager.Shake(true, 1);

        StopAllCoroutines();
        battleInfoManager.CalculateResult(victory);

        battleInfoManager.DisplayMiddleMessage(0);
        turnQueueManager.ShowTurnQueue(0);
        turnQueueManager.HideTurnQueue(1);
        statusManager.HideMutantStatusSection();
        statusManager.HideScavengerStatusSection();
        attackController.HideAttackButtons();
    }

    void Update()
    {
        // Always monitor if group of characters are alive or not
        // instead of checking every end of loop

        // Here all mutants are dead, which means victory
        if (!characterManager.AllMutantsAlive && !battleEnd)
        {
            battleEnd = true;
            StartCoroutine(DisplayBattleResult(true));
        }

        // All scavengers are dead, defeat
        if (!characterManager.AllScavengersAlive && !battleEnd)
        {
            battleEnd = true;
            StartCoroutine(DisplayBattleResult(false));
        }

    }
}
