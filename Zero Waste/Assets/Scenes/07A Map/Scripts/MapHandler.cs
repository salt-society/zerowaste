using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapHandler : MonoBehaviour
{
    #region Editor Variables

    [Header("UI Components")]
    // Holds the images of the map
    public Image[] areaImages;

    public Image levelSelectScreen;

    [Header("Scriptable Objects")]
    // Holds the areas scriptable objects
    public Areas[] areas;

    #endregion

    #region Private Variables
    // Global Variable to access the data controller
    private DataController dataController;

    // Necessary variables

    // This is to hold the area that is selected or the user has come from
    private Areas area;

    // This is to hold the node that is selected or the user has come from
    private Node node;

    // This is to hold the most latest level the user is on
    private int currentNode;
    #endregion

    // Set map through user click
    public void SetMap(Areas selectedArea)
    {
        area = selectedArea;

        SetupNodes();
    }

    // Enable the Node Panel and set the necessary details
    public void SetupNodes()
    {
        levelSelectScreen.gameObject.SetActive(true);
        levelSelectScreen.GetComponent<LevelHandler>().SetupLevel(area);
    }

    // First find data controller when object is awake
    private void Awake()
    {
        dataController = FindObjectOfType<DataController>();
    }

    // Find out which areas are unlocked and do something about it
    private void Start()
    {
        // Set current node for system to know where the user currently is
        currentNode = dataController.currentSaveData.currentNodeId;

        // Just an index
        int CTR = 0;
        // Unlock the areas which are <= to the currentNode
        foreach(Areas unlockableArea in areas)
        {
            if(unlockableArea.areaId <= currentNode)
            {
                areaImages[CTR].GetComponent<Button>().interactable = true;
                areaImages[CTR].transform.GetChild(0).gameObject.SetActive(false);
                areaImages[CTR].transform.GetChild(1).gameObject.SetActive(true);
            }
                
            CTR++;
        }
    }
}
