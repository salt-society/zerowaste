using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingFunctions : MonoBehaviour
{
    [Header("Necessities")]
    public GameObject upgradePanel;

    private DataController myController;

    // Variable to hold scraps needed to break
    private int neededScraps;

    // Variable to hold what number of break is available
    private int levelBreak;

    private void Start()
    {
        myController = FindObjectOfType<DataController>();
    }

    // Show scavenger roster
    public List<Player> DisplayCurrentRoster()
    {
        return myController.scavengerRoster;
    }

    // Display the selected scavenger after he / she is selected from the scavenger list
    public void DisplayScavenger(Player selectedScavenger)
    {
        neededScraps = 0;
        levelBreak = 0;

        // Display the name here
        // upgradePanel.name.text = selectedScavenger.name;

        // Display his / her current level here
        // upgradePanel.currentLevel.text = selectedScavenger.currentLevel;

        // Display his / her current level cap here
        // upgradePanel.currentLevelCap.text = selectedScavenger.currentLevelCap;

        // Display the scraps needed here
        if (selectedScavenger.currentLevelCap == 10)
        {
            neededScraps = 200;
            levelBreak = 1;
            // upgradePanel.scrapsNeeded.text = neededScraps;
        }

        else if (selectedScavenger.currentLevelCap == 20)
        {
            neededScraps = 500;
            levelBreak = 2;
            // upgradePanel.scrapsNeeded.text = neededScraps;
        }

        else if (selectedScavenger.currentLevelCap == 30)
        {
            // upgradePanel.scrapsNeeded.text = "N/A";
            return;
        }

        if (selectedScavenger.currentLevel <= selectedScavenger.currentLevelCap)
        {
            // Insufficient Level
            // upgradePanel.levelNote.text = selectedScavenger.name + "'s level is not enough!"
            // break.disable;
        }

        if (myController.currentSaveData.scraps < neededScraps)
        {
            // Insufficient Scrap
            // upgradePanel.scrapNote.text = "You have insufficient scraps!";
            // break.disable;
        }
    }

    // Function for when break is activated
    public void BreakScavenger(Player selectedScavenger)
    {
        // Only arrive here if sure that all requirements are met
        // First reduce scraps
        myController.UseScrap(neededScraps);

        // Next adjust player values
        if(levelBreak == 1)
        {
            selectedScavenger.currentLevelCap = 20;
            selectedScavenger.abilities[2] = selectedScavenger.UpgradedCA;
        }

        else if(levelBreak == 2)
        {
            selectedScavenger.currentLevelCap = 30;
            selectedScavenger.abilities[3] = selectedScavenger.UpgradedUA;
        }

        // Do something here if you want animations

        // Save data

        // Close the screens
        
    }
}
