using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusManager : MonoBehaviour
{
    #region Battle Managers
    private DataController dataController;
    private StatusManager instance;
    private CharacterManager characterManager;
    private ParticleManager particleManager;
    #endregion

    #region Public
    [Space]
    public GameObject scavengerStatusSection;
    public GameObject[] scavengerStatusPanel;
    public GameObject[] detailedScavengerStatusPanel;

    [Space]
    public GameObject[] healthBars;
    public GameObject[] antBars;

    [Space]
    public GameObject mutantStatusSection;
    public GameObject[] mutantStatusPanel;

    [Space]
    public GameObject pollutionBar;
    public TextMeshProUGUI pollutionValue;

    [Space]
    public Transform canvasTransform;
    public TextMeshProUGUI healPointsPrefab;
    public TextMeshProUGUI smallHealPrefab;
    public TextMeshProUGUI damagePointsPrefab;
    public TextMeshProUGUI smallDamagePrefab;
    public GameObject debuffArrowPrefab;
    public GameObject buffArrowPrefab;
    #endregion

    #region Private
    private int combinedPL;
    #endregion

    // <summary>
    // Destroy any other instance of this script
    // </summary>
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    // <summary>
    // 
    // </summary>
    void Start()
    {
        // Find and get Data Controller
        dataController = FindObjectOfType<DataController>();
        characterManager = FindObjectOfType<CharacterManager>();
        particleManager = FindObjectOfType<ParticleManager>();
    }

    // <summary>
    // Set details according to each scavenger data
    // </summary>
    public void SetScavengerDetails(Player[] scavengerData)
    {
        // Loop through all scavengers
        for (int i = 0; i < scavengerData.Length; i++)
        {
            // Array sent will always be 3, whether it has a data or not
            // To avoid errors always check if there's a scavenger in position
            if (scavengerData[i] != null)
            {
                // Scavenger Icon
                scavengerStatusPanel[i].transform.GetChild(1).GetChild(0).
                    gameObject.GetComponent<Image>().sprite = scavengerData[i].characterHalf;

                // Name
                scavengerStatusPanel[i].transform.GetChild(2).gameObject.
                    GetComponent<TextMeshProUGUI>().text = scavengerData[i].characterName;

                // Class Icon
                scavengerStatusPanel[i].transform.GetChild(3).gameObject.
                    GetComponent<Image>().sprite = scavengerData[i].characterClass.roleLogo;

                // Level
                scavengerStatusPanel[i].transform.GetChild(4).gameObject.
                    GetComponent<TextMeshProUGUI>().text += scavengerData[i].currentLevel;

                // HP, ANT, SPD, DEF values on detailed status
                detailedScavengerStatusPanel[i].transform.GetChild(2).GetChild(2).
                    gameObject.GetComponent<TextMeshProUGUI>().text = scavengerData[i].currentHP.ToString();
                detailedScavengerStatusPanel[i].transform.GetChild(3).GetChild(2).
                    gameObject.GetComponent<TextMeshProUGUI>().text = scavengerData[i].currentSpd.ToString();
                detailedScavengerStatusPanel[i].transform.GetChild(4).GetChild(2).
                    gameObject.GetComponent<TextMeshProUGUI>().text = scavengerData[i].currentAtk.ToString();
                detailedScavengerStatusPanel[i].transform.GetChild(5).GetChild(2).
                    gameObject.GetComponent<TextMeshProUGUI>().text = scavengerData[i].currentDef.ToString();

                // Ult description
                detailedScavengerStatusPanel[i].transform.GetChild(6).GetChild(2).
                    gameObject.GetComponent<TextMeshProUGUI>().text = scavengerData[i].abilities[3].abilityDescription;
            }
        }
    }

    // <summary>
    // Display scavenger status panel
    // </summary>
    public IEnumerator DisplayScavengerStatusSection(Player[] scavengerData)
    {
        // Display
        scavengerStatusSection.SetActive(true);

        // Display each panel, enable hp and ant bar
        int i = 0;
        foreach (Player scavenger in scavengerData)
        {
            if (scavenger != null)
            {
                // Scavenger Status Panel
                scavengerStatusPanel[i].SetActive(true);

                // HP and ANT Bars
                scavengerStatusPanel[i].transform.GetChild(5).
                    GetChild(0).gameObject.SetActive(true);
                scavengerStatusPanel[i].transform.GetChild(6).
                    GetChild(0).gameObject.SetActive(true);

                yield return new WaitForSeconds(0.3f);
            }

            i++;
        }

        yield return new WaitForSeconds(1f);

        // Disable HP and ANT bar animator after so we can
        // manipulate the fill value of the bar
        for (i = 0; i < scavengerData.Length; i++)
        {
            healthBars[i].GetComponent<Animator>().enabled = false;
            antBars[i].GetComponent<Animator>().enabled = false;
            antBars[i].GetComponent<Image>().fillAmount = 0.5f;
        }

        // Apply and display status effects if there's any
        for (i = 0; i < scavengerData.Length; i++)
        {
            // Make sure there's a scavenger in position to avoid errors
            if (scavengerData[i] != null)
            {
                dataController = FindObjectOfType<DataController>();
                if (dataController != null)
                {
                    // Go through all effects and apply each one of it
                    if (dataController.battleModifiers.Length > 0)
                    {
                        foreach (Effect effect in dataController.battleModifiers)
                        {
                            if (effect.type.Equals("Status"))
                            {
                                if (effect.state.Equals("Buff"))
                                {
                                    scavengerData[i].IsBuffed(Instantiate(effect));
                                }
                                else
                                {
                                    scavengerData[i].IsDebuffed(Instantiate(effect));
                                }

                                // Adds effect icon in status panel
                                // Status panel can only show up to 3 icons
                                AddEffectToStatusPanel(dataController.targetParty, i, effect);
                            }
                        }
                            
                        // Add effects to status list
                        // Battle modifiers should be status effects only
                        AddEffectsToStatusList(dataController.targetParty, i, dataController.battleModifiers, null);
                    }
                }
            }
        }
        
    }

    public void ShowScavengerStatusSection()
    {
        scavengerStatusSection.GetComponent<Animator>().SetBool("Down", false);
    }

    public void HideScavengerStatusSection()
    {
        scavengerStatusSection.GetComponent<Animator>().SetBool("Down", true);
    }

    public void ShowStatusPanel(int position)
    {
        scavengerStatusPanel[position].SetActive(true);
    }

    public void HideStatusPanel(int position)
    {
        scavengerStatusPanel[position].SetActive(false);
    }

    // <summary>
    // Shows detailed status of Scavengers
    // </summary>
    public void ShowDetailedScavengerStatus(int scavengerPosition)
    {
        // If any of the panels is open, close it first before opening another
        bool isEnabled = detailedScavengerStatusPanel[scavengerPosition].activeInHierarchy;

        if (isEnabled == false)
        {
            int count = 0;
            foreach (GameObject statusPanel in detailedScavengerStatusPanel)
            {
                if (count != scavengerPosition)
                {
                    if (statusPanel.activeInHierarchy)
                        statusPanel.SetActive(!statusPanel.activeInHierarchy);
                }

                count++;
            }

            detailedScavengerStatusPanel[scavengerPosition].SetActive(!isEnabled);
        }
        else
        {
            detailedScavengerStatusPanel[scavengerPosition].SetActive(!isEnabled);
        }
    }

    public void HideAllDetailedStatusPanels()
    {
        foreach (GameObject detailedPanel in detailedScavengerStatusPanel)
            detailedPanel.SetActive(false);
    }

    // <summary>
    // Display status effect icon
    // </summary>
    public void AddEffectToStatusPanel(string appliedTo, int position, Effect effect)
    {
        if (appliedTo.Equals("Scavenger"))
        {
            GameObject effectPanel = scavengerStatusPanel[position].transform.GetChild(7).gameObject;
            for (int i = 0; i < effectPanel.transform.childCount; i++)
            {
                if (!effectPanel.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    effectPanel.transform.GetChild(i).GetChild(1).
                        gameObject.GetComponent<Image>().sprite = effect.icon;
                    effectPanel.transform.GetChild(i).gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    // <summary>
    // Display info about status effects on a character
    // </summary>
    public void AddEffectsToStatusList(string appliedTo, int position, Effect effect, Sprite originOfEffect)
    {
        if (appliedTo.Equals("Scavenger"))
        {
            detailedScavengerStatusPanel[position].transform.GetChild(8).
                GetChild(0).GetChild(0).gameObject.GetComponent<EffectList>().AddEffect(effect, originOfEffect);
        }
    }

    // <summary>
    // Display info about status effects on a character
    // </summary>
    public void AddEffectsToStatusList(string appliedTo, int position, Effect[] effects, Sprite originOfEffect)
    {
        if (appliedTo.Equals("Scavenger"))
        {
            detailedScavengerStatusPanel[position].transform.GetChild(8).
                GetChild(0).GetChild(0).gameObject.GetComponent<EffectList>().AddEffects(effects, originOfEffect);
        }
    }

    public void UpdateEffect(int effectNo, int duration, int listPosition)
    {
        detailedScavengerStatusPanel[listPosition].transform.GetChild(8).
                GetChild(0).GetChild(0).gameObject.GetComponent<EffectList>().UpdateEffect(effectNo, duration.ToString());
    }

    // <summary>
    // Remove status effect icon
    // </summary>
    public void RemoveEffect(string appliedTo, int scavPos, int effectPos)
    {
        if (appliedTo.Equals("Scavenger"))
        {
            // Loop through children of effect panel, which are the effect icons
            GameObject effectPanel = scavengerStatusPanel[scavPos].transform.GetChild(7).gameObject;
            for (int i = 0; i < effectPanel.transform.childCount; i++)
            {
                if (i == effectPos)
                {
                    // Just to be safe, check if effect icon is active
                    if (effectPanel.transform.GetChild(i).gameObject.activeInHierarchy)
                    {
                        effectPanel.transform.GetChild(i).gameObject.SetActive(false);
                        break;
                    }
                }
            }

            detailedScavengerStatusPanel[scavPos].transform.GetChild(8).
                GetChild(0).GetChild(0).gameObject.GetComponent<EffectList>().RemoveEffect(effectPos);
        }
    }

    // <summary>
    // Set details according to each mutant data
    // </summary>
    public void SetMutantDetails(Enemy[] mutantData)
    {
        // Loop through each mutant
        for (int i = 0; i < mutantData.Length; i++)
        {
            if (mutantData[i] != null)
            {
                // Icon
                mutantStatusPanel[i].GetComponent<Image>().sprite = mutantData[i].characterThumb;

                // Level
                mutantStatusPanel[i].transform.GetChild(0).GetChild(0).
                    gameObject.GetComponent<TextMeshProUGUI>().text = mutantData[i].mutantLevel.ToString();

                // PL
                combinedPL += mutantData[i].currentPollutionLevel;
            }
        }
    }

    public void SetBossDetails(Enemy[] mutantData)
    {
        foreach (Enemy mutant in mutantData)
        {
            if (mutant != null)
            {
                mutantStatusSection.transform.GetChild(5).GetComponent<Image>().sprite = mutant.characterThumb;
                combinedPL += mutant.currentPollutionLevel;
            }
        }
    }

    // <summary>
    // Display mutant status panel
    // </summary>
    public IEnumerator DisplayMutantStatusSection(Enemy[] mutantData)
    {
        // Display status section for waste mutants
        mutantStatusSection.SetActive(true);

        // Display PL Bar
        pollutionBar.transform.parent.gameObject.SetActive(true);
        pollutionBar.GetComponent<Animator>().enabled = false;

        // Display combined PL of all mutants
        int i = 0;
        int increment = (combinedPL / dataController.mutantCount) / 5;
        for (i = 0; i <= combinedPL; i+=increment)
        {
            // Activate text before iterating value
            if (i == 0)
                pollutionValue.gameObject.SetActive(true);

            // Create an increasing effect by constantly changing value of
            // text until it reaches combine PL value
            pollutionValue.GetComponent<TextMeshProUGUI>().text = i.ToString();
            yield return null;
        }

        if (dataController != null)
        {
            if (dataController.currentBattle.isBossBattle)
            {
                mutantStatusSection.transform.GetChild(5).gameObject.SetActive(true);
            }
            else
            {
                // Show mutant icons near PL Bar
                i = 0;
                foreach (Enemy mutant in mutantData)
                {
                    // Make sure there's mutant in position
                    if (mutant != null)
                    {
                        mutantStatusPanel[i].SetActive(true);
                        i++;
                        yield return null;
                    }
                }
            }
        }
    }

    public void HideMutantStatusSection()
    {
        mutantStatusSection.SetActive(false);
    }

    // <summary>
    // Shows damage taken by Scavenger or Mutant
    // </summary>
    public IEnumerator ShowValues(string value, int maxValue, string valueType, GameObject characterObj)
    {
        TextMeshProUGUI valuePrefab;
        int particleIndex = 0;

        if (valueType.Equals("Offensive"))
        {
            if (int.Parse(value) > (maxValue/2))
                valuePrefab = damagePointsPrefab;
            else
                valuePrefab = smallDamagePrefab;

            particleIndex = 2;
        }
        else
        {
            if (int.Parse(value) > (maxValue/2))
                valuePrefab = healPointsPrefab;
            else
                valuePrefab = smallHealPrefab;

            particleIndex = 3;
        }

        TextMeshProUGUI valueObj = Instantiate(valuePrefab, canvasTransform);
        valueObj.text = value;

        BoxCollider2D collider = characterObj.GetComponent<BoxCollider2D>();
        float randomX = Random.Range(collider.bounds.min.x - 5, collider.bounds.max.x + 5);
        float randomY = Random.Range(collider.bounds.center.y + 12, collider.bounds.max.y);

        Vector2 valuePos = Camera.main.WorldToScreenPoint(new Vector3(randomX, randomY, 0));

        valueObj.transform.position = valuePos;
        valueObj.gameObject.SetActive(true);
        //StartCoroutine(particleManager.PlayParticles(particleIndex, new Vector3(randomX, randomY, 0)));

        yield return new WaitForSeconds(1.8f);
        Destroy(valueObj.gameObject);
    }

    public IEnumerator ShowTotalValue(string value, string valueType, int targetType, int particleIndex)
    {
        TextMeshProUGUI valuePrefab = (valueType.Equals("Offensive") ? damagePointsPrefab : healPointsPrefab);
        TextMeshProUGUI valueObj = valuePrefab;
        valueObj.text = value;

        BoxCollider2D collider = characterManager.GetCharacterSection(targetType).GetComponent<BoxCollider2D>();
        float maxX = (targetType == 0) ? collider.bounds.max.x : collider.bounds.min.x;
        float maxY = collider.bounds.max.y - 5f;
        Vector2 valuePos = Camera.main.WorldToScreenPoint(new Vector3(maxX, maxY, 0));

        valueObj.transform.position = valuePos;
        valueObj.gameObject.SetActive(true);
        StartCoroutine(particleManager.PlayParticles(particleIndex, new Vector3(maxX, maxY, 0)));
        
        yield return new WaitForSeconds(1.8f);
        // Destroy(valueObj.gameObject);
    }

    // <summary>
    //
    // </summary>
    public IEnumerator ShowBuff(GameObject targetObject, string effectState)
    {
        GameObject buffArrow = Instantiate((effectState.Equals("Buff")) ? buffArrowPrefab : debuffArrowPrefab, canvasTransform);
        BoxCollider2D collider = targetObject.GetComponent<BoxCollider2D>();

        bool isScav = targetObject.GetComponent<CharacterMonitor>().CheckCharacterType("Scavenger");
        float xPos = 0;
        float yPos = 0;

        if (isScav) 
        {
            xPos = collider.bounds.max.x;
            yPos = collider.bounds.max.y - 15f;
        }
        else
        {
            xPos = collider.bounds.max.x;
            yPos = collider.bounds.max.y - 20f;
        }
            
        Vector2 arrowPos = Camera.main.WorldToScreenPoint(new Vector3(xPos, yPos, 0));

        buffArrow.transform.position = arrowPos;
        buffArrow.SetActive(true);

        yield return new WaitForSeconds(1f);

        buffArrow.SetActive(false);
        Destroy(buffArrow);
    }

    public IEnumerator IncrementHealthBar(float currentHealth, float maxHealth, int position)
    {
        float hpLeft = currentHealth / maxHealth;
        float hpBarValue = healthBars[position].GetComponent<Image>().fillAmount;

        while (hpBarValue < hpLeft)
        {
            hpBarValue += 0.1f;
            if (hpBarValue > maxHealth)
                hpBarValue = maxHealth;

            healthBars[position].GetComponent<Image>().fillAmount = hpBarValue;

            yield return new WaitForSeconds(0.1f);
        }

        detailedScavengerStatusPanel[position].transform.GetChild(2).GetChild(2).
                    gameObject.GetComponent<TextMeshProUGUI>().text = currentHealth.ToString();
    }

    public IEnumerator DecrementHealthBar(float currentHealth, float maxHealth, int position)
    {
        float hpLeft = currentHealth / maxHealth;
        float hpBarValue = healthBars[position].GetComponent<Image>().fillAmount;

        while (hpBarValue > hpLeft)
        {
            hpBarValue -= 0.1f;
            if (hpBarValue < 0)
                hpBarValue = 0;

            healthBars[position].GetComponent<Image>().fillAmount = hpBarValue;

            yield return new WaitForSeconds(0.1f);
        }

        detailedScavengerStatusPanel[position].transform.GetChild(2).GetChild(2).
                    gameObject.GetComponent<TextMeshProUGUI>().text = currentHealth.ToString();
    }

    public IEnumerator IncrementAntidoteBar(float currentAnt, float maxAnt, int position)
    {
        float antidoteLeft = currentAnt / maxAnt;
        float antBarValue = antBars[position].GetComponent<Image>().fillAmount;

        while (antBarValue < antidoteLeft)
        {
            antBarValue += 0.1f;
            if (antBarValue > maxAnt)
                antBarValue = maxAnt;

            antBars[position].GetComponent<Image>().fillAmount = antBarValue;

            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator DecrementAntidoteBar(float currentAnt, float maxAnt, int position)
    {
        float antidoteLeft = currentAnt / maxAnt;
        float antBarValue = antBars[position].GetComponent<Image>().fillAmount;

        while (antBarValue > antidoteLeft)
        {
            antBarValue -= 0.1f;
            if (antBarValue < 0)
                antBarValue = 0;

            antBars[position].GetComponent<Image>().fillAmount = antBarValue;

            yield return new WaitForSeconds(0.1f);
        }
    }

    // <summary>
    // Decrement pollution bar
    // </summary>
    public IEnumerator DecrementPollutionBar(int damageTaken) 
    {
        GameObject[] mutantPrefabs = characterManager.GetAllCharacterPrefabs(0);

        int currentCombinedPL = 0;
        int maxCombinedPL = 0;
        foreach (GameObject mutantPrefab in mutantPrefabs) 
        {
            if (mutantPrefab != null)
            {
                currentCombinedPL += mutantPrefab.GetComponent<CharacterMonitor>().CurrentHealth;
                maxCombinedPL += mutantPrefab.GetComponent<CharacterMonitor>().GetMutantMaxHealth();
            }
        }

        float combinedPLLeft = (float)currentCombinedPL / (float)maxCombinedPL;
        float pollutionBarValue = pollutionBar.GetComponent<Image>().fillAmount;

        while (pollutionBarValue > combinedPLLeft)
        {
            pollutionBarValue -= 0.05f;
            if (pollutionBarValue < 0)
                pollutionBarValue = 0;

            pollutionBar.GetComponent<Image>().fillAmount = pollutionBarValue;
            yield return new WaitForSeconds(0.05f);
        }

        for (int i = 4; i <= damageTaken; i+=4)
        {
            pollutionValue.text = ((currentCombinedPL + damageTaken) - i).ToString();
            yield return null;
        }

        pollutionValue.text = currentCombinedPL.ToString();
    }

    public IEnumerator IncrementPollutionBar(int pollutionGained)
    {
        GameObject[] mutantPrefabs = characterManager.GetAllCharacterPrefabs(0);

        int currentCombinedPL = 0;
        int maxCombinedPL = 0;
        foreach (GameObject mutantPrefab in mutantPrefabs)
        {
            if (mutantPrefab != null)
            {
                currentCombinedPL += mutantPrefab.GetComponent<CharacterMonitor>().CurrentHealth;
                maxCombinedPL += mutantPrefab.GetComponent<CharacterMonitor>().GetMutantMaxHealth();
            }
        }

        float combinedPLLeft = (float)currentCombinedPL / (float)maxCombinedPL;
        float pollutionBarValue = pollutionBar.GetComponent<Image>().fillAmount;

        while (pollutionBarValue < combinedPLLeft)
        {
            pollutionBarValue += 0.05f;
            if (pollutionBarValue > maxCombinedPL)
                pollutionBarValue = maxCombinedPL;

            pollutionBar.GetComponent<Image>().fillAmount = pollutionBarValue;
            yield return new WaitForSeconds(0.05f);
        }

        for (int i = 4; i <= pollutionGained; i += 4)
        {
            pollutionValue.text = ((currentCombinedPL - pollutionGained) + i).ToString();
            yield return null;
        }

        pollutionValue.text = currentCombinedPL.ToString();
    }
}
