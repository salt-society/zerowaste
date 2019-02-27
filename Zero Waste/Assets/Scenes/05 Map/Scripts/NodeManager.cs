using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeManager : MonoBehaviour
{
    public DataController dataController;
    public LevelList levelListManager;

    [Space]
    private Node nodeData;
    private Node previousNodeData;

    private GameObject node, previousNode;
    private MapController mapController;

    private TextMeshProUGUI focusedAreaName;
    private TextMeshProUGUI focusedSubname;

    private GameObject nodeDetails;
    private GameObject levelList;

    private bool canSelectNode;

    void Start()
    {
        dataController = GameObject.FindObjectOfType<DataController>();
    }

    public void SetNodeData(Node nodeData, GameObject node)
    {
        this.nodeData = nodeData;
        this.node = node;
    }

    public void SetPreviousNodeData(Node previousNodeData, GameObject previousNode)
    {
        this.previousNodeData = previousNodeData;
        this.previousNode = previousNode;
    }

    public void SetMapController(MapController mapController)
    {
        this.mapController = mapController;
        this.levelListManager = mapController.levelListManager;
    }

    public void SendAreaNameComponents(TextMeshProUGUI focusedAreaName, TextMeshProUGUI focusedSubname)
    {
        this.focusedAreaName = focusedAreaName;
        this.focusedSubname = focusedSubname;
    }

    public void SendNodeDetailComponents(GameObject nodeDetails)
    {
        this.nodeDetails = nodeDetails;
    }

    public void SendLevelListComponents(GameObject levelList)
    {
        this.levelList = levelList;
    }

    public void AllowNodeSelection(int allow)
    {
        canSelectNode = (allow > 0) ? true : false;
    }

    void Update()
    {
        // Just to be safe, always check if there's a data controller before execution
        if (dataController == null)
            return;

        // Prevents unecessary click that may cause errors
        // Only executes when map is zoomed into an area and everything is set up
        if (canSelectNode)
        {
            // Enable node colliders
            mapController.EnableNodeColliders(true);

            // Only register click/touch when its only one
            // and at beggining of the touch phase
            if (Input.touchCount == 1 && (TouchPhase.Began == Input.GetTouch(0).phase))
            {
                // Convert touch position to world point
                Vector2 origin = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                // Get whatever is hit/collided by touch
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero);

                // If hit collided with something and is not empty, check if its a node
                if (hit.collider != null)
                {
                    if (hit.transform.gameObject.Equals(node))
                    {
                        // Play node select SFX
                        GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 3");

                        // Pass current node details to data controller
                        dataController.currentSaveData.currentNodeId = nodeData.nodeId;
                        dataController.currentNode = nodeData;
                        dataController.currentNodeObject = gameObject;

                        dataController.SaveSaveData();
                        dataController.SaveGameData();
      
                        // Since a node is already selected, prevent player from selecting more
                        canSelectNode = false;
                        mapController.EnableNodeColliders(false);

                        // Remove all pointers to start fresh and point to
                        // current selected node
                        mapController.RemoveAllPointers();

                        // Finally, show list of battles in node
                        StartCoroutine(ShowListOfBattles());
                    }
                }
            }
        }
    }

    public IEnumerator ShowListOfBattles()
    {
        Debug.Log("Populating battle list.");

        // Get the list manager from levelList gameObject
        LevelList levelListManager = levelList.transform.GetChild(3).transform.GetChild(1).transform.
            GetChild(0).transform.GetChild(0).GetComponent<LevelList>();

        // Send node details to levelList
        // levelList is a gameObject that is inside a grid view group
        // Script dynamically creates game object in grids
        // to show levels or battles inside one node
        levelListManager.gameObject.SetActive(true);
        levelListManager.SetNodeData(nodeData);
        levelListManager.SendScripts(mapController, this);
        levelListManager.SendBattleComponents(nodeDetails, levelList);
        levelListManager.PopulateGrid();

        // Set node name
        levelList.transform.GetChild(2).transform.GetChild(0).
            GetComponent<TextMeshProUGUI>().text = nodeData.nodeName;

        // Hide focused area name
        focusedAreaName.GetComponent<Animator>().SetBool("Fade Out", true);
        focusedSubname.GetComponent<Animator>().SetBool("Fade Out", true);

        // Delay
        yield return new WaitForSeconds(.5f);

        // Deactivate focused area name and subtitle
        // The reason these objects needs to be deactivated is
        // because for animation to play once awake
        focusedAreaName.gameObject.SetActive(false);
        focusedSubname.gameObject.SetActive(false);
        focusedAreaName.GetComponent<Animator>().SetBool("Fade Out", false);
        focusedSubname.GetComponent<Animator>().SetBool("Fade Out", false);

        // Remove pointer from previous node and point to current
        RemovePointer(previousNode);
        yield return new WaitForSeconds(.2f);
        PointCurrentNode(node);

        // Show levelList gameObject and start unlocking battles
        // if there's any battle to unlock
        levelList.SetActive(true);
        StartCoroutine(levelListManager.UnlockBattles());
    }

    public IEnumerator HideListOfBattles()
    {
        canSelectNode = true;
        mapController.EnableNodeColliders(true);

        levelList.transform.GetChild(3).transform.GetChild(1).transform.
            GetChild(0).transform.GetChild(0).GetComponent<LevelList>().RemoveCellsfFromGrid();
        levelList.GetComponent<Animator>().SetBool("Exit", true);
        yield return new WaitForSeconds(1f);
        levelList.GetComponent<Animator>().SetBool("Exit", false);
        levelList.SetActive(false);

        focusedAreaName.gameObject.SetActive(true);
        focusedSubname.gameObject.SetActive(true);
    }

    public void PointCurrentNode(GameObject node)
    {
        Transform pointerTransform = node.transform.GetChild(0);
        pointerTransform.gameObject.SetActive(true);
    }

    public void RemovePointer(GameObject node)
    {
        Transform pointerTransform = node.transform.GetChild(0);
        pointerTransform.gameObject.SetActive(false);
    }

    public IEnumerator HideNode()
    {
        node.transform.GetChild(0).gameObject.SetActive(false);
        if (nodeData.hasPath)
        {
            if (node.transform.GetChild(1).gameObject.activeInHierarchy)
                node.transform.GetChild(1).gameObject.GetComponent<Animator>().SetBool("Lock Path", true);
        }

        node.GetComponent<Animator>().SetBool("Fade Out", true);

       yield return new WaitForSeconds(1f);

        if (nodeData.hasPath)
        {
            if (node.transform.GetChild(1).gameObject.activeInHierarchy)
            {
                node.transform.GetChild(1).gameObject.GetComponent<Animator>().SetBool("Lock Path", false);
                node.transform.GetChild(1).gameObject.SetActive(false);
            }
        }

        node.GetComponent<Animator>().SetBool("Fade Out", false);
        node.SetActive(false);
    }
}
