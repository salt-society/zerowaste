using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusManager : MonoBehaviour {

    [Header("Scavengers Status Panel")]
    public GameObject scavengerStatus;

    [Space]
    public GameObject[] scavengerName;
    public GameObject[] scavengerLvl;
    public GameObject[] scavengerIcon;
    public GameObject[] classIcon;
    public GameObject[] scavengerHealthFills;
    public GameObject[] scavengerHealthBars;
    public GameObject[] scavengerAntBars;
    public GameObject[] scavengerAntFills;
    public GameObject[] damageCounters;

    [Space]
    public GameObject pollutionBarPanel;
    public GameObject pollutionLevelPrefab;

    private GameObject[] pollutionLevels;

    private int scavengerCount;
    private int mutantCount;

    private List<double> scavengerCurrentHealth;
    private List<double> scavengerMaxHealth;
    private List<double> scavengerCurrentAntidote;
    private List<double> scavengerMaxAntidote;

    private List<double> mutantCurrentPollutionLvl;
    private List<double> mutantMaxPollutionLvl;

    public void SetCharacterCount(int scavengerCount, int mutantCount)
    {
        this.scavengerCount = scavengerCount;
        this.mutantCount = mutantCount;
    }

    public void SetCharactersStatistics(Player[] scavengers, Enemy[] mutants)
    {
        scavengerCurrentHealth = new List<double>();
        scavengerMaxHealth = new List<double>();
        scavengerCurrentAntidote = new List<double>();
        scavengerMaxAntidote = new List<double>();

        foreach (Player scavenger in scavengers)
        {
            double maxHealth = scavenger.baseHP + 
                (int)((scavenger.currentLevel - 1) * scavenger.hpModifier);
            scavengerMaxHealth.Add(maxHealth);
            scavengerCurrentHealth.Add(scavenger.currentHP);

            double maxAntidote = scavenger.baseAnt;
            scavengerMaxAntidote.Add(maxAntidote);
            scavengerCurrentAntidote.Add(scavenger.currentAnt);
        }

        mutantCurrentPollutionLvl = new List<double>();
        mutantMaxPollutionLvl = new List<double>();

        foreach (Enemy mutant in mutants)
        {
            mutantCurrentPollutionLvl.Add(mutant.currentPollutionLevel);
            mutantMaxPollutionLvl.Add(mutant.maxPollutionLevel);
        }
    }

    public double ComputeHealth(double current, double max)
    {
        double health = current / max;
        return health;
    }

    public double ComputeAntidote(double current, double max)
    {
        double antidote = current / max;
        return antidote;
    }

    public void SetScavengerDetails(Player[] scavengers)
    {
        for (int i = 0; i < scavengerCount; i++)
        {
            scavengerIcon[i].GetComponent<Image>().sprite = scavengers[i].characterHalf;
            scavengerName[i].GetComponent<TextMeshProUGUI>().text = scavengers[i].characterName;
            scavengerLvl[i].GetComponent<TextMeshProUGUI>().text = "LVL. " + scavengers[i].currentLevel;
            classIcon[i].GetComponent<Image>().sprite = scavengers[i].characterClass.roleLogo;
        }
    }

    IEnumerator SetScavengerHealthBar(double currentHealth, double maxHealth, int position)
    {
        double fillAmount = ComputeHealth(currentHealth, maxHealth);

        foreach (GameObject healthFill in scavengerHealthFills)
        {
            healthFill.GetComponent<Image>().fillAmount = 0;
            healthFill.GetComponent<Image>().fillAmount = (float)fillAmount;
        }

        yield return null;
    }

    IEnumerator SetScavengerAntidoteBar(double currentAnt, double maxAnt, int position)
    {
        double fillAmount = ComputeAntidote(currentAnt, maxAnt);

        foreach (GameObject antFill in scavengerAntFills)
        {
            antFill.GetComponent<Image>().fillAmount = 0;
            antFill.GetComponent<Image>().fillAmount = (float)fillAmount;
        }

        yield return null;
    }

    public IEnumerator DisplayScavengerDetails()
    {
        scavengerStatus.SetActive(true);
        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < scavengerCount; i++)
        {
            scavengerIcon[i].SetActive(true);
            scavengerName[i].SetActive(true);
            scavengerLvl[i].SetActive(true);
            classIcon[i].SetActive(true);

            scavengerHealthBars[i].SetActive(true);
            scavengerHealthFills[i].SetActive(true);
            StartCoroutine(SetScavengerHealthBar(scavengerCurrentHealth[i], scavengerMaxHealth[i], i));

            scavengerAntBars[i].SetActive(true);
            scavengerAntFills[i].SetActive(true);
            StartCoroutine(SetScavengerAntidoteBar(scavengerCurrentAntidote[i], 
                scavengerMaxAntidote[i], i));
        }

        yield return new WaitForSeconds(3f);
    }

    public void IncrementHealth(double heal, int position)
    {
        double additionHealth = heal / scavengerMaxHealth[position];
        scavengerHealthFills[position].GetComponent<Image>().fillAmount += (float)additionHealth;
    }

    public void DecrementHealth(double damage, int position)
    {
        double damageTaken = damage / scavengerMaxHealth[position];
        scavengerHealthFills[position].GetComponent<Image>().fillAmount -= (float)damageTaken;
    }

    public void ShowDamage(string damagePoints, int position)
    {
        damageCounters[0].GetComponent<TextMeshProUGUI>().text = damagePoints;
        damageCounters[0].SetActive(true);
    }
    
    public void IncrementAntidote(double charge, int position)
    {
        double antidoteCharge = charge / scavengerMaxAntidote[position];
        scavengerAntFills[position].GetComponent<Image>().fillAmount += (float)antidoteCharge;
    }

    public void DecrementAntidote(double lostAntidote, int position)
    {
        double antidoteUsed = lostAntidote / scavengerMaxAntidote[position];
        scavengerAntFills[position].GetComponent<Image>().fillAmount -= (float)antidoteUsed;
    }

    public void SetPollutionLevelBarCount()
    {
        pollutionLevels = new GameObject[mutantCount];
    }

    public void DisplayPollutionBars(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        pollutionBarPanel.SetActive(showComponent);
    }

    public void AddPollutionLevelBar(float minX, float minY, float extentY, int position)
    {
        Vector3 pollutionBarWorldPos = new Vector3(minX, minY + extentY);
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(pollutionBarWorldPos);

        GameObject pollutionBarObj = Instantiate(pollutionLevelPrefab, 
            pollutionBarPanel.transform);
        pollutionBarObj.transform.position = screenPoint;
        pollutionLevels[position] = pollutionBarObj.transform.GetChild(0).gameObject;
    }

    public void IncrementPollutionBar(double heal, int position)
    {
        double additionalHealth = heal / mutantMaxPollutionLvl[position];
        pollutionLevels[position].GetComponent<Image>().fillAmount += (float)additionalHealth;
    }

    public void DecrementPollutionBar(double damage, int position)
    {
        double damageTaken = damage / mutantMaxPollutionLvl[position];
        pollutionLevels[position].GetComponent<Image>().fillAmount -= (float)damageTaken;
    }

    public void DisplayEffects(string appliedTo, Effect effect)
    {

    }

}
