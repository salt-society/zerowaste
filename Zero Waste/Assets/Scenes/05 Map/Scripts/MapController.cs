using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapController : MonoBehaviour
{
    public DataController dataController;
    public TeamSelect teamSelectManager;

    [Space]
    public Camera mainCamera;
    public Camera fxCamera;

    [Space]
    public GameObject infectedAreasSign;
    public TextMeshProUGUI focusedAreaName;
    public TextMeshProUGUI focusedSubname;
    public Button zwaButton;
    public Button zoomOutButton;
    public Button exploreButton;
    public GameObject nodeDetails;
    public GameObject levelList;
    public GameObject teamSelect;
    public GameObject scavengerRoster;
    public GameObject boosterRoster;

    [Space]
    public List<Areas> areaData;
    public List<GameObject> areaMap;
    public List<GameObject> areaHover;
    public List<GameObject> locks;
    public List<GameObject> areaNames;
    public List<GameObject> areaTooltips;

    [Space]
    public GameObject nodePrefab;
    public GameObject pathPrefab;

    [Space]
    public List<GameObject> nodes;
    public List<GameObject> nodeParents;

    private int currentAreaId;
    private int currentNodeId;

    private bool canSelectMap;
    private bool focusArea;
    private bool move;
    private bool focus;
    private bool zoomOut;
    private bool canSelectNode;

    private Vector2 camDefaultPosition;
    private Vector2 destination;

    private Areas currentSelectMapData;
    private string currentSelectedMap;

    private GameObject currentSelectedNode;
    private GameObject currentSelectedBattle;
    
    void Start()
    {
        dataController = GameObject.FindObjectOfType<DataController>();
        if (dataController != null)
        {
            SetDefaultValues();

            Debug.Log("Current Area Id: " + currentAreaId);

            // Check if game's just started
            if (dataController.currentSaveData.currentAreaId == -1)
            {
                dataController.currentSaveData.currentAreaId++;
                currentAreaId = dataController.currentSaveData.currentAreaId;

                dataController.currentSaveData.UnlockArea();

                AssignNodeData();
                StartCoroutine(UnlockMapWithAnimation(areaData[currentAreaId]));
            }
            else
            {
                AssignNodeData();
                UnlockMaps();
            }
        }
    }

    void SetDefaultValues()
    {
        canSelectMap = false;
        focusArea = false;
        move = false;
        focus = false;
        zoomOut = false;
    }

    void AssignNodeData()
    {
        int i = 0;
        foreach (Areas area in areaData)
        {
            GameObject parent = nodeParents[area.areaId];
            List<Node> nodes = area.nodes;

            if (nodes.Count > 0)
            {
                foreach (Node node in nodes)
                {
                    GameObject nodeObj = Instantiate(nodePrefab, parent.transform);
                    nodeObj.transform.localPosition = area.positions[i];

                    if (node.hasPath)
                    {
                        GameObject path = Instantiate(pathPrefab, nodeObj.transform);
                        path.transform.localScale = node.pathScale;
                        path.transform.localPosition = node.pathPosition;
                        path.transform.Rotate(node.pathRotation);

                        path.SetActive(false);
                    }

                    nodeObj.SetActive(false);
                    this.nodes.Add(nodeObj);

                    this.nodes[i].GetComponent<NodeManager>().SetNodeData(Instantiate(node), nodeObj);
                    i++;
                }
            }
            else
            {
                i += area.maxNodes;
            }
        }
    }

    public void UnlockMaps()
    {
        foreach (Areas area in areaData)
        {
            if (area.areaId <= currentAreaId)
            {
                areaMap[area.areaId].GetComponent<BoxCollider2D>().enabled = true;
                locks[area.areaId].SetActive(false);

                areaNames[area.areaId].SetActive(true);
            }  
        }

        canSelectMap = true;
    }

    IEnumerator UnlockMapWithAnimation(Areas area)
    {
        if (currentAreaId == 0)
        {
            yield return new WaitForSeconds(1.5f);
        }

        if (area.areaId <= currentAreaId)
        {
            locks[area.areaId].GetComponent<Animator>().SetBool("Unlock Map", true);
            yield return new WaitForSeconds(1.5f);
            locks[area.areaId].GetComponent<Animator>().SetBool("Unlock Map", false);
            locks[area.areaId].SetActive(false);

            yield return new WaitForSeconds(0.2f);

            areaNames[area.areaId].SetActive(true);
            areaMap[area.areaId].GetComponent<BoxCollider2D>().enabled = true;
            canSelectMap = true;
        }
    }

    public void UnlockNodes()
    {
        foreach (Areas area in areaData)
        {
            if (area.areaId <= currentAreaId)
            {
                foreach (Node node in area.nodes)
                {
                    if (node.nodeId <= currentNodeId)
                    {
                        nodes[node.nodeId].SetActive(true);

                        if (node.hasPath && (node.nodeId < currentNodeId))
                        {
                            Transform pathTransform = nodes[node.nodeId].transform.GetChild(1);
                            GameObject path = pathTransform.gameObject;
                            path.SetActive(true);
                        }

                        if (node.nodeId == currentNodeId)
                            PointCurrentNode(node.nodeId);
                    }
                }

                nodeParents[area.areaId].SetActive(true);
                Debug.Log("Unlocked Nodes");
            }
        }
    }

    public void LockNodes()
    {
        foreach (Areas area in areaData)
        {
            if (area.areaId <= currentAreaId)
            {
                foreach (Node node in area.nodes)
                {
                    if (node.nodeId <= currentNodeId)
                    {
                        nodes[node.nodeId].SetActive(false);

                        if (node.hasPath && (node.nodeId < currentNodeId))
                        {
                            Transform pathTransform = nodes[node.nodeId].transform.GetChild(1);
                            GameObject path = pathTransform.gameObject;
                            path.SetActive(false);
                        }

                        if (node.nodeId == currentNodeId)
                            PointCurrentNode(node.nodeId);
                    }
                }

                nodeParents[area.areaId].SetActive(false);
                Debug.Log("Unlocked Nodes");
            }
        }
    }

    public void PointCurrentNode(int nodeId)
    {
        Transform pointerTransform = nodes[nodeId].transform.GetChild(0);
        pointerTransform.gameObject.SetActive(true);
    }

    public void RemovePointer(int nodeId)
    {
        Transform pointerTransform = nodes[nodeId].transform.GetChild(0);
        pointerTransform.gameObject.SetActive(false);
    }

    public void RemoveAllPointers()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            Transform pointerTransform = nodes[i].transform.GetChild(0);
            pointerTransform.gameObject.SetActive(false);
        }
    }

    IEnumerator UnlockNodeWithAnimation(Node node)
    {
        if (currentNodeId == 0)
        {
            yield return new WaitForSeconds(1.5f);
        }

        if (node.nodeId <= currentNodeId)
        {
            if (node.nodeId != 0)
            {
                Node previousNode = areaData[currentAreaId].nodes[node.nodeId - 1];
                if (previousNode.hasPath)
                {
                    Transform pathTransform = nodes[previousNode.nodeId].transform.GetChild(1);
                    GameObject path = pathTransform.gameObject;
                    path.SetActive(true);

                    RemovePointer(previousNode.nodeId);
                }

                yield return new WaitForSeconds(.5f);
            }

            nodes[node.nodeId].SetActive(true);
            nodeParents[currentAreaId].SetActive(true);

            if (node.nodeId == currentNodeId)
                PointCurrentNode(node.nodeId);

            yield return null;
        }
    }

    void Update()
    {
        // Map Selection through Raycast
        if (canSelectMap)
        {
            if (Input.touchCount == 1)
            {
                if (TouchPhase.Began == Input.GetTouch(0).phase)
                {
                    Vector2 origin = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero);

                    if (hit.collider != null)
                    {
                        Debug.Log(hit.transform.gameObject.name);

                        GameObject obj = hit.transform.gameObject;
                        if (obj.name.Equals("Lock"))
                        {
                            StartCoroutine(ShowAreaTooltip(obj));
                        }

                        if (obj.name.Equals("Terre") || obj.name.Equals("Mare")
                            || obj.name.Equals("Atmos"))
                        {
                            StartCoroutine(ShowAreaThreatLevel(obj));
                        }
                        
                    }
                    else
                    {
                        Debug.Log("No map clicked.");

                        if (!string.IsNullOrEmpty(currentSelectedMap))
                        {
                            StartCoroutine(HideAreaThreatLevel());
                        }
                    }
                }
            }
        }

        // Focus on Area
        if (focusArea)
        {
            if (move)
            {
                Vector2 camPosition = mainCamera.transform.position;
                if (destination.x > 0)
                {
                    if (camPosition.x <= destination.x)
                    {
                        Vector2 offset, direction;

                        if (!zoomOut)
                        {
                            offset = destination - camDefaultPosition; ;
                            direction = Vector2.ClampMagnitude(offset, 1.0f);
                        }
                        else
                        {
                            offset = destination + camDefaultPosition; ;
                            direction = Vector2.ClampMagnitude(offset, 1.0f);
                        }
                        
                        mainCamera.transform.Translate((direction) * currentSelectMapData.moveSpeed * Time.deltaTime);
                        fxCamera.transform.Translate((direction) * currentSelectMapData.moveSpeed * Time.deltaTime);
                    }
                    else
                    {
                        focus = true;
                        move = false;
                    }
                }
                else
                {
                    if (camPosition.x >= destination.x)
                    {
                        Vector2 offset = destination - camDefaultPosition; ;
                        Vector2 direction = Vector2.ClampMagnitude(offset, 1.0f);

                        mainCamera.transform.Translate((direction) * currentSelectMapData.moveSpeed * Time.deltaTime);
                        fxCamera.transform.Translate((direction) * currentSelectMapData.moveSpeed * Time.deltaTime);
                    }
                    else
                    {
                        focus = true;
                        move = false;
                    }
                }
            }

            if (focus)
            {
                float mainCamSize = mainCamera.orthographicSize;
                float fxCamSize = fxCamera.orthographicSize;

                if (!zoomOut)
                {
                    if (mainCamSize >= currentSelectMapData.zoomSize && fxCamSize >= currentSelectMapData.zoomSize)
                    {
                        mainCamera.orthographicSize -= (currentSelectMapData.zoomSpeed * Time.deltaTime);
                        fxCamera.orthographicSize -= (currentSelectMapData.zoomSpeed * Time.deltaTime);
                    }
                    else
                    {
                        focus = false;
                        focusArea = false;

                        ShowFocusedAreaContent();
                    }
                }
                else
                {
                    if (mainCamSize <= 200 && fxCamSize <= 200)
                    {
                        mainCamera.orthographicSize += (currentSelectMapData.zoomSpeed * Time.deltaTime);
                        fxCamera.orthographicSize += (currentSelectMapData.zoomSpeed * Time.deltaTime);
                    }
                    else
                    {
                        focus = false;
                        focusArea = false;
                        zoomOut = false;

                        HideFocusedAreaContent();
                    }
                }
                
            }
        }
    }

    IEnumerator ShowAreaTooltip(GameObject obj)
    {
        Debug.Log(obj.transform.parent.gameObject.name);
        foreach (Areas area in areaData)
        {
            if (area.areaName.Equals(obj.transform.parent.gameObject.name))
            {
                areaTooltips[area.areaId].SetActive(true);
                yield return new WaitForSeconds(2f);
                areaTooltips[area.areaId].SetActive(false);
                break;
            }
        }
    }

    IEnumerator ShowAreaTooltip(int areaId)
    {
        areaTooltips[areaId].SetActive(true);
        yield return new WaitForSeconds(2f);
        areaTooltips[areaId].SetActive(false);
    }

    IEnumerator ShowAreaThreatLevel(GameObject obj)
    {
        if (!string.IsNullOrEmpty(currentSelectedMap))
        {
            StartCoroutine(HideAreaThreatLevel());
        }

        yield return new WaitForSeconds(.5f);

        foreach (Areas area in areaData)
        {
            if(area.areaName.Equals(obj.transform.gameObject.name))
            {
                areaHover[area.areaId].SetActive(true);
                exploreButton.gameObject.SetActive(true);

                TextMeshProUGUI tooltipText = areaTooltips[area.areaId].transform.GetChild(0)
                    .gameObject.GetComponent<TextMeshProUGUI>();
                tooltipText.text = "Doctor Cooper allowed this area to be explored. " +
                    " Still, be careful Scavenger!";
                StartCoroutine(ShowAreaTooltip(area.areaId));
                
                currentSelectedMap = area.areaName;
                currentSelectMapData = area;
                break;
            }
                
        }
    }

    IEnumerator HideAreaThreatLevel()
    {
        foreach (Areas area in areaData)
        {
            if (area.areaName.Equals(currentSelectedMap))
            {
                areaHover[area.areaId].GetComponent<Animator>().SetBool("Fade Out", true);
                exploreButton.gameObject.GetComponent<Animator>().SetBool("Exit", true);
                yield return new WaitForSeconds(.2f);
                areaHover[area.areaId].GetComponent<Animator>().SetBool("Fade Out", false);
                exploreButton.gameObject.GetComponent<Animator>().SetBool("Exit", false);
                areaHover[area.areaId].SetActive(false);
                exploreButton.gameObject.SetActive(false);

                currentSelectedMap = string.Empty;
                break;
            }
        }
    }

    private void ExploreMap()
    {
        camDefaultPosition = mainCamera.transform.position;
        destination = currentSelectMapData.coordinates;

        canSelectMap = false;
        focusArea = true;
        move = true;

        infectedAreasSign.GetComponent<Animator>().SetBool("Fade Out", true);
        zwaButton.gameObject.SetActive(false);

        foreach (GameObject areaName in areaNames)
            areaName.SetActive(false);
    }

    private void ZoomOutOfMap()
    {
        StartCoroutine(ZoomOutAnimation());
    }

    IEnumerator ZoomOutAnimation()
    {
        for (int i = 0; i <= currentNodeId; i++)
            StartCoroutine(nodes[i].GetComponent<NodeManager>().HideNode());   
            
        yield return new WaitForSeconds(1f);

        camDefaultPosition = mainCamera.transform.position;
        destination = new Vector3(0, 0, -10);

        canSelectMap = true;
        focusArea = true;
        move = true;
        zoomOut = true;

        yield return new WaitForSeconds(1f);
        areaNames[currentSelectMapData.areaId].SetActive(true);
    }

    private void ShowFocusedAreaContent()
    {
        infectedAreasSign.SetActive(false);
        zoomOutButton.gameObject.SetActive(true);

        focusedAreaName.text = currentSelectMapData.areaName;
        focusedAreaName.gameObject.SetActive(true);

        focusedSubname.text = currentSelectMapData.subtitle;
        focusedSubname.gameObject.SetActive(true);

        if(dataController != null) 
        {
            Debug.Log(dataController.currentSaveData.currentNodeId);
            if (dataController.currentSaveData.currentNodeId == -1)
            {
                dataController.currentSaveData.currentNodeId++;
                currentNodeId = dataController.currentSaveData.currentNodeId;

                StartCoroutine(UnlockNodeWithAnimation(currentSelectMapData.nodes[currentNodeId]));
            }
            else
            {
                UnlockNodes();
            }
        }
        else
        {
            UnlockNodes();
        }

        foreach (GameObject node in nodes)
        {
            node.GetComponent<NodeManager>().SetMapController(this);
            node.GetComponent<NodeManager>().AllowNodeSelection(1);
            node.GetComponent<NodeManager>().SetPreviousBattleData(currentSelectMapData.nodes[currentNodeId], nodes[currentNodeId]);
            node.GetComponent<NodeManager>().SendAreaNameComponents(focusedAreaName, focusedSubname);
            node.GetComponent<NodeManager>().SendNodeDetailComponents(nodeDetails);
            node.GetComponent<NodeManager>().SendLevelListComponents(levelList);
        }
            
    }

    private void HideFocusedAreaContent()
    {
        LockNodes();

        zoomOutButton.gameObject.SetActive(false);
        focusedAreaName.gameObject.SetActive(false);
        focusedSubname.gameObject.SetActive(false);
        infectedAreasSign.SetActive(true);
        zwaButton.gameObject.SetActive(true);
        infectedAreasSign.SetActive(true);
    }

    public void SetCurrentSelectedNode(GameObject currentSelectedNode)
    {
        this.currentSelectedNode = currentSelectedNode;
    }

    public void SetCurrentSelectedBattle(GameObject currentSelectedBattle)
    {
        this.currentSelectedBattle = currentSelectedBattle;
    }

    public void EnableNodeColliders(bool enabled)
    {
        foreach (Node node in currentSelectMapData.nodes)
            nodes[node.nodeId].GetComponent<BoxCollider2D>().enabled = enabled;
    }

    public void HideListOfLevels()
    {
        StartCoroutine(currentSelectedNode.GetComponent<NodeManager>().HideListOfLevels());
    }

    public void RetreatFromNode()
    {
        StartCoroutine(currentSelectedBattle.GetComponent<LevelManager>().HideBattleDetails());
    }

    public void HuntNode()
    {
        StartCoroutine(ShowTeamSelect());
    }

    IEnumerator ShowTeamSelect()
    {
        RetreatFromNode();
        HideListOfLevels();

        yield return new WaitForSeconds(2f);

        teamSelect.SetActive(true);
        EnableNodeColliders(false);

        StartCoroutine(teamSelectManager.SetDefaultScavengers());
    }

    public void CancelTeamSelection()
    {
        StartCoroutine(HideTeamSelect());
    }

    IEnumerator HideTeamSelect()
    {
        EnableNodeColliders(true);

        teamSelect.GetComponent<Animator>().SetBool("Exit", true);
        yield return new WaitForSeconds(2f);
        teamSelect.GetComponent<Animator>().SetBool("Exit", false);
        teamSelect.SetActive(false);
    }
}
