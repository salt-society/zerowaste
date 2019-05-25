using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TeamSelectHandler : MonoBehaviour
{
    #region Editor Variables

    [Header("UI Components")]

    public GameObject levelHandler;

    [Space]

    public List<GameObject> scavengerHolders;
    public List<GameObject> addButtons;

    [Space]
    public Image tooltip;

    #endregion

    #region Private Variables

    private DataController dataController;

    private List<Player> playerAtSlots;

    #endregion

    // Setup the Team Select Screen
    public void SetupTeamSelect()
    {

        Player scavenger = dataController.scavengerRoster[0];

        scavengerHolders[0].transform.GetChild(0).GetComponent<Image>().sprite = scavenger.characterHalf;
        scavengerHolders[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Level " + scavenger.currentLevel.ToString();
        scavengerHolders[0].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = scavenger.name;
        scavengerHolders[0].transform.GetChild(3).GetComponent<Image>().sprite = scavenger.characterClass.roleLogo;


        if(dataController.currentSaveData.lastPartyUsed.Count >= 1)
        {
            int CTR = 1;
            
            while(CTR < dataController.currentSaveData.lastPartyUsed.Count)
            {
                scavengerHolders[CTR].transform.GetChild(0).GetComponent<Image>().sprite = scavenger.characterHalf;
                scavengerHolders[CTR].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = scavenger.currentLevel.ToString();
                scavengerHolders[CTR].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = scavenger.name;
                scavengerHolders[CTR].transform.GetChild(3).GetComponent<Image>().sprite = scavenger.characterClass.roleLogo;
                CTR++;
            }
        }
    }

    // Distinguish between what control was clicked and do appropriate stuff
    public void ButtonClicked()
    {
        // Get the button name
        string name = EventSystem.current.currentSelectedGameObject.name;

        LevelHandler handler = levelHandler.GetComponent<LevelHandler>();

        switch(name)
        {
            case "Scavenger 01":
                StartCoroutine(TooltipAnimation());
                break;

            case "Scavenger 02":
                handler.OpenScavengerSelect(1);
                break;

            case "Scavenger 03":
                handler.OpenScavengerSelect(2);
                break;

            case "Add 02":
                handler.OpenScavengerSelect(1);
                break;

            case "Add 03":
                handler.OpenScavengerSelect(2);
                break;
        }
    }

    // Add the selected scavenger to the team
    public void AddScavenger(int chosenSlot, Player selectedPlayer)
    {
        if(playerAtSlots.Contains(selectedPlayer))
        {
            int previousIndex = playerAtSlots.IndexOf(selectedPlayer);

            if (previousIndex == chosenSlot)
                return;

            playerAtSlots[previousIndex] = null;
            
            if(previousIndex == 1)
            {
                if(playerAtSlots[2] != null)
                {
                    playerAtSlots[1] = playerAtSlots[2];
                    scavengerHolders[1].GetComponent<SelectHandler>().SetPlayer(playerAtSlots[1]);
                    scavengerHolders[1].SetActive(true);
                    addButtons[0].SetActive(false);

                    playerAtSlots[2] = selectedPlayer;
                    scavengerHolders[2].GetComponent<SelectHandler>().SetPlayer(selectedPlayer);
                    scavengerHolders[2].SetActive(true);
                    addButtons[1].SetActive(false);
                }

                else
                {
                    playerAtSlots[2] = selectedPlayer;
                    scavengerHolders[2].GetComponent<SelectHandler>().SetPlayer(selectedPlayer);
                    scavengerHolders[2].SetActive(true);
                    addButtons[1].SetActive(false);

                    scavengerHolders[1].SetActive(false);
                    addButtons[0].SetActive(true);
                }
            }

            else if(previousIndex == 2)
            {
                if (playerAtSlots[1] != null)
                {
                    playerAtSlots[2] = playerAtSlots[1];
                    scavengerHolders[2].GetComponent<SelectHandler>().SetPlayer(playerAtSlots[2]);
                    scavengerHolders[2].SetActive(true);
                    addButtons[1].SetActive(false);

                    playerAtSlots[1] = selectedPlayer;
                    scavengerHolders[1].GetComponent<SelectHandler>().SetPlayer(selectedPlayer);
                    scavengerHolders[1].SetActive(true);
                    addButtons[0].SetActive(false);
                }

                else
                {
                    playerAtSlots[1] = selectedPlayer;
                    scavengerHolders[1].GetComponent<SelectHandler>().SetPlayer(selectedPlayer);
                    scavengerHolders[1].SetActive(true);
                    addButtons[0].SetActive(false);

                    scavengerHolders[2].SetActive(false);
                    addButtons[1].SetActive(true);
                }
            }
        }

        else
        {
            playerAtSlots[chosenSlot] = selectedPlayer;
            scavengerHolders[chosenSlot].GetComponent<SelectHandler>().SetPlayer(selectedPlayer);
            scavengerHolders[chosenSlot].SetActive(true);
            addButtons[chosenSlot - 1].SetActive(false);
        }
    }

    // Send data to level handler and go to battle
    public void SendDataToLevelHandler()
    {
        levelHandler.GetComponent<LevelHandler>().GoToBattle(playerAtSlots);
    }

    // Coroutine to show and hide the tooltip
    private IEnumerator TooltipAnimation()
    {
        tooltip.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        tooltip.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerAtSlots = new List<Player>();

        playerAtSlots.Add(dataController.scavengerRoster[0]);
        scavengerHolders[0].GetComponent<SelectHandler>().SetPlayer(playerAtSlots[0]);

        playerAtSlots.Add(null);
        playerAtSlots.Add(null);
    }

    // Find the data controller
    private void Awake()
    {
        dataController = FindObjectOfType<DataController>();
    }
}
