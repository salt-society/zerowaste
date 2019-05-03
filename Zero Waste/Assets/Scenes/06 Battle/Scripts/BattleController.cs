using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private CooperCorner cooperCorner;

    [Space]
    public float[] scavengerEntranceDelay;
    public float[] mutantEntranceDelay;
    public float[] renderDelay;

    [Space]
    public GameObject fadeTransition;

    [Space]
    public GameObject cooperCornerObject;
    public GameObject tutorialSection;

    [Space]
    private Player[] scavengerTeam;
    private Enemy[] mutantTeam;

    private List<Character> characterQueue;

    private int turnCount;
    private int totalTurnCount;
    private bool firstLoop;
    private bool loopDone;
    private bool endOfLoop;
    private bool battleEnd;

    public GameObject[] demoMessage;

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
        

        PlayBGM();
        // MarkAsPlayed();
        CheckMode();
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

    void PlayBGM()
    {
        if (audioManager != null)
        {
            if (!string.IsNullOrEmpty(dataController.currentBattle.BGM))
                audioManager.PlaySound(dataController.currentBattle.BGM);
        }
    }

    void CheckMode()
    {
        if (dataController != null)
        {
            if (dataController.currentBattle.isTutorial)
            {
                CreateBattleData();
                BattleSetup();

                cooperCornerObject.SetActive(true);
                cooperCorner = FindObjectOfType<CooperCorner>();
            }
            else
            {
                GetBattleData();
                BattleSetup();
            }
        }
    }

    void CreateBattleData()
    {
        if (dataController != null)
        {
            List<Player> scavengerTemp = new List<Player>();
            List<Enemy> mutantTemp = new List<Enemy>();

            scavengerTemp.Add(dataController.scavengerRoster[0]);
            dataController.scavengerCount = scavengerTemp.Count;

            mutantTemp.Add(dataController.allWasteList[0]);
            dataController.mutantCount = mutantTemp.Count;

            mutantTemp[0].baseLevel = dataController.currentBattle.wastePool.baseLevel;
            mutantTemp[0].maxLevel = dataController.currentBattle.wastePool.maxLevel;

            scavengerTeam = new Player[scavengerTemp.Count];
            scavengerTeam = scavengerTemp.ToArray();
            scavengerTeam = characterManager.InstantiateCharacterData(scavengerTeam);
            scavengerTeam = characterManager.InitializeScavengers(scavengerTeam);

            mutantTeam = new Enemy[mutantTemp.Count];
            mutantTeam = mutantTemp.ToArray();
            mutantTeam = characterManager.InstantiateCharacterData(mutantTeam);

            if (dataController.currentBattle.isBossBattle)
                mutantTeam = characterManager.InitializeBoss(mutantTeam[0]);
            else
                mutantTeam = characterManager.InitializeMutants(mutantTeam);
        }
    }

    void GetBattleData()
    {
        if (dataController != null)
        {
            List<Player> scavengerTemp = new List<Player>();
            List<Enemy> mutantTemp = new List<Enemy>();

            foreach (Player scavenger in dataController.scavengerTeam)
            {
                if (scavenger != null)
                    scavengerTemp.Add(scavenger);
            }     

            foreach (Enemy mutant in dataController.wasteTeam)
            {
                if (mutant != null)
                    mutantTemp.Add(mutant);
            }

            dataController.scavengerCount = scavengerTemp.Count;
            dataController.mutantCount = mutantTemp.Count;

            scavengerTeam = new Player[scavengerTemp.Count];
            scavengerTeam = scavengerTemp.ToArray();
            scavengerTeam = characterManager.InstantiateCharacterData(scavengerTeam);
            scavengerTeam = characterManager.InitializeScavengers(scavengerTeam);

            mutantTeam = new Enemy[mutantTemp.Count];
            mutantTeam = mutantTemp.ToArray();
            mutantTeam = characterManager.InstantiateCharacterData(mutantTeam);

            if (dataController.currentBattle.isBossBattle)
                mutantTeam = characterManager.InitializeBoss(mutantTeam[0]);
            else
                mutantTeam = characterManager.InitializeMutants(mutantTeam);
        }
    }

    void SetDefaultFlags()
    {
        firstLoop = true;
        loopDone = true;
        battleEnd = false;
        endOfLoop = false;
        turnCount = 0;
    }

    void BattleSetup()
    {
        if (dataController != null)
        {
            SetDefaultFlags();

            environmentManager.SetBackground(dataController.currentBattle.background);
            statusManager.SetScavengerDetails(scavengerTeam);
            statusManager.SetMutantDetails(mutantTeam);
            characterManager.InstantiateCharacterPrefab(scavengerTeam);
            characterManager.InstantiateCharacterPrefab(mutantTeam);

            if (!dataController.currentBattle.isBossBattle)
            {
                if (dataController.currentBattle.isTutorial)
                {
                    StartCoroutine(BattleTutorialLoop());
                }
                else
                {
                    StartCoroutine(BattleLoop());
                }
            }
            else if (dataController.currentBattle.isBossBattle)
            {
                StartCoroutine(BossBattleLoop());
            }
        } 
    }

    void ProcessTurn()
    {
        turnQueueManager.CalculateTurn(scavengerTeam, mutantTeam);

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
        yield return new WaitForSeconds(mutantEntranceDelay[dataController.mutantCount - 1]);

        StartCoroutine(statusManager.DisplayScavengerStatusSection(scavengerTeam));
        StartCoroutine(statusManager.DisplayMutantStatusSection(mutantTeam));
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
                Debug.Log(dataController.scavengerCount);
                yield return new WaitForSeconds(renderDelay[dataController.scavengerCount - 1]);

                firstLoop = false;
            }

            if (turnCount < (scavengerTeam.Length + mutantTeam.Length))
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
                        enemyAbilityManager.SetupEnemyAttack(mutantObject.GetComponent<CharacterMonitor>().Mutant, mutantObject);
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
                    int j = 0;
                    foreach (GameObject characterObj in characterManager.GetAllCharacterPrefabs(i))
                    {
                        if (characterObj != null)
                        {
                            if (characterObj.GetComponent<CharacterMonitor>().IsAlive)
                                StartCoroutine(characterObj.GetComponent<CharacterMonitor>().UpdateEffects());

                            if (i == 0)
                            {
                                mutantTeam[j] = null;
                                mutantTeam[j] = characterObj.GetComponent<CharacterMonitor>().Mutant;
                                j++;
                            }

                            if (i == 1)
                            {
                                scavengerTeam[j] = null;
                                scavengerTeam[j] = characterObj.GetComponent<CharacterMonitor>().Scavenger;
                                j++;
                            }
                        }
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

    public IEnumerator BattleTutorialLoop()
    {
        if (!battleEnd)
        {
            if (firstLoop)
            {
                yield return new WaitForSeconds(3f);

                cameraManager.Shake(true, 2);
                audioManager.PlaySound("Monster Scream");
                StartCoroutine(characterManager.ScavengersEntrance());
                yield return new WaitForSeconds(scavengerEntranceDelay[dataController.scavengerCount - 1]);
                StartCoroutine(characterManager.MutantEntrance());
                yield return new WaitForSeconds(mutantEntranceDelay[dataController.scavengerCount - 1]);

                tutorialSection.SetActive(true);
                cameraManager.Shake(false, 2);

                cooperCorner.PlayTutorial(0);
                yield return new WaitUntil(cooperCorner.CanProceed);
                cooperCorner.PlayTutorial(1);
                yield return new WaitUntil(cooperCorner.CanProceed);
                cooperCorner.PlayTutorial(2);
                yield return new WaitUntil(cooperCorner.CanProceed);
                cooperCorner.PlayTutorial(3);
                yield return new WaitUntil(cooperCorner.CanProceed);
                cooperCorner.PlayTutorial(4);
                yield return new WaitUntil(cooperCorner.CanProceed);
                cooperCorner.PlayTutorial(5);
                yield return new WaitUntil(cooperCorner.CanProceed);

                StartCoroutine(statusManager.DisplayScavengerStatusSection(scavengerTeam));
                cooperCorner.PlayTutorial(6);
                yield return new WaitUntil(cooperCorner.CanProceed);

                tutorialSection.SetActive(false);

                cooperCorner.PlayTutorial(7);
                yield return new WaitUntil(cooperCorner.CanProceed);

                tutorialSection.SetActive(true);

                StartCoroutine(statusManager.DisplayMutantStatusSection(mutantTeam));
                cooperCorner.PlayTutorial(8);
                yield return new WaitUntil(cooperCorner.CanProceed);

                firstLoop = false;
            }

            if (turnCount < (scavengerTeam.Length + mutantTeam.Length))
            {
                if (loopDone)
                {
                    if (totalTurnCount == 0)
                        cooperCorner.PlayTutorial(9);

                    ProcessTurn();
                    turnQueueManager.ShowPointer(0);

                    battleInfoManager.SetMiddleMessage("Processing Turn");
                    battleInfoManager.DisplayMiddleMessage(1);
                    battleInfoManager.HideMiddleMessage(0);
                    yield return new WaitForSeconds(1f);
                    battleInfoManager.HideMiddleMessage(1);

                    StartCoroutine(turnQueueManager.DisplayTurnQueue());

                    if (totalTurnCount == 0)
                        yield return new WaitUntil(cooperCorner.CanProceed);
                    
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

                    if (totalTurnCount == 0)
                    {
                        cooperCorner.PlayTutorial(10);
                    }

                    /*battleInfoManager.DisplayNextTurnPanel(1);
                    yield return new WaitForSeconds(0.5f);
                    battleInfoManager.DisplayNextTurnSign(1);
                    yield return new WaitForSeconds(1f);

                    battleInfoManager.DisplayNextTurnSign(0);
                    battleInfoManager.DisplayNextTurnPanel(0);*/

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

                        if (totalTurnCount == 0)
                        {
                            yield return new WaitUntil(cooperCorner.CanProceed);

                            cooperCorner.PlayTutorial(11);
                            yield return new WaitUntil(cooperCorner.CanProceed);
                            cooperCorner.PlayTutorial(12);
                            yield return new WaitUntil(cooperCorner.CanProceed);
                            cooperCorner.PlayTutorial(13);
                            yield return new WaitUntil(cooperCorner.CanProceed);
                            cooperCorner.PlayTutorial(14);
                            yield return new WaitUntil(cooperCorner.CanProceed);
                            cooperCorner.PlayTutorial(15);
                            yield return new WaitUntil(cooperCorner.CanProceed);

                            cooperCorner.PlayTutorial(16);
                            cooperCornerObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.6f);
                            yield return new WaitUntil(cooperCorner.CanProceed);

                            tutorialSection.SetActive(false);
                            cooperCornerObject.SetActive(false);
                        }

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
                        enemyAbilityManager.SetupEnemyAttack(mutantObject.GetComponent<CharacterMonitor>().Mutant, mutantObject);
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
                    int j = 0;
                    foreach (GameObject characterObj in characterManager.GetAllCharacterPrefabs(i))
                    {
                        if (characterObj != null)
                        {
                            if (characterObj.GetComponent<CharacterMonitor>().IsAlive)
                                StartCoroutine(characterObj.GetComponent<CharacterMonitor>().UpdateEffects());

                            if (i == 0)
                            {
                                mutantTeam[j] = null;
                                mutantTeam[j] = characterObj.GetComponent<CharacterMonitor>().Mutant;
                                j++;
                            }

                            if (i == 1)
                            {
                                scavengerTeam[j] = null;
                                scavengerTeam[j] = characterObj.GetComponent<CharacterMonitor>().Scavenger;
                                j++;
                            }
                        }
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

    IEnumerator RenderBossBattleComponents()
    {
        StartCoroutine(battleInfoManager.ShowBossAnimation(1));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(battleInfoManager.ShowBossAnimation(0));

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(characterManager.ScavengersEntrance());
        yield return new WaitForSeconds(scavengerEntranceDelay[dataController.scavengerCount - 1]);

        cameraManager.Shake(true, 2);
        StartCoroutine(characterManager.MutantEntrance());
        yield return new WaitForSeconds(mutantEntranceDelay[dataController.mutantCount - 1]);
        yield return new WaitForSeconds(1f);
        cameraManager.Shake(false, 2);

        StartCoroutine(statusManager.DisplayScavengerStatusSection(scavengerTeam));
        StartCoroutine(statusManager.DisplayMutantStatusSection(mutantTeam));
        yield return new WaitForSeconds(3f);

        firstLoop = false;
        yield return null;
    }

    IEnumerator BossBattleLoop()
    {
        if (!battleEnd)
        {
            if (firstLoop)
            {
                StartCoroutine(RenderBossBattleComponents());
                yield return new WaitForSeconds(renderDelay[dataController.scavengerCount - 1]);

                firstLoop = false;
            }

            if (turnCount < (scavengerTeam.Length + mutantTeam.Length))
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

                    /*battleInfoManager.DisplayNextTurnPanel(1);
                    yield return new WaitForSeconds(0.5f);
                    battleInfoManager.DisplayNextTurnSign(1);
                    yield return new WaitForSeconds(1f);

                    battleInfoManager.DisplayNextTurnSign(0);
                    battleInfoManager.DisplayNextTurnPanel(0);*/

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
                        enemyAbilityManager.SetupEnemyAttack(mutantObject.GetComponent<CharacterMonitor>().Mutant, mutantObject);
                    }
                }
                else
                {
                    turnCount++;
                    StopAllCoroutines();
                    StartCoroutine(BossBattleLoop());
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

                float delay = 1f;
                for (int i = 0; i < 2; i++)
                {
                    foreach (GameObject characterObj in characterManager.GetAllCharacterPrefabs(i))
                    {
                        if (characterObj != null)
                        {
                            if (characterObj.GetComponent<CharacterMonitor>().IsAlive)
                            {
                                StartCoroutine(characterObj.GetComponent<CharacterMonitor>().UpdateEffects());

                                if (i == 0)
                                    delay += characterObj.GetComponent<CharacterMonitor>().SwitchLength;
                            }
                                

                            if (i == 0)
                            {
                                int position = characterObj.GetComponent<CharacterMonitor>().Position;
                                mutantTeam[position] = null;
                                mutantTeam[position] = characterObj.GetComponent<CharacterMonitor>().Mutant;

                            }

                            if (i == 1)
                            {
                                int position = characterObj.GetComponent<CharacterMonitor>().Position;
                                scavengerTeam[position] = null;
                                scavengerTeam[position] = characterObj.GetComponent<CharacterMonitor>().Scavenger;
                            }
                        }
                    }

                    if (i == 1)
                    {
                        yield return new WaitForSeconds(delay);
                        StartCoroutine(BossBattleLoop());
                    }
                }
            }
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
        if (dataController.currentBattle.isBossBattle)
        {
            Debug.Log("Boss Battle");
            StartCoroutine(BossBattleLoop());
        }
        else
        {
            StartCoroutine(BattleLoop());
        }
        
    }

    IEnumerator DisplayBattleResult(bool victory)
    {
        cameraManager.Shake(true, 1);
        yield return new WaitForSeconds(1.60f);
        cameraManager.Shake(true, 1);

        //StopAllCoroutines();
        battleInfoManager.CalculateResult(victory);

        battleInfoManager.DisplayMiddleMessage(0);
        turnQueueManager.ShowTurnQueue(0);
        turnQueueManager.HideTurnQueue(1);
        statusManager.HideMutantStatusSection();
        statusManager.HideScavengerStatusSection();
        attackController.HideAttackButtons();
    }

    // Check if the battle has ended
    public void CheckBattleEnd(int targetCharacter)
    {
        // Here all mutants are dead, which means victory
        if (targetCharacter == 0)
        {
            battleEnd = true;
            StartCoroutine(DisplayBattleResult(true));
        }

        // All scavengers are dead, defeat
        if (targetCharacter == 1)
        {
            battleEnd = true;
            StartCoroutine(DisplayBattleResult(false));
        }

    }

    public void BattleEnd()
    {
        if (dataController.currentBattle.cutsceneAtEnd)
        {
            dataController.currentCutscene = dataController.currentBattle.endCutscene;
        }

        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", true);
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", false);

        if (dataController.currentBattle.nextScene.Equals("Demo"))
        {
            audioManager.PlaySound("Cheers");

            foreach(GameObject message in demoMessage) 
                message.SetActive(true);

            yield return new WaitForSeconds(5f);

            int nextSceneId = dataController.GetNextSceneId("Splash Screen");
            dataController.DeleteAllData();
            SceneManager.LoadScene(nextSceneId);
        }
        else
        {
            int nextSceneId = dataController.GetNextSceneId(dataController.currentBattle.nextScene);
            SceneManager.LoadScene(nextSceneId);
        }
        
    }
}
