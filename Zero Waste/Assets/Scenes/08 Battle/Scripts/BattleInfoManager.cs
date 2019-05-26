using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleInfoManager : MonoBehaviour {

    private DataController dataController;
    private StatusManager statusManager;
    private ParticleManager particleManager;
    private CharacterManager characterManager;
    private AudioManager audioManager;
    private BattleController battleController;

    private int scrapReward;
    private int expReward;

    public GameObject battleStart;

    public GameObject bossBattleBackground;
    public GameObject bossBattleStart;

    [Space]
    public GameObject turnProcessSign;
    public GameObject turnSignPanel;
    public GameObject turnSignInnerBox;
    public Image currentCharacter;
    public TextMeshProUGUI currentCharName;

    [Space]
    public GameObject battleResult;
    public GameObject victoryLabel;
    public GameObject defeatLabel;

    [Space]
    public GameObject scavengerExp;
    public GameObject[] scavengerExpObj;

    [Space]
    public GameObject victoryLootBox;
    public GameObject defeatLootBox;

    [Space]
    public GameObject fadeTransition;

    void Awake()
    {
        dataController = FindObjectOfType<DataController>();
        statusManager = FindObjectOfType<StatusManager>();
        particleManager = FindObjectOfType<ParticleManager>();
        characterManager = FindObjectOfType<CharacterManager>();
        audioManager = FindObjectOfType<AudioManager>();
        battleController = FindObjectOfType<BattleController>();
    }

    public void ShowStartAnimation(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        battleStart.SetActive(showComponent);
        //audioManager.PlaySound("Prrrt");
    }

    public IEnumerator ShowBossAnimation(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;

        if (showComponent)
        {
            bossBattleBackground.SetActive(showComponent);
            yield return new WaitForSeconds(0.5f);
            bossBattleStart.SetActive(showComponent);
            //audioManager.PlaySound("Prrrt");
        }
        else
        {
            bossBattleBackground.GetComponent<Animator>().SetBool("Hide", true);
            yield return new WaitForSeconds(0.6f);
            bossBattleBackground.SetActive(showComponent);
            bossBattleStart.SetActive(showComponent);
        }
    }

    public void SetMiddleMessage(string message)
    {
        turnProcessSign.GetComponentInChildren<TextMeshProUGUI>().text = message;
    }

    public void DisplayMiddleMessage(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        turnProcessSign.SetActive(showComponent);
    }

    public void ShowMiddleMessage(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        turnProcessSign.GetComponent<Animator>().SetBool("Hide", showComponent);
    }

    public void HideMiddleMessage(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        turnProcessSign.GetComponent<Animator>().SetBool("Hide", showComponent);
    }

    public bool GetMiddleMessageState()
    {
        return turnProcessSign.GetComponent<Animator>().GetBool("Hide");
    }

    public void SetCurrentTurn(Sprite currentCharacter, string name)
    {
        this.currentCharacter.sprite = currentCharacter;
        this.currentCharName.text = name + "'s Turn";
    }

    public void DisplayNextTurnPanel(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        turnSignPanel.SetActive(showComponent);

        statusManager.HideAllDetailedStatusPanels();
    }

    public void DisplayNextTurnSign(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        turnSignInnerBox.SetActive(showComponent);
    }

    public void AddScrap(int scrap)
    {
        scrapReward += scrap;
    }

    public void AddExp(int exp)
    {
        expReward += exp;
    }

    public void CalculateResult(bool victory)
    {
        if (victory)
        {
            dataController.AddScrap(scrapReward);

            GameObject[] scavObjs = characterManager.GetAllCharacterPrefabs(1);
            StartCoroutine(Victory(scavObjs));
        }
        else 
        {
            if (scrapReward != 0 && expReward != 0)
            {
                scrapReward = scrapReward - (scrapReward * (int)(0.10f));
                expReward = expReward - (expReward * (int)(0.10f));

                dataController.AddScrap(scrapReward);

                GameObject[] scavObjs = characterManager.GetAllCharacterPrefabs(1);
                StartCoroutine(DefeatWithScraps(scavObjs));
            }
            else 
            {
                StartCoroutine(DefeatWithoutScrap());
            }
        }
    }

    public IEnumerator Victory(GameObject[] scavObjs)
    {
        StartCoroutine(audioManager.StopSound(dataController.currentNode.BGM, 1f));
        audioManager.PlaySound("Victory");

        battleResult.SetActive(true);
        victoryLabel.SetActive(true);
        yield return new WaitForSeconds(2.0f);

        scavengerExp.transform.GetChild(2).gameObject.SetActive(true);
        scavengerExp.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        int i = 0;
        foreach (GameObject scavObj in scavObjs)
        {
            int expReward = this.expReward;
            if (i < scavengerExpObj.Length)
            {
                // Get scavenger data, add exp, then save before the animations
                CharacterMonitor charMonitor = scavObj.GetComponent<CharacterMonitor>();

                // Set scavenger icon, name, current level
                scavengerExpObj[i].transform.GetChild(0).GetComponent<Image>().sprite = charMonitor.Scavenger.characterThumb;
                scavengerExpObj[i].transform.GetChild(1).GetComponent<Image>().sprite = charMonitor.Scavenger.characterThumb;
                scavengerExpObj[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = charMonitor.Scavenger.characterName;
                scavengerExpObj[i].transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = charMonitor.Scavenger.currentLevel.ToString();
                scavengerExpObj[i].transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = charMonitor.Scavenger.currentLevel.ToString();

                // Show scavenger data
                scavengerExpObj[i].SetActive(true);
                yield return new WaitForSeconds(0.5f);

                // Show acquired exp
                scavengerExpObj[i].transform.GetChild(5).gameObject.SetActive(true);
                if (charMonitor.Scavenger.currentLevel < 30)
                {
                    scavengerExpObj[i].transform.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + expReward.ToString() + " EXP";
                    scavengerExpObj[i].transform.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>().text = "+" + expReward.ToString() + " EXP";

                    LevelRequirements levelReq = new LevelRequirements();
                    while (expReward > 0)
                    {
                        if ((charMonitor.Scavenger.currentExp + expReward) >= levelReq.expReq[charMonitor.Scavenger.currentLevel - 1])
                        {
                            int levelUp = charMonitor.Scavenger.currentLevel;
                            if (levelUp++ <= charMonitor.Scavenger.currentLevelCap)
                            {
                                audioManager.PlaySound("Level Up");
                                float currentExp = (float)charMonitor.Scavenger.currentExp / (float)levelReq.expReq[charMonitor.Scavenger.currentLevel - 1];
                                scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = currentExp;
                                float leftToFill = 1f - currentExp;

                                yield return new WaitForSeconds(0.5f);
                                for (float k = 0.0f; k <= leftToFill; k += 0.1f)
                                {
                                    scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = currentExp + k;
                                    yield return null;
                                }
                                scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = 1f;

                                charMonitor.Scavenger.currentExp = 0;
                                expReward -= levelReq.expReq[charMonitor.Scavenger.currentLevel - 1];

                                charMonitor.Scavenger.currentLevel++;
                                scavengerExpObj[i].transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = charMonitor.Scavenger.currentLevel.ToString();
                                scavengerExpObj[i].transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = charMonitor.Scavenger.currentLevel.ToString();
                                scavengerExpObj[i].transform.GetChild(6).gameObject.SetActive(true);

                                yield return new WaitForSeconds(1f);
                            }
                            else
                            {
                                audioManager.PlaySound("Good Effect");
                                scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = 1f;
                                scavengerExpObj[i].transform.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = "MAX LEVEL CAP";
                                scavengerExpObj[i].transform.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>().text = "MAX LEVEL CAP";
                            }
                        }
                        else
                        {
                            audioManager.PlaySound("Good Effect");
                            float currentExp = (float)charMonitor.Scavenger.currentExp / (float)levelReq.expReq[charMonitor.Scavenger.currentLevel - 1];
                            float expRewardFloat = (float)expReward / (float)levelReq.expReq[charMonitor.Scavenger.currentLevel - 1];
                            scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = currentExp;

                            yield return new WaitForSeconds(0.5f);
                            for (float k = 0.0f; k <= expRewardFloat; k += 0.1f)
                            {
                                scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = currentExp + k;
                                yield return null;
                            }

                            scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = currentExp + expRewardFloat;
                            expReward -= expReward;
                        }
                    }
                }
                else
                {
                    audioManager.PlaySound("Max Level");
                    scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = 1f;
                    scavengerExpObj[i].transform.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = "MAX LEVEL";
                    scavengerExpObj[i].transform.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>().text = "MAX LEVEL";
                }
                dataController.AddExp(charMonitor.Scavenger);
            }
            i++;
        }

        yield return new WaitForSeconds(1.0f);
        scavengerExp.GetComponent<Animator>().SetBool("Up", true);
        yield return new WaitForSeconds(0.5f);
        scavengerExp.SetActive(false);

        StartCoroutine(ShowVictoryLootBox());
        yield return new WaitForSeconds(5.0f);

        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", true);
        battleController.BattleEnd();
    }

    public IEnumerator ShowVictoryLootBox()
    {
        victoryLootBox.SetActive(true);
        yield return new WaitForSeconds(1.0f);

        victoryLootBox.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "+ " + scrapReward + " scraps!";

        victoryLootBox.GetComponent<Animator>().SetBool("Open", true);
        yield return new WaitForSeconds(1.0f);

        victoryLootBox.transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        victoryLootBox.SetActive(false);
    }

    public IEnumerator DefeatWithScraps(GameObject[] scavObjs)
    {
        StartCoroutine(audioManager.StopSound(dataController.currentNode.BGM, 1f));
        audioManager.PlaySound("Defeat");

        battleResult.SetActive(true);
        defeatLabel.SetActive(true);
        yield return new WaitForSeconds(2.0f);

        scavengerExp.transform.GetChild(1).gameObject.SetActive(true);
        scavengerExp.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        int i = 0;
        foreach (GameObject scavObj in scavObjs)
        {
            int expReward = this.expReward;
            if (i < scavengerExpObj.Length)
            {
                // Get scavenger data, add exp, then save before the animations
                CharacterMonitor charMonitor = scavObj.GetComponent<CharacterMonitor>();

                // Set scavenger icon, name, current level
                scavengerExpObj[i].transform.GetChild(0).GetComponent<Image>().sprite = charMonitor.Scavenger.characterThumb;
                scavengerExpObj[i].transform.GetChild(1).GetComponent<Image>().sprite = charMonitor.Scavenger.characterThumb;
                scavengerExpObj[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = charMonitor.Scavenger.characterName;
                scavengerExpObj[i].transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = charMonitor.Scavenger.currentLevel.ToString();
                scavengerExpObj[i].transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = charMonitor.Scavenger.currentLevel.ToString();

                // Show scavenger data
                scavengerExpObj[i].SetActive(true);
                yield return new WaitForSeconds(0.5f);

                // Show acquired exp
                scavengerExpObj[i].transform.GetChild(5).gameObject.SetActive(true);
                if (charMonitor.Scavenger.currentLevel < 30)
                {
                    scavengerExpObj[i].transform.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + expReward.ToString() + " EXP";
                    scavengerExpObj[i].transform.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>().text = "+" + expReward.ToString() + " EXP";

                    LevelRequirements levelReq = new LevelRequirements();
                    while (expReward > 0)
                    {
                        if ((charMonitor.Scavenger.currentExp + expReward) >= levelReq.expReq[charMonitor.Scavenger.currentLevel - 1])
                        {
                            int levelUp = charMonitor.Scavenger.currentLevel;
                            if (levelUp++ <= charMonitor.Scavenger.currentLevelCap)
                            {
                                audioManager.PlaySound("Level Up");
                                float currentExp = (float)charMonitor.Scavenger.currentExp / (float)levelReq.expReq[charMonitor.Scavenger.currentLevel - 1];
                                scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = currentExp;
                                float leftToFill = 1f - currentExp;

                                yield return new WaitForSeconds(0.5f);
                                for (float k = 0.0f; k <= leftToFill; k += 0.1f)
                                {
                                    scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = currentExp + k;
                                    yield return null;
                                }
                                scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = 1f;

                                charMonitor.Scavenger.currentExp = 0;
                                expReward -= levelReq.expReq[charMonitor.Scavenger.currentLevel - 1];

                                charMonitor.Scavenger.currentLevel++;
                                scavengerExpObj[i].transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = charMonitor.Scavenger.currentLevel.ToString();
                                scavengerExpObj[i].transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = charMonitor.Scavenger.currentLevel.ToString();
                                scavengerExpObj[i].transform.GetChild(6).gameObject.SetActive(true);

                                yield return new WaitForSeconds(1f);
                            }
                            else
                            {
                                audioManager.PlaySound("Good Effect");
                                scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = 1f;
                                scavengerExpObj[i].transform.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = "MAX LEVEL CAP";
                                scavengerExpObj[i].transform.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>().text = "MAX LEVEL CAP";
                            }
                        }
                        else
                        {
                            audioManager.PlaySound("Good Effect");
                            float currentExp = (float)charMonitor.Scavenger.currentExp / (float)levelReq.expReq[charMonitor.Scavenger.currentLevel - 1];
                            float expRewardFloat = (float)expReward / (float)levelReq.expReq[charMonitor.Scavenger.currentLevel - 1];
                            scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = currentExp;

                            yield return new WaitForSeconds(0.5f);
                            for (float k = 0.0f; k <= expRewardFloat; k += 0.1f)
                            {
                                scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = currentExp + k;
                                yield return null;
                            }

                            scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = currentExp + expRewardFloat;
                            expReward -= expReward;
                        }
                    }
                }
                else
                {
                    audioManager.PlaySound("Max Level");
                    scavengerExpObj[i].transform.GetChild(4).GetChild(0).GetComponent<Image>().fillAmount = 1f;
                    scavengerExpObj[i].transform.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = "MAX LEVEL";
                    scavengerExpObj[i].transform.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>().text = "MAX LEVEL";
                }
                dataController.AddExp(charMonitor.Scavenger);
            }
            i++;
        }

        yield return new WaitForSeconds(1.0f);
        scavengerExp.GetComponent<Animator>().SetBool("Up", true);
        yield return new WaitForSeconds(0.5f);
        scavengerExp.SetActive(false);

        StartCoroutine(ShowVictoryLootBox());
        yield return new WaitForSeconds(5.0f);

        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", true);
        battleController.BattleEnd();
    }

    public IEnumerator ShowDefeatLootBox()
    {
        yield return null;
    }

    public IEnumerator DefeatWithoutScrap()
    {
        StartCoroutine(audioManager.StopSound(dataController.currentNode.BGM, 1f));
        audioManager.PlaySound("Defeat");

        battleResult.SetActive(true);
        defeatLabel.SetActive(true);
        yield return new WaitForSeconds(2.0f);

        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", true);
        battleController.BattleEnd();
    }
}
