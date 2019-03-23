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
    private List<Player> scavengerTeam;
    private List<Enemy> mutantTeam;

    [Space]
    [SerializeField] private Sprite backgroundSprite;

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
            battleEnd = false;
            endOfLoop = false;

            environmentManager.SetBackground(backgroundSprite);

            characterManager.InstantiateCharacterPrefab(scavengerTeamArray);
            characterManager.InstantiateCharacterPrefab(mutantTeamArray);

            battleInfoManager.SetBattleDetails(battleNo, nodeName);

            statusManager.SetScavengerDetails(scavengerTeamArray);
            statusManager.SetMutantDetails(mutantTeamArray);

            StartCoroutine(BattleLoop());
        } 
    }

    void ProcessTurn()
    {
        turnQueueManager.CalculateTurn(scavengerTeamArray, mutantTeamArray);

        characterQueue = new List<Character>();
        characterQueue = turnQueueManager.GetCharacterQueue();
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

        if(turnCount < (dataController.scavengerCount + dataController.wasteCount))
        {
            if (loopDone)
            {
                ProcessTurn();
                turnQueueManager.ShowPointer(0);

                battleInfoManager.SetMiddleMessage("Processing Turn");
                battleInfoManager.DisplayMiddleMessage(1);
                yield return new WaitForSeconds(1f);

                StartCoroutine(turnQueueManager.DisplayTurnQueue(1));

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

                    GameObject mutantObject = characterManager.GetMutantPrefab(characterQueue[turnCount] as Enemy);

                    if (mutantObject != null)
                    {
                        mutantObject.GetComponent<CharacterMonitor>().CharacterBattleStance();
                        yield return new WaitForSeconds(1f);
                    }

                    attackController.EnableAttackButtons(0);
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

            StartCoroutine(BattleLoop());
        }

        yield return null;
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

    IEnumerator DisplayBattleResult()
    {
        yield return new WaitForSeconds(3f);

        // Stop battle from looping
        StopAllCoroutines();

        turnQueueManager.ShowTurnQueue(0);
        turnQueueManager.HideTurnQueue(1);
        statusManager.HideMutantStatusSection();
        statusManager.HideScavengerStatusSection();
        attackController.HideAttackButtons();

        // Show battle results
        battleInfoManager.DisplayBattleResult();
    }

    void Update()
    {
        // Always monitor if group of characters are alive or not
        // instead of checking every end of loop
        // Here all mutants are dead
        if (!characterManager.AllMutantsAlive)
        {
            battleEnd = true;
            StartCoroutine(DisplayBattleResult());
        }
        // All scavengers are dead
        else if (!characterManager.AllScavengersAlive)
        {
            battleEnd = true;
            StartCoroutine(DisplayBattleResult());
        }
        // Neither are dead so check dot effects
        else 
        {
            if (endOfLoop)
            {
                endOfLoop = false;
                foreach (GameObject scavengerObj in characterManager.GetAllCharacterPrefabs(1))
                {
                    if (scavengerObj.GetComponent<CharacterMonitor>().IsAlive)
                        StartCoroutine(scavengerObj.GetComponent<CharacterMonitor>().UpdateScavengerEffects());
                }
                return;
            }
        }

    }
}
