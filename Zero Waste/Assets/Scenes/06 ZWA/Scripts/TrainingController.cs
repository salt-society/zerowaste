using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainingController : MonoBehaviour
{
    #region Editor Variables

    [Header("UI Components")]

    public List<GameObject> partHighlights;
    public List<GameObject> partNames;

    [Header("Training Screen Components")]
    public GameObject trainingScreen;
    public Image scavengerHolder;
    public GameObject scrollview;
    public GameObject scavengerPrefab;

    [Header("Upgrade Screen Components")]
    public GameObject upgradeScreen;

    #endregion

    #region Private Variables

    private DataController dataController;

    private Player currentPlayer;

    private int neededScraps;

    private int levelBreak;

    #endregion

    IEnumerator DisplayPartNames()
    {
        foreach (GameObject tooltip in partNames)
            tooltip.SetActive(!tooltip.activeInHierarchy);

        yield return new WaitForSeconds(1.0f);

        foreach (GameObject tooltip in partNames)
            tooltip.GetComponent<Animator>().SetBool("Hide", true);

        yield return new WaitForSeconds(1.0f);

        foreach (GameObject tooltip in partNames)
        {
            tooltip.SetActive(!tooltip.activeInHierarchy);
            tooltip.GetComponent<Animator>().SetBool("Hide", false);
        }
    }

    public void SetSelectedScavenger(Player selectedPlayer)
    {
        currentPlayer = selectedPlayer;

        trainingScreen.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        trainingScreen.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
        trainingScreen.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite = currentPlayer.characterFull;
        upgradeScreen.SetActive(true);

        SetupUpgradeScreen();
    }

    public void PerformBreak()
    {
        if(dataController.currentSaveData.scraps < neededScraps)
        {
            upgradeScreen.transform.GetChild(6).gameObject.SetActive(true);
            upgradeScreen.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = "INSUFFICIENT SCRAPS";
            upgradeScreen.transform.GetChild(8).GetComponent<Button>().interactable = false;
        }

        else
        {
            dataController.UseScrap(neededScraps);

            // Next adjust player values
            if (levelBreak == 1)
            {
                currentPlayer.currentLevelCap = 20;
                currentPlayer.abilities[2] = currentPlayer.UpgradedCA;
            }

            else if (levelBreak == 2)
            {
                currentPlayer.currentLevelCap = 30;
                currentPlayer.abilities[3] = currentPlayer.UpgradedUA;
            }

            dataController.SaveSaveData();
            dataController.SaveGameData();

            SetupUpgradeScreen();
        }
    }

    public void ShowTrainingScreen()
    {
        trainingScreen.SetActive(true);
        trainingScreen.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        trainingScreen.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
        ShowRoster();
    }

    public void HideTrainingScreen()
    {
        trainingScreen.SetActive(false);
    }

    public void HideUpgradeScreen()
    {
        upgradeScreen.SetActive(false);
        currentPlayer = null;

        trainingScreen.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        trainingScreen.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
    }

    private void ShowRoster()
    {
        List<Player> roster = dataController.scavengerRoster;

        ClearRoster();

        for (int CTR = 0; CTR < roster.Count; CTR++)
        {
            GameObject instancePrefab = Instantiate(scavengerPrefab, scrollview.transform);
            instancePrefab.GetComponent<ScavengerInstanceHandler>().SetPlayer(roster[CTR]);
        }
    }

    private void ClearRoster()
    {
        if (scrollview.transform.childCount > 0)
        {
            foreach (Transform child in scrollview.transform)
                Destroy(child.gameObject);
        }
    }

    private void SetupUpgradeScreen()
    {
        neededScraps = 0;
        levelBreak = 0;

        upgradeScreen.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentPlayer.currentLevel.ToString();
        upgradeScreen.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = currentPlayer.currentLevelCap.ToString();
        upgradeScreen.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = (currentPlayer.currentLevelCap + 10).ToString();

        if(currentPlayer.currentLevel < currentPlayer.currentLevelCap)
        {
            upgradeScreen.transform.GetChild(6).gameObject.SetActive(true);
            upgradeScreen.transform.GetChild(7).gameObject.SetActive(true);
            upgradeScreen.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = "MUST BE LEVEL " + currentPlayer.currentLevelCap + " TO PERFORM BREAK";
            upgradeScreen.transform.GetChild(8).GetComponent<Button>().interactable = false;
        }

        else
        {
            upgradeScreen.transform.GetChild(6).gameObject.SetActive(false);
            upgradeScreen.transform.GetChild(7).gameObject.SetActive(false);

            if (currentPlayer.currentLevelCap == 10)
            {
                neededScraps = 200;
                levelBreak = 1;
            }

            else if (currentPlayer.currentLevelCap == 20)
            {
                neededScraps = 500;
                levelBreak = 2;
            }

            upgradeScreen.transform.GetChild(8).GetChild(0).GetComponent<TextMeshProUGUI>().text = neededScraps.ToString();
            upgradeScreen.transform.GetChild(8).GetComponent<Button>().interactable = true;
        }
    }

    private void Awake()
    {
        dataController = FindObjectOfType<DataController>();

        currentPlayer = null;
    }

    private void Start()
    {
        StartCoroutine(DisplayPartNames());
    }
}
