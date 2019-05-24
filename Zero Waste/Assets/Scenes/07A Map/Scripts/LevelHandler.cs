using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelHandler : MonoBehaviour
{
    #region Editor Variables

    [Header("UI Components")]
    public Image overlay;

    [Space]
    public GameObject nodeScreen;
    public GameObject teamSelectScreen;
    public GameObject rosterScreen;

    #endregion

    #region Private Variables

    private DataController dataController;

    private Node node;

    #endregion

    // Setup the Screen with the selected level
    public void SetupLevel(Areas area)
    {
        nodeScreen.SetActive(true);
        nodeScreen.GetComponent<NodeHandler>().SetupNodes(area);
    }

    // Set the selected node
    public void SetSelectedNode(Node selectedNode)
    {
        node = selectedNode;
    }

    // Empty the selected node
    public void EmptySelectedNode()
    {
        node = null;
    }

    // Close the whole node screen
    public void CloseThis()
    {
        this.gameObject.SetActive(false);
    }

    // Overload to get back to level up screen
    public void SetupLevel()
    {
        nodeScreen.SetActive(true);
    }

    // Close the screen on click
    public void CloseLevel()
    {
        nodeScreen.SetActive(false);
    }

    // Open the Team Select Screen
    public void SetupTeamSelect()
    {
        teamSelectScreen.SetActive(true);
        teamSelectScreen.GetComponent<TeamSelectHandler>().SetupTeamSelect();
    }

    // Close the Team Select Screen
    public void CloseTeamSelect()
    {
        teamSelectScreen.SetActive(false);
    }

    // Open the scavenger select screen
    public void OpenScavengerSelect(int chosenSlot)
    {
        rosterScreen.SetActive(true);
        rosterScreen.GetComponent<RosterHandler>().ShowRoster(chosenSlot);
    }

    // Close the scavenger select screen
    public void CloseScavengerSelect()
    {
        rosterScreen.SetActive(false);
    }

    // Change the image and the overlay
    public void ChangeOverlay(Sprite nodeBackground)
    {
        gameObject.GetComponent<Image>().sprite = nodeBackground;
        overlay.GetComponent<Image>().sprite = nodeBackground;
    }

    // Go to battle with the necessary information
    public void GoToBattle(List<Player> playerTeam)
    {
        dataController.currentNode = node;
        dataController.scavengerTeam = playerTeam.ToArray();
        dataController.mutantTeam = node.wastePool.SelectWasteFromPool();

        int nextScene = dataController.GetNextSceneId("Battle");
        SceneManager.LoadScene(nextScene);
    }

    private void Awake()
    {
        dataController = FindObjectOfType<DataController>();
    }
}
