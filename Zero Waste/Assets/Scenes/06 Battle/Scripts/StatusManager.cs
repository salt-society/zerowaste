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
    public GameObject damageCounter;
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
                detailedScavengerStatusPanel[i].transform.GetChild(0).GetChild(1).
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
                detailedScavengerStatusPanel[i].transform.GetChild(1).GetChild(1).
                    gameObject.GetComponent<TextMeshProUGUI>().text = scavengerData[i].currentHP.ToString();
                detailedScavengerStatusPanel[i].transform.GetChild(2).GetChild(1).
                    gameObject.GetComponent<TextMeshProUGUI>().text = scavengerData[i].currentAnt.ToString();
                detailedScavengerStatusPanel[i].transform.GetChild(3).GetChild(1).
                    gameObject.GetComponent<TextMeshProUGUI>().text = scavengerData[i].currentSpd.ToString();
                detailedScavengerStatusPanel[i].transform.GetChild(4).GetChild(1).
                    gameObject.GetComponent<TextMeshProUGUI>().text = scavengerData[i].currentDef.ToString();

                // Ult and Skill description
                detailedScavengerStatusPanel[i].transform.GetChild(7).GetChild(1).
                    gameObject.GetComponent<TextMeshProUGUI>().text = scavengerData[i].abilities[3].abilityDescription;
                detailedScavengerStatusPanel[i].transform.GetChild(8).GetChild(1).
                    gameObject.GetComponent<TextMeshProUGUI>().text = scavengerData[i].abilities[2].abilityDescription;
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
                scavengerStatusPanel[i].transform.GetChild(6).
                    GetChild(0).gameObject.SetActive(true);
                scavengerStatusPanel[i].transform.GetChild(7).
                    GetChild(0).gameObject.SetActive(true);

                yield return new WaitForSeconds(1f);

                // Disable HP and ANT animator after
                scavengerStatusPanel[i].transform.GetChild(6).
                    GetChild(0).gameObject.GetComponent<Animator>().enabled = false;
                scavengerStatusPanel[i].transform.GetChild(7).
                    GetChild(0).gameObject.GetComponent<Animator>().enabled = false;
            }

            i++;
        }

        yield return null;

        // Display status effects
        for(i = 0; i < scavengerData.Length; i++)
        {
            // Make sure there's a scavenger in position to avoid errors
            if (scavengerData[i] != null)
            {
                // Effects, if there's any at start
                dataController = FindObjectOfType<DataController>();
                if (dataController != null)
                {
                    // Can only show 3 effects on status panel
                    if (dataController.targetParty.Equals("Scavenger"))
                    {
                        scavengerStatusPanel[i].transform.GetChild(7).gameObject.SetActive(true);

                        if (dataController.battleModifiers.Length > 3)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                scavengerStatusPanel[i].transform.GetChild(8).GetChild(j).
                                    GetChild(1).gameObject.GetComponent<Image>().sprite = dataController.battleModifiers[j].effectIcon;
                                scavengerStatusPanel[i].transform.GetChild(8).gameObject.SetActive(true);
                                yield return new WaitForSeconds(0.3f);
                            }
                        }
                        else
                        {
                            // Won't execute if 0
                            int effectCount = 0;
                            foreach (Effect effect in dataController.battleModifiers)
                            {
                                scavengerStatusPanel[i].transform.GetChild(8).GetChild(effectCount).
                                    GetChild(1).gameObject.GetComponent<Image>().sprite = effect.effectIcon;
                                scavengerStatusPanel[i].transform.GetChild(8).GetChild(effectCount).
                                    gameObject.SetActive(true);
                                effectCount++;
                                yield return new WaitForSeconds(0.3f);
                            }
                        }

                        // Show details of all effects
                        detailedScavengerStatusPanel[i].transform.GetChild(5).
                            GetChild(0).GetChild(0).gameObject.GetComponent<EffectList>().AddEffects(dataController.battleModifiers, null);
                    }
                }
            }
        }
        
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

    // <summary>
    // Display status effect icon
    // </summary>
    public void AddEffect(string appliedTo, int position, Effect effect)
    {
        if (appliedTo.Equals("Scavenger"))
        {
            GameObject effectPanel = scavengerStatusPanel[position].transform.GetChild(7).gameObject;
            for (int i = 0; i < effectPanel.transform.childCount; i++)
            {
                if (!effectPanel.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    effectPanel.transform.GetChild(i).gameObject.
                        GetComponent<Image>().sprite = effect.effectIcon;
                    effectPanel.transform.GetChild(i).gameObject.SetActive(true);
                    break;
                }
            }
        }
        else
        {

        }
    }

    // <summary>
    // Remove status effect icon
    // </summary>
    public void RemoveEffect(string appliedTo, int position, Effect effect)
    {
        if (appliedTo.Equals("Scavenger"))
        {
            // Loop through children of effect panel, which are the effect icons
            GameObject effectPanel = scavengerStatusPanel[position].transform.GetChild(7).gameObject;
            for (int i = 0; i < effectPanel.transform.childCount; i++)
            {
                // Just to be safe, check if effect icon is active
                if (effectPanel.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    // Then see if effect sprite matches effect to be removed
                    if(effectPanel.transform.GetChild(i).gameObject.
                        GetComponent<Image>().sprite == effect.effectIcon)
                    {
                        effectPanel.transform.GetChild(i).gameObject.SetActive(false);
                        break;
                    }
                }
            }
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
        int increment = (combinedPL / dataController.wasteCount) / 5;
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

    // <summary>
    // Shows damage taken by Scavenger or Mutant
    // </summary>
    public void ShowDamagePoints(string damagePoints, GameObject characterObject)
    {
        // Set value damage counter
        damageCounter.GetComponent<TextMeshProUGUI>().text = damagePoints;

        // Get box collider of target so damage counter can be positioned
        BoxCollider2D collider = characterObject.GetComponent<BoxCollider2D>();
        float midpoint = collider.bounds.center.x;
        Vector2 damageCounterPosition = Camera.main.WorldToScreenPoint(new Vector3(midpoint, 8, 0));

        damageCounter.transform.position = damageCounterPosition;
        damageCounter.SetActive(true);
    }

    // <summary>
    // Hide damage points
    // </summary>
    public IEnumerator HideDamagePoints()
    {
        damageCounter.GetComponent<Animator>().SetBool("Hide", true);
        yield return null;
        damageCounter.SetActive(false);
        damageCounter.GetComponent<Animator>().SetBool("Hide", false);
    }

    public IEnumerator DecrementAntidote(float currentAnt, float maxAnt, int position)
    {
        float antidoteLeft = currentAnt / maxAnt;
        float antBarValue = antBars[position].GetComponent<Image>().fillAmount;

        while (antBarValue > antidoteLeft)
        {
            antBarValue -= 0.1f;
            if (antBarValue < 0)
                antBarValue = 0;

            antBars[position].GetComponent<Image>().fillAmount -= antBarValue;

            Debug.Log("Ant Bar Value: " + antBarValue);
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
            currentCombinedPL += mutantPrefab.GetComponent<CharacterMonitor>().CurrentHealth;
            maxCombinedPL += mutantPrefab.GetComponent<CharacterMonitor>().GetMutantMaxHealth();
        }

        Debug.Log(currentCombinedPL);


        float combinedPLLeft = (float)currentCombinedPL / (float)maxCombinedPL;
        float pollutionBarValue = pollutionBar.GetComponent<Image>().fillAmount;

        Debug.Log(combinedPLLeft);

        while (pollutionBarValue > combinedPLLeft)
        {
            pollutionBarValue -= 0.01f;
            if (pollutionBarValue < 0)
                pollutionBarValue = 0;

            pollutionBar.GetComponent<Image>().fillAmount = pollutionBarValue;
            yield return new WaitForSeconds(0.15f);
        }

        for (int i = 4; i <= damageTaken; i+=4)
        {
            pollutionValue.text = ((currentCombinedPL + damageTaken) - i).ToString();
            yield return null;
        }

        pollutionValue.text = currentCombinedPL.ToString();
    }
}
