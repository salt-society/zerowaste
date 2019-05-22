using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class NodeHandler : MonoBehaviour
{
    #region Editor Variables

    [Header("UI Components")]
    public Image nodeIcon;
    public GameObject nodeName;
    public GameObject nodeDescription;
    public GameObject clearedIcon;
    public GameObject mapIcon;

    [Space]
    public Image[] poolImages;

    [Header("Level Handler")]
    public GameObject levelHandler;

    #endregion

    #region Private Variables

    private DataController dataController;

    private Areas area;

    private List<Node> unlockedNodes;

    private int nodeIndex;

    private int currentNode;

    #endregion

    public void SetupNodes(Areas selectedArea)
    {
        area = selectedArea;

        // Find out which nodes are unlocked
        currentNode = dataController.currentSaveData.currentNodeId;

        // Change the selected area icon and the name
        mapIcon.GetComponent<Image>().sprite = area.sprite;
        mapIcon.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = area.areaName;

        // Reset the unlocked Nodes
        unlockedNodes.Clear();

        foreach(Node node in area.nodes)
        {
            if(node.nodeId <= currentNode)
                unlockedNodes.Add(node);
        }

        nodeIndex = 0;

        SetupCurrentNode();
    }

    // Change index based on right or left button
    public void ChangeNodeIndex()
    {
        // Get the button name
        string name = EventSystem.current.currentSelectedGameObject.name;

        if(name == "Right Button")
        {
            if (nodeIndex == unlockedNodes.Count - 1)
                nodeIndex = 0;

            else
                nodeIndex++;
        }

        else if(name == "Left Button")
        {
            if (nodeIndex == 0)
                nodeIndex = unlockedNodes.Count - 1;

            else
                nodeIndex--;
        }

        SetupCurrentNode();
    }

    // Send Selected Node to levelHandler
    public void SendSelectedNode()
    {
        levelHandler.GetComponent<LevelHandler>().SetSelectedNode(unlockedNodes[nodeIndex]);
    }

    // Empty Selected Node just to be safe
    public void EmptySelectedNode()
    {
        levelHandler.GetComponent<LevelHandler>().EmptySelectedNode();
    }

    // Function for setting up the current Node using nodeIndex
    private void SetupCurrentNode()
    {
        // Setup the image of the overlays
        levelHandler.GetComponent<LevelHandler>().ChangeOverlay(unlockedNodes[nodeIndex].nodeBackground);

        nodeIcon.sprite = unlockedNodes[nodeIndex].nodeIcon;
        nodeName.GetComponent<TextMeshProUGUI>().text = unlockedNodes[nodeIndex].nodeName;
        nodeDescription.GetComponent<TextMeshProUGUI>().text = unlockedNodes[nodeIndex].info;

        if (unlockedNodes[nodeIndex].nodeId < currentNode)
            clearedIcon.SetActive(true);

        else
            clearedIcon.SetActive(false);

        if(unlockedNodes[nodeIndex].wastePool.wastePool.Length > 0)
        {
            int CTR = 0;
            foreach(Enemy enemy in unlockedNodes[nodeIndex].wastePool.wastePool)
            {
                poolImages[CTR].sprite = enemy.characterThumb;

                if (unlockedNodes[nodeIndex].nodeId < currentNode)
                    poolImages[CTR].color = Color.white;

                else
                    poolImages[CTR].color = Color.black;

                CTR++;

                if (CTR == 3)
                    break;
            }

            if(CTR != 3)
            {
                while(CTR != 3)
                {
                    poolImages[CTR].color = Color.black;
                    CTR++;
                }
            }
        }
    }

    // Find the data controller
    private void Awake()
    {
        dataController = FindObjectOfType<DataController>();

        unlockedNodes = new List<Node>();
    }
}
