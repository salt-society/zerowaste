using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeHandler : MonoBehaviour
{
    #region Editor Variables

    [Header("UI Components")]
    public Image nodeIcon;
    public GameObject nodeName;
    public GameObject nodeDescription;
    public GameObject clearedIcon;

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

        foreach(Node node in area.nodes)
        {
            if(node.nodeId <= currentNode)
                unlockedNodes.Add(node);
        }

        nodeIndex = 0;

        SetupCurrentNode();
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
    }

    // Find the data controller
    private void Awake()
    {
        dataController = FindObjectOfType<DataController>();

        unlockedNodes = new List<Node>();
    }
}
