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
    public GameObject[] scavengerExpBars;

    [Space]
    public GameObject victoryLootBox;
    public GameObject defeatLootBox;

    void Start()
    {
        dataController = FindObjectOfType<DataController>();
        statusManager = FindObjectOfType<StatusManager>();
        particleManager = FindObjectOfType<ParticleManager>();
    }

    public void ShowStartAnimation(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        battleStart.SetActive(showComponent);
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
            StartCoroutine(Victory());
        }
        else 
        {
            scrapReward = scrapReward - (scrapReward * (int)(0.10f));
            expReward = expReward - (expReward * (int)(0.10f));

            StartCoroutine(Defeat());
        }
    }

    public IEnumerator Victory()
    {
        battleResult.SetActive(true);
        victoryLabel.SetActive(true);
        yield return new WaitForSeconds(1f);

        StartCoroutine(ShowVictoryLootBox());
    }

    public IEnumerator ShowVictoryLootBox()
    {
        victoryLootBox.SetActive(true);
        yield return new WaitForSeconds(1f);

        victoryLootBox.GetComponent<Animator>().SetBool("Open", true);
        StartCoroutine(particleManager.PlayParticles(0, Camera.main.ScreenToWorldPoint(victoryLootBox.transform.position)));

        yield return new WaitForSeconds(1f);
        victoryLootBox.SetActive(false);
    }

    public IEnumerator Defeat()
    {
        battleResult.SetActive(true);
        defeatLabel.SetActive(true);
        yield return new WaitForSeconds(2f);

        StartCoroutine(ShowDefeatLootBox());
    }

    public IEnumerator ShowDefeatLootBox()
    {
        victoryLootBox.SetActive(true);
        yield return new WaitForSeconds(1f);

        victoryLootBox.GetComponent<Animator>().SetBool("Open", true);
        StartCoroutine(particleManager.PlayParticles(0, Camera.main.ScreenToWorldPoint(victoryLootBox.transform.position)));

        yield return new WaitForSeconds(1f);
        victoryLootBox.SetActive(false);
    }
}
