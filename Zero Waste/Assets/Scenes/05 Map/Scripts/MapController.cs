using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public DataController dataController;
    public TeamSelect teamSelectManager;

    [Space]
    public Camera mainCamera;
    public Camera fxCamera;

    [Space]
    public GameObject fadeTransition;
    public GameObject loadingPanel;

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
    private bool zoomSfxDone;
    private bool canSelectNode;

    private Vector2 camDefaultPosition;
    private Vector2 destination;

    private Areas currentSelectMapData;

    private GameObject currentSelectedNode;
    private GameObject currentSelectedBattle;
    
    void Start()
    {
        dataController = GameObject.FindObjectOfType<DataController>();
        if (dataController != null)
        {
            GameObject.FindObjectOfType<AudioManager>().PlaySound("Monsters Underground");

            SetDefaultValues();
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
                StartCoroutine(LoadGameProgress());
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
        zoomSfxDone = false;
    }

    IEnumerator LoadGameProgress()
    {
        if((dataController.currentArea == null) && (dataController.currentNode == null) &&
            (dataController.currentBattle == null))
        {
            AssignNodeData();
            UnlockMaps();
        }
        else
        {
            loadingPanel.SetActive(true);
            if (dataController.currentArea.areaId == (dataController.currentSaveData.currentAreaId - 1))
            {

            }
            else
            {
                if (dataController.currentNode.nodeId == (dataController.currentSaveData.currentNodeId - 1))
                {

                }
                else
                {
                    if (dataController.currentBattle.battleId == (dataController.currentSaveData.currentBattleId - 1))
                    {
                        AssignNodeData();
                        UnlockMaps();

                        dataController.currentArea = dataController.currentNode.area;

                        ExploreMap();
                        EnableNodeColliders(false);
                        RemoveAllPointers();

                        int nodeId = dataController.currentSaveData.currentNodeId;

                        yield return new WaitForSeconds(5f);

                        loadingPanel.SetActive(false);
                        StartCoroutine(nodes[nodeId].GetComponent<NodeManager>().ShowListOfLevels());

                        dataController.currentBattle = null;
                    }
                    else
                    {
                        AssignNodeData();
                        UnlockMaps();

                        loadingPanel.SetActive(false);
                    }
                }
            }
        }
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
            yield return new WaitForSeconds(0.5f);
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

    IEnumerator ShowAreaTooltip(GameObject obj)
    {
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
        if (dataController.currentArea != null)
            StartCoroutine(HideAreaThreatLevel());

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
                
                currentSelectMapData = area;
                dataController.currentArea = area;

                break;
            }
                
        }
    }

    IEnumerator HideAreaThreatLevel()
    {
        foreach (Areas area in areaData)
        {
            if (area.Equals(dataController.currentArea))
            {
                areaHover[area.areaId].GetComponent<Animator>().SetBool("Fade Out", true);
                exploreButton.gameObject.GetComponent<Animator>().SetBool("Exit", true);
                yield return new WaitForSeconds(.2f);
                areaHover[area.areaId].GetComponent<Animator>().SetBool("Fade Out", false);
                exploreButton.gameObject.GetComponent<Animator>().SetBool("Exit", false);
                areaHover[area.areaId].SetActive(false);
                exploreButton.gameObject.SetActive(false);

                break;
            }
        }
    }

    private void ExploreMap()
    {
        GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 3");

        camDefaultPosition = mainCamera.transform.position;
        if (dataController != null)
        {
            destination = dataController.currentArea.coordinates;

            canSelectMap = false;
            focusArea = true;
            move = true;

            infectedAreasSign.GetComponent<Animator>().SetBool("Fade Out", true);
            zwaButton.gameObject.SetActive(false);

            foreach (GameObject areaName in areaNames)
                areaName.SetActive(false);
        }
        else
        {
            destination = currentSelectMapData.coordinates;

            canSelectMap = false;
            focusArea = true;
            move = true;

            infectedAreasSign.GetComponent<Animator>().SetBool("Fade Out", true);
            zwaButton.gameObject.SetActive(false);

            foreach (GameObject areaName in areaNames)
                areaName.SetActive(false);
        }
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

        if (dataController != null)
        {
            areaNames[dataController.currentArea.areaId].SetActive(true);
        }
        else
        {
            areaNames[currentSelectMapData.areaId].SetActive(true);
        }
        
    }

    private void ShowFocusedAreaContent()
    {
        infectedAreasSign.SetActive(false);
        zoomOutButton.gameObject.SetActive(true);

        focusedAreaName.text = dataController.currentArea.areaName;
        focusedAreaName.gameObject.SetActive(true);

        focusedSubname.text = dataController.currentArea.subtitle;
        focusedSubname.gameObject.SetActive(true);

        if(dataController != null) 
        {
            Debug.Log(dataController.currentSaveData.currentNodeId);
            if (dataController.currentSaveData.currentNodeId == -1)
            {
                dataController.currentSaveData.currentNodeId++;
                currentNodeId = dataController.currentSaveData.currentNodeId;

                StartCoroutine(UnlockNodeWithAnimation(dataController.currentArea.nodes[currentNodeId]));
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
            node.GetComponent<NodeManager>().SetPreviousNodeData(dataController.currentArea.
                nodes[currentNodeId], nodes[currentNodeId]);
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

    public void EnableNodeColliders(bool enabled)
    {
        foreach (Node node in dataController.currentArea.nodes)
            nodes[node.nodeId].GetComponent<BoxCollider2D>().enabled = enabled;
    }

    public void HideListOfLevels()
    {
        if (dataController != null)
        {
            GameObject.FindObjectOfType<AudioManager>().PlaySound("Whoosh 06");
            StartCoroutine(dataController.currentNodeObject.
                GetComponent<NodeManager>().HideListOfLevels());
        }
        
    }

    public void RetreatFromNode()
    {
        if (dataController != null)
        {
            GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 3");
            StartCoroutine(dataController.currentBattleObject.
                GetComponent<LevelManager>().HideBattleDetails());
        }
    }

    public void HuntNode()
    {
        GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 3");
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

    public void CancelTeamSelect()
    {
        GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 3");

        teamSelectManager.CancelBattle();
        StartCoroutine(HideTeamSelect());
    }

    IEnumerator HideTeamSelect()
    {
        EnableNodeColliders(true);

        scavengerRoster.GetComponent<Animator>().SetBool("Slide Right", true);
        yield return new WaitForSeconds(.5f);
        scavengerRoster.GetComponent<Animator>().SetBool("Slide Right", false);
        scavengerRoster.SetActive(false);

        teamSelect.GetComponent<Animator>().SetBool("Exit", true);
        yield return new WaitForSeconds(2f);
        teamSelect.GetComponent<Animator>().SetBool("Exit", false);
        teamSelect.SetActive(false);
    }

    public void EnterBattle()
    {
        if (dataController != null)
        {
            bool canEnterBattle = teamSelectManager.EnterBattle();

            if (canEnterBattle)
            {
                StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("Monsters Underground", 2f));

                int nextSceneId = 0;
                if (dataController.currentBattle.cutsceneAtStart || dataController.currentBattle.isMajorCutscene)
                {
                    nextSceneId = dataController.GetNextSceneId(dataController.currentBattle.nextLevel);
                    dataController.currentCutscene = dataController.currentBattle.startCutscene;
                    StartCoroutine(LoadScene(nextSceneId));
                }
                else
                {
                    nextSceneId = dataController.GetNextSceneId(dataController.currentBattle.nextLevel);
                    StartCoroutine(LoadScene(nextSceneId));
                }
            }
        }
    }

    IEnumerator LoadScene(int sceneId)
    {
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", true);
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", false);

        SceneManager.LoadScene(sceneId);
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
                        GameObject obj = hit.transform.gameObject;
                        if (obj.name.Equals("Lock"))
                        {
                            GameObject.FindObjectOfType<AudioManager>().PlaySound("Beep Denied");
                            StartCoroutine(ShowAreaTooltip(obj));
                        }

                        if (obj.name.Equals("Terre") || obj.name.Equals("Mare")
                            || obj.name.Equals("Atmos"))
                        {
                            GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 3");
                            StartCoroutine(ShowAreaThreatLevel(obj));
                        }

                    }
                    else
                    {
                        if (dataController.currentArea != null)
                            StartCoroutine(HideAreaThreatLevel());
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

                        mainCamera.transform.Translate((direction) * dataController.currentArea.moveSpeed * Time.deltaTime);
                        fxCamera.transform.Translate((direction) * dataController.currentArea.moveSpeed * Time.deltaTime);
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

                        mainCamera.transform.Translate((direction) * dataController.currentArea.moveSpeed * Time.deltaTime);
                        fxCamera.transform.Translate((direction) * dataController.currentArea.moveSpeed * Time.deltaTime);
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

                if (!zoomSfxDone)
                {
                    GameObject.FindObjectOfType<AudioManager>().PlaySound("Whoosh 02");
                    zoomSfxDone = true;
                }

                if (!zoomOut)
                {
                    if (mainCamSize >= dataController.currentArea.zoomSize && fxCamSize >= dataController.currentArea.zoomSize)
                    {
                        mainCamera.orthographicSize -= (dataController.currentArea.zoomSpeed * Time.deltaTime);
                        fxCamera.orthographicSize -= (dataController.currentArea.zoomSpeed * Time.deltaTime);
                    }
                    else
                    {
                        focus = false;
                        focusArea = false;
                        zoomSfxDone = false;

                        ShowFocusedAreaContent();
                    }
                }
                else
                {
                    if (mainCamSize <= 200 && fxCamSize <= 200)
                    {
                        mainCamera.orthographicSize += (dataController.currentArea.zoomSpeed * Time.deltaTime);
                        fxCamera.orthographicSize += (dataController.currentArea.zoomSpeed * Time.deltaTime);
                    }
                    else
                    {
                        focus = false;
                        focusArea = false;
                        zoomOut = false;
                        zoomSfxDone = false;

                        HideFocusedAreaContent();
                    }
                }

            }
        }
    }
}
