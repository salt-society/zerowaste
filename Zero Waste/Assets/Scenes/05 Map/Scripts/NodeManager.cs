using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeManager : MonoBehaviour
{
    public DataController dataController;

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
        if (canSelectNode)
        {
            if (Input.touchCount == 1)
            {
                if (TouchPhase.Began == Input.GetTouch(0).phase)
                {
                    Vector2 origin = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero);

                    if (hit.collider != null)
                    {
                        GameObject obj = hit.transform.gameObject;
                        
                        if (obj.Equals(node))
                        {
                            GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 3");

                            if (dataController != null)
                            {
                                dataController.currentNode = nodeData;
                                dataController.currentNodeObject = gameObject;
                            }

                            canSelectNode = false;
                            
                            mapController.EnableNodeColliders(false);
                            mapController.RemoveAllPointers();

                            StartCoroutine(ShowListOfLevels());
                        }
                    }
                }
            }
        }
    }

    public IEnumerator ShowListOfLevels()
    {
        LevelList levelListManager = levelList.transform.GetChild(3).transform.GetChild(1).transform.
            GetChild(0).transform.GetChild(0).GetComponent<LevelList>();
        levelListManager.gameObject.SetActive(true);
        levelListManager.nodeData = nodeData;
        levelListManager.SendScripts(mapController, this);
        levelListManager.SendBattleComponents(nodeDetails, levelList);
        levelListManager.PopulateGrid();

        levelList.transform.GetChild(2).transform.GetChild(0).
            GetComponent<TextMeshProUGUI>().text = nodeData.nodeName;

        focusedAreaName.GetComponent<Animator>().SetBool("Fade Out", true);
        focusedSubname.GetComponent<Animator>().SetBool("Fade Out", true);

        yield return new WaitForSeconds(.5f);

        focusedAreaName.gameObject.SetActive(false);
        focusedSubname.gameObject.SetActive(false);
        focusedAreaName.GetComponent<Animator>().SetBool("Fade Out", false);
        focusedSubname.GetComponent<Animator>().SetBool("Fade Out", false);

        RemovePointer(previousNode);
        yield return new WaitForSeconds(.2f);
        PointCurrentNode(node);

        levelList.SetActive(true);

        StartCoroutine(levelListManager.UnlockLevel());
    }

    public IEnumerator HideListOfLevels()
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
