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
    public LevelList levelListManager;

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
    public List<GameObject> nodeContainers;

    private bool mapSelectionState;
    private bool focusOnSpecificArea;
    private bool cameraMoveState;
    private bool cameraFocusState;
    private bool zoomOutState;
    private bool zoomSfxState;
    private bool canSelectNode;

    private Vector2 camDefaultPosition;
    private Vector2 destination;

    private GameObject currentSelectedNode;
    private GameObject currentSelectedBattle;
    
    void Start()
    {
        // Find data controller
        dataController = GameObject.FindObjectOfType<DataController>();

        // Just to be safe, always check if there's a data controller before execution
        if (dataController != null)
        {
            // Play BGM
            GameObject.FindObjectOfType<AudioManager>().PlaySound("Monsters Underground");

            // Set default values of bools/flags and start loading game progess
            SetFlags();
            StartCoroutine(LoadGameProgress());
        }
    }

    void SetFlags()
    {
        // States if map can be clicked
        mapSelectionState = false;

        // States if an area should be focused
        // Will be triggered when ExploreButton() is called
        focusOnSpecificArea = false;
        cameraMoveState = false;
        cameraFocusState = false;

        // This one's for reversing emphasis on area
        zoomOutState = false;

        // Able to play sfx every zoom in and out
        zoomSfxState = false;
    }

    IEnumerator LoadGameProgress()
    {
        Debug.Log("@LoadGameProgress()");

        // If currentArea, currentNode, and currentBattle are null, it means player
        // just got to Map Screen and haven't played any battles
        if((dataController.currentArea == null) && (dataController.currentNode == null) &&
            (dataController.currentBattle == null))
        {
            // To prevent unlock animation from being repeated every time game is played,
            // I didn't based the unlock animation on area count

            // At start, area unlock will always be 1 thus if the condition is base on it
            // unlock animation will always play. By Default, whenever there's a new game
            // ids, like currentAreaId, is equal to -1 which acts as a condition if unlock 
            // animation should be play upon entering Map Screen
            if (dataController.currentSaveData.currentAreaId == -1)
            {
                // Increment so unlock animation of first area won't happen again
                dataController.currentSaveData.currentAreaId++;

                // Save progress
                dataController.SaveSaveData();
                dataController.SaveGameData();

                // Create all nodes, assign each with data then unlock first area
                CreateNodes();
                StartCoroutine(UnlockAreaWithAnimation(areaData[0]));
            }
            // Doesn't play map unlock animation again cause been there, done that
            else
            {
                // Create all nodes, assign each with and unlock maps
                CreateNodes();
                UnlockAreaMaps(false);
            }
        }
        else
        {
            // Hide map screen by showing loading panel
            loadingPanel.SetActive(true);

            // Check if there is a level need to unlock
            // Checking should be in this order -> Area (Map), Node, Battle
            // If there is a change in Area, unlock it with animation
            // NOTE: Area can only be unlocked one at a time
            if (dataController.currentArea.areaId < dataController.currentSaveData.areas.Count - 1)
            {
                // Save progress
                dataController.SaveSaveData();
                dataController.SaveGameData();

                // Create all nodes, assign each with data the unlock first area
                CreateNodes();
                UnlockAreaMaps(false);
                StartCoroutine(UnlockAreaWithAnimation(areaData[dataController.currentSaveData.areas.Count - 1]));
            }
            // If no changes, check nodes in current area
            else
            {
                // If there is a change in Node inside current area, unlock it with animation
                // NOTE: Node can only be unlocked one at a time
                if (dataController.currentNode.nodeId < dataController.currentSaveData.nodes.Count - 1)
                {
                    // Create all nodes and unlock maps that can be unlocked
                    CreateNodes();
                    UnlockAreaMaps(false);

                    // Make sure that map and node selection isn't enabled
                    mapSelectionState = false;
                    // nodes[dataController.currentSaveData.nodes.Count - 1].GetComponent<NodeManager>().AllowNodeSelection(0);

                    // Set which area to focus in, the area of current node
                    // Then start focusing
                    dataController.currentArea = dataController.currentNode.area;
                    dataController.currentSaveData.currentAreaId = dataController.currentArea.areaId;

                    ExploreMap();
                    EnableNodeColliders(false);
                    RemoveAllPointers();

                    // Show unlock animation/sign
                    yield return null;

                    // If unlocking is finished, make map interactable again
                    // nodes[dataController.currentSaveData.nodes.Count - 1].GetComponent<NodeManager>().AllowNodeSelection(1);
                }
                // If no changes, check battles in current node
                else
                {
                    // Since ZWA is not yet finished, create a temporary bool
                    // that states that there isn't a ZWA Cutscene unlocked
                    bool toZWA = false;

                    // Check if a ZWA Cutscene is unlocked
                    if (toZWA)
                    {

                    }
                    else
                    {
                        // Always zoom to current node even if there's no new battle
                        CreateNodes();
                        UnlockAreaMaps(false);

                        // Make sure that map and node selection isn't enabled
                        mapSelectionState = false;

                        // Set which area to focus in, the area of current node
                        // Then start focusing
                        dataController.currentArea = dataController.currentNode.area;
                        dataController.currentSaveData.currentAreaId = dataController.currentArea.areaId;

                        ExploreMap();
                        EnableNodeColliders(false);
                        RemoveAllPointers();

                        // Delay
                        yield return new WaitForSeconds(3f);

                        // Get nodeId of current node and show list of battles
                        int nodeId = dataController.currentNode.nodeId;
                        StartCoroutine(nodes[nodeId].GetComponent<NodeManager>().ShowListOfBattles());
                        
                    }
                }
            }
        }
    }

    void CreateNodes()
    {
        Debug.Log("@CreateNodes()");

        // Just to be safe, always check if there's a data controller before execution
        if (dataController == null)
            return;

        // Creates all nodes upon entering Map though not visible
        int i = 0;
        foreach (Areas area in areaData)
        {
            // Get container of nodes in and node itself in each area
            GameObject nodeContainer = nodeContainers[area.areaId];
            List<Node> nodes = area.nodes;

            // Check if there's a node in current area
            if (nodes.Count > 0)
            {
                // Loop through node data
                foreach (Node node in nodes)
                {
                    // Create a node
                    GameObject nodeObj = Instantiate(nodePrefab, nodeContainer.transform);
                    nodeObj.transform.localPosition = area.nodePositions[i];

                    // Check if node has a path
                    // End nodes, very last node in area, doesn't have path attached to it
                    if (node.hasPath)
                    {
                        // Create path
                        GameObject path = Instantiate(pathPrefab, nodeObj.transform);
                        path.transform.localScale = node.pathScale;
                        path.transform.localPosition = node.pathPosition;
                        path.transform.Rotate(node.pathRotation);

                        path.SetActive(false);
                    }
                    nodeObj.SetActive(false);
                    nodeObj.GetComponent<NodeManager>().SetNodeData(Instantiate(node), nodeObj);

                    // Add to node list so that node can be accessed later
                    this.nodes.Add(nodeObj);
                    i++;
                }

                // Reset counter
                i = 0;
            }
        }
    }

    public void UnlockAreaMaps(bool unlockAreaWithAnimation)
    {
        Debug.Log("@UnlockAreaMaps()");

        // Just to be safe, always check if there's a data controller before execution
        if (dataController != null)
        {
            // Loop through each area
            foreach (Areas area in areaData)
            {
                // Unlock area only if its less than or equal current areas unlocked
                if (dataController.currentSaveData.areas.ContainsKey(area.areaId))
                {
                    // Check if area's going to be unlocked with animation
                    if (area.areaId == (dataController.currentSaveData.areas.Count - 1))
                    {
                        if (!unlockAreaWithAnimation)
                        {
                            // Enable collider so in can be clicked, remove lock, and show area name
                            areaMap[area.areaId].GetComponent<BoxCollider2D>().enabled = true;
                            locks[area.areaId].SetActive(false);
                            areaNames[area.areaId].SetActive(true);

                            break;
                        }
                    }
                    else
                    {
                        // Enable collider so in can be clicked, remove lock, and show area name
                        areaMap[area.areaId].GetComponent<BoxCollider2D>().enabled = true;
                        locks[area.areaId].SetActive(false);
                        areaNames[area.areaId].SetActive(true);

                        Debug.Log("Unlocked" + area.areaName + ".");
                    }
                }
                else
                {
                    Debug.Log("Area " + area.areaName + " not in list of unlocked areas");
                }
            }

            // Change flag to enable map selection (raycast, on Update method)
            mapSelectionState = true;
        }
    }

    IEnumerator UnlockAreaWithAnimation(Areas area)
    {
        Debug.Log("@UnlockedAreaWithAnimation()");

        // Just to be safe, always check if there's a data controller before execution
        if (dataController != null)
        {
            // Delay to give way for transition to finish before unlocking Map
            if (dataController.currentSaveData.currentAreaId == 0)
                yield return new WaitForSeconds(0.5f);

            // Unlock area only if its less than or equal current areas unlocked
            if (area.areaId <= (dataController.currentSaveData.areas.Count -1))
            {
                locks[area.areaId].GetComponent<Animator>().SetBool("Unlock Map", true);
                yield return new WaitForSeconds(1.5f);
                locks[area.areaId].GetComponent<Animator>().SetBool("Unlock Map", false);
                locks[area.areaId].SetActive(false);

                yield return new WaitForSeconds(0.2f);

                areaNames[area.areaId].SetActive(true);
                areaMap[area.areaId].GetComponent<BoxCollider2D>().enabled = true;
                mapSelectionState = true;
            }
        }
    }

    public void ShowUnlockedNodes(bool unlockLastNodeWithAnimation)
    {
        Debug.Log("@ShowNodesUnlocked()");

        // Just to be safe, always check if there's a data controller before execution
        if (dataController == null)
            return;

        // If an area is selected, unlock its nodes
        // Just to be sure check if there's a current area in data controller
        if (dataController.currentArea != null)
        {
            // Loop through each node data
            foreach (Node node in dataController.currentArea.nodes)
            {
                // If node id is in node dictionary, its unlocked
                if (dataController.currentSaveData.nodes.ContainsKey(node.nodeId))
                {
                    // Point to latest node unlocked
                    if (node.nodeId == (dataController.currentSaveData.nodes.Count - 1))
                    {
                        // Point pointer to last node unlocked
                        PointCurrentNode(node.nodeId);

                        // Plays unlock animation if true
                        if (unlockLastNodeWithAnimation)
                            StartCoroutine(UnlockNodeWithAnimation(node));
                    }
                        
                    // Show node
                    // nodes is a list of node gameObjects, not the data
                    nodes[node.nodeId].SetActive(true);

                    // Check if node has path
                    // Only check if node preceeds/second to the latest unlocked node
                    if (node.hasPath && (node.nodeId == (dataController.currentSaveData.nodes.Count - 2)))
                    {
                        // Get path gameObject and set it to true
                        nodes[node.nodeId].transform.GetChild(1).gameObject.SetActive(true);
                    }
                }
            }

            // Activate node container
            nodeContainers[dataController.currentArea.areaId].SetActive(true);
        }
    }

    public void HideNodes()
    {
        Debug.Log("@HideNodes()");
        // Just to be safe, always check if there's a data controller before execution
        if (dataController == null)
            return;

        // If an area is selected, hide its nodes
        // Just to be sure check if there's a current area in data controller
        if (dataController.currentArea != null)
        {
            // Loop through each node data
            foreach (Node node in dataController.currentArea.nodes)
            {
                // If node id is in node dictionary, its unlocked
                if (dataController.currentSaveData.nodes.ContainsKey(node.nodeId))
                {
                    // Remove all pointes
                    RemoveAllPointers();

                    // Hide node
                    // nodes is a list of node gameObjects, not the data
                    nodes[node.nodeId].SetActive(false);

                    // Check if node has path
                    // Only check if node preceeds/second to the latest unlocked node
                    if (node.hasPath)
                    {
                        // Get path gameObject and set it to true
                        nodes[node.nodeId].transform.GetChild(1).gameObject.SetActive(false);
                    }
                }
            }

            // Activate node container
            nodeContainers[dataController.currentArea.areaId].SetActive(false);
        }
    }

    public void PointCurrentNode(int nodeId)
    {
        Debug.Log("@PointCurrentNode()");
        // Gets pointer of node and activates it
        Transform pointerTransform = nodes[nodeId].transform.GetChild(0);
        pointerTransform.gameObject.SetActive(true);
    }

    public void RemovePointer(int nodeId)
    {
        Debug.Log("@RemovePointer()");
        // Gets pointer of node and hides it
        Transform pointerTransform = nodes[nodeId].transform.GetChild(0);
        pointerTransform.gameObject.SetActive(false);
    }

    public void RemoveAllPointers()
    {
        Debug.Log("@RemoveAllPointers()");
        // Get all pointers in all nodes and hide
        foreach (GameObject node in nodes)
            node.transform.GetChild(0).gameObject.SetActive(false);
    }

    IEnumerator UnlockNodeWithAnimation(Node node)
    {
        Debug.Log("@UnlockNodeWithAnimation()");
        // Just to be safe, always check if there's a data controller before execution
        if (dataController != null)
        {
            // Delay
            yield return new WaitForSeconds(0.5f);

            // Only unlock node if its id is in dictionary
            if (dataController.currentSaveData.nodes.ContainsKey(node.nodeId))
            {
                // Point to latest node unlocked
                if (node.nodeId == (dataController.currentSaveData.nodes.Count - 1))
                    PointCurrentNode(node.nodeId);

                // Check if node is not the very first node in area
                // to activate previous node's path
                if (node.nodeId != node.area.startNodeIndex)
                {
                    // Get previous node
                    Node previousNode = areaData[dataController.currentArea.areaId].nodes[node.nodeId - 1];

                    // If previous node has path, activate it
                    if (previousNode.hasPath)
                    {
                        nodes[previousNode.nodeId].transform.GetChild(1).gameObject.SetActive(true);
                        RemovePointer(previousNode.nodeId);
                    }

                    // Delay
                    yield return new WaitForSeconds(0.5f);
                }

                // Show latest node by activating node and its container
                nodes[node.nodeId].SetActive(true);
                nodeContainers[dataController.currentSaveData.areas.Count - 1].SetActive(true);
            }
        }
    }

    IEnumerator ShowAreaTooltip(GameObject hitObj)
    {
        Debug.Log("@ShowAreaTooltip(GameObject hitObj)");

        // Just to be safe, always check if there's a data controller before execution
        if (dataController != null)
        {
            foreach (Areas area in areaData)
            {
                if (area.areaName.Equals(hitObj.transform.parent.gameObject.name))
                {
                    areaTooltips[area.areaId].SetActive(true);
                    yield return new WaitForSeconds(2f);
                    areaTooltips[area.areaId].SetActive(false);
                    break;
                }
            }
        }
    }

    IEnumerator ShowAreaTooltip()
    {
        Debug.Log("@ShowAreaTooltip()");

        // Just to be safe, always check if there's a data controller before execution
        if (dataController != null)
        {
            areaTooltips[dataController.currentArea.areaId].SetActive(true);
            yield return new WaitForSeconds(2f);
            areaTooltips[dataController.currentArea.areaId].SetActive(false);
        }
    }

    IEnumerator ShowAreaThreatLevel(GameObject hitObj)
    {
        Debug.Log("@ShowAreaThreatLevel(GameObject hitObj)");

        // Just to be safe, always check if there's a data controller before execution
        if (dataController != null)
        {
            // If an area is already selected, remove threat level and tooltip
            if (dataController.currentArea != null)
            {
                StartCoroutine(HideAreaThreatLevel());
                yield return new WaitForSeconds(.5f);
            }

            // Loop through each area in areaData list
            foreach (Areas area in areaData)
            {
                // Check which area should threat level be shown by
                // comparing if name of object and area is the same
                if (area.areaName.Equals(hitObj.transform.gameObject.name))
                {
                    // Show threat level and explore button
                    exploreButton.gameObject.SetActive(true);
                    areaHover[area.areaId].SetActive(true);

                    // Set currently selected area/map
                    dataController.currentArea = area;
                    dataController.currentSaveData.currentAreaId = area.areaId;
                    
                    // Temporary set text of tooltip for this area
                    TextMeshProUGUI tooltipText = areaTooltips[area.areaId].transform.GetChild(0)
                        .gameObject.GetComponent<TextMeshProUGUI>();
                    tooltipText.text = "Doctor Cooper allowed you to explore this area.";
                    StartCoroutine(ShowAreaTooltip());

                    break;
                }

            }
        }
    }

    IEnumerator HideAreaThreatLevel()
    {
        Debug.Log("@HideAreaThreatLevel()");

        // Just to be safe, always check if there's a data controller before execution
        if (dataController != null)
        {
            // If unselected, hide area threat level and select button
            // This will happen when player clicks anywhere on screen or clicks another area
            areaHover[dataController.currentArea.areaId].GetComponent<Animator>().SetBool("Fade Out", true);
            exploreButton.gameObject.GetComponent<Animator>().SetBool("Exit", true);
            yield return new WaitForSeconds(.2f);
            areaHover[dataController.currentArea.areaId].GetComponent<Animator>().SetBool("Fade Out", false);
            exploreButton.gameObject.GetComponent<Animator>().SetBool("Exit", false);
            areaHover[dataController.currentArea.areaId].SetActive(false);
            exploreButton.gameObject.SetActive(false);
        }
    }

    private void ExploreMap()
    {
        Debug.Log("@ExploreMap()");

        // Just to be safe, always check if there's a data controller before execution
        if (dataController == null)
            return;

        // Play SFX
        GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 3");

        // Prepeare for Zoom In
        // Get camera's default position and destination coordinates
        camDefaultPosition = mainCamera.transform.position;
        destination = dataController.currentArea.coordinates;

        // Trigger Zoom In function (which is in Update method) by
        // changing the flags focusArea and move to true
        focusOnSpecificArea = true;
        cameraMoveState = true;

        // While zooming in, make sure to disable click function (raycast)
        mapSelectionState = false;

        // Hide signs and ZWA Button
        infectedAreasSign.GetComponent<Animator>().SetBool("Fade Out", true);
        zwaButton.gameObject.SetActive(false);

        // Hide all area names
        foreach (GameObject areaName in areaNames)
            areaName.SetActive(false);
    }

    private void ZoomOutOfMap()
    {
        Debug.Log("@ZommOutOfMap");

        // Placed coroutine in another function so it can to called
        // from button click/added as listener
        StartCoroutine(ZoomOutAnimation());
    }

    IEnumerator ZoomOutAnimation()
    {
        Debug.Log("@ZoomOutAnimation()");
        // Just to be safe, always check if there's a data controller before execution
        if (dataController != null)
        {
            // Hide all nodes in current area
            for (int i = 0; i < dataController.currentArea.maxNodes; i++)
                StartCoroutine(nodes[i].GetComponent<NodeManager>().HideNode());

            // Delay
            yield return new WaitForSeconds(1f);

            // Prepare for Zoom Out
            // Get camera's current position and destination coordinates
            camDefaultPosition = mainCamera.transform.position;
            destination = new Vector3(0, 0, -10);

            // Trigger Zoom Out function (which is in Update method) 
            // by changing flags to false
            focusOnSpecificArea = true;
            cameraMoveState = true;
            zoomOutState = true;

            // While zooming out, make sure to disable click function (raycast)
            mapSelectionState = false;

            // Delay
            yield return new WaitForSeconds(1f);

            // Display names of unlocked areas
            foreach (Areas area in areaData)
            {
                if (dataController.currentSaveData.areas.ContainsKey(area.areaId))
                {
                    areaNames[area.areaId].SetActive(true);
                }
            }

            // Map is interactable once zooming out is done
            mapSelectionState = true;
        }
    }

    private void ShowAreaContent()
    {
        Debug.Log("@ShowAreaContent()");

        // Just to be safe, always check if there's a data controller before execution
        if (dataController == null)
            return;

        // If loading panel is open, hide it
        if (loadingPanel.activeInHierarchy)
            loadingPanel.SetActive(!loadingPanel.activeInHierarchy);

        // Unlock Nodes
        // Check currentNodeId to determine if its a new game
        if (dataController.currentSaveData.currentNodeId == -1)
        {
            // Increment so unlock animation wont happend again for the first node
            dataController.currentSaveData.currentNodeId++;
            StartCoroutine(UnlockNodeWithAnimation(dataController.currentArea.nodes[0]));

            // Save progress
            dataController.SaveSaveData();
            dataController.SaveGameData();
        }
        // If there's a need to unlock a node
        else if (dataController.currentNode != null)
        {
            if (dataController.currentNode.nodeId < (dataController.currentSaveData.nodes.Count - 1))
            {
                // Remove current selected node because players are allowed
                // to choose another node other than previous nodes
                dataController.currentNode = null;

                // By passing true, unlock animation for latest node will play
                ShowUnlockedNodes(true);

                // Allow node selection
                nodes[dataController.currentSaveData.nodes.Count
                    - 1].GetComponent<NodeManager>().AllowNodeSelection(1);

                // Save progress
                dataController.SaveSaveData();
                dataController.SaveGameData();
            }
            else
            {
                ShowUnlockedNodes(false);
            }
            
        }
        // If there's no change in nodes or game just started,
        // continued where player left off
        else
        {
            ShowUnlockedNodes(false);
        }

        // Set up all node managers, since all nodes are created, just hidden
        foreach (GameObject node in nodes)
        {
            node.GetComponent<NodeManager>().SetMapController(this);
            node.GetComponent<NodeManager>().AllowNodeSelection(1);
            node.GetComponent<NodeManager>().SetPreviousNodeData(dataController.currentArea.
                nodes[dataController.currentSaveData.currentNodeId], nodes[dataController.currentSaveData.currentNodeId]);
            node.GetComponent<NodeManager>().SendAreaNameComponents(focusedAreaName, focusedSubname);
            node.GetComponent<NodeManager>().SendNodeDetailComponents(nodeDetails);
            node.GetComponent<NodeManager>().SendLevelListComponents(levelList);
        }

        // Hide signs and show zoom out button
        infectedAreasSign.SetActive(false);
        zoomOutButton.gameObject.SetActive(true);

        // Show name of focused area as well as its subname
        focusedAreaName.text = dataController.currentArea.areaName;
        focusedAreaName.gameObject.SetActive(true);

        focusedSubname.text = dataController.currentArea.subtitle;
        focusedSubname.gameObject.SetActive(true);
    }

    private void HideAreaContent()
    {
        HideNodes();

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
        Debug.Log("@HuntListOfLevels()");

        // Just to be safe, always check if there's a data controller before execution
        if (dataController != null)
        {
            // Play SFX
            GameObject.FindObjectOfType<AudioManager>().PlaySound("Whoosh 06");

            StartCoroutine(nodes[dataController.currentNode.nodeId].GetComponent<NodeManager>().HideListOfBattles());
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
        Debug.Log("@HuntNode()");

        // Just to be safe, always check if there's a data controller before execution
        if (dataController == null)
            return;

        // Check if battle is not null, just to avoid errors if its null
        if (dataController.currentBattle != null)
        {
            // If major cutscene, no need to select team. Go straight to cutscene
            if (dataController.currentBattle.isMajorCutscene)
            {
                // Play SFX and stop BGM
                StartCoroutine(GameObject.FindObjectOfType<AudioManager>().StopSound("Monsters Underground", 2f));
                GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 3");
                GameObject.FindObjectOfType<AudioManager>().PlaySound("Whoosh 06");

                // Disable button for Hunt Node
                nodeDetails.transform.GetChild(4).GetComponent<Button>().interactable = false;

                // Load next scene
                int nextSceneId = dataController.GetNextSceneId(dataController.currentBattle.nextScene);
                dataController.currentCutscene = dataController.currentBattle.startCutscene;
                StartCoroutine(LoadScene(nextSceneId));
            }
            // If not and has a battle, show team selection panel
            else
            {
                // Play SFX
                GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 3");
                StartCoroutine(ShowTeamSelect());
            }
        }
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
                    nextSceneId = dataController.GetNextSceneId(dataController.currentBattle.nextScene);
                    dataController.currentCutscene = dataController.currentBattle.startCutscene;
                    StartCoroutine(LoadScene(nextSceneId));
                }
                else
                {
                    nextSceneId = dataController.GetNextSceneId(dataController.currentBattle.nextScene);
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
        // Just to be safe, always check if there's a data controller before execution
        if (dataController == null)
            return;

        // Prevents unecessary click that may cause errors
        // Only executes when all components of map are set up
        if (mapSelectionState)
        {
            // Only register click/touch when its only one
            // and at beggining of the touch phase
            if (Input.touchCount == 1 && (TouchPhase.Began == Input.GetTouch(0).phase))
            {
                // Convert touch position to world point
                Vector2 origin = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                // Get whatever is hit/collided by touch
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero);

                // If hit collided with something and is not empty, check what it is
                if (hit.collider != null)
                {
                    // Get gameObject
                    GameObject obj = hit.transform.gameObject;

                    // Check if its a lock and show tooltip
                    if (obj.name.Equals("Lock"))
                    {
                        GameObject.FindObjectOfType<AudioManager>().PlaySound("Beep Denied");
                        StartCoroutine(ShowAreaTooltip(obj));
                    }

                    // Check if its any of the three areas, show tooltip and threat level
                    if (obj.name.Equals("Terre") || obj.name.Equals("Mare")
                        || obj.name.Equals("Atmos"))
                    {
                        GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 3");
                        StartCoroutine(ShowAreaThreatLevel(obj));
                    }

                }
                else
                {
                    // If nothing is hit and there is a current seletced area, unselect it
                    if (dataController.currentArea != null)
                        StartCoroutine(HideAreaThreatLevel());
                }
            }
        }

        // If set to true, focuses on selected area
        // 2 parts: (1) moves to current coordinates of map, (2) zoom in
        if (focusOnSpecificArea)
        {
            // First, move cam to current area's coordinates
            if (cameraMoveState)
            {
                // Always get position of camera to determine if camera
                // has reached its destination
                Vector2 camPosition = mainCamera.transform.position;

                if (destination.x > 0)
                {
                    if (camPosition.x <= destination.x)
                    {
                        Vector2 offset, direction;

                        // If zoom in, move camera to destination coordinates
                        if (!zoomOutState)
                        {
                            offset = destination - camDefaultPosition;
                            direction = Vector2.ClampMagnitude(offset, 1.0f);
                        }
                        // If zooming out, go back to default position of cam
                        else
                        {
                            offset = destination + camDefaultPosition;
                            direction = Vector2.ClampMagnitude(offset, 1.0f);
                        }

                        // Move all cameras, not just Main Camera
                        mainCamera.transform.Translate((direction) * dataController.currentArea.moveSpeed * Time.deltaTime);
                        fxCamera.transform.Translate((direction) * dataController.currentArea.moveSpeed * Time.deltaTime);
                    }
                    else
                    {
                        // Set flags to trigger focus animation
                        cameraFocusState = true;
                        cameraMoveState = false;
                    }
                }
                else
                {
                    if (camPosition.x >= destination.x)
                    {
                        Vector2 offset = destination - camDefaultPosition;
                        Vector2 direction = Vector2.ClampMagnitude(offset, 1.0f);

                        mainCamera.transform.Translate((direction) * dataController.currentArea.moveSpeed * Time.deltaTime);
                        fxCamera.transform.Translate((direction) * dataController.currentArea.moveSpeed * Time.deltaTime);
                    }
                    else
                    {
                        cameraFocusState = true;
                        cameraMoveState = false;
                    }
                }
            }

            // Now, decrement camera size to create an illusion of zooming in
            if (cameraFocusState)
            {
                // Get orthographics sizes of all cameras
                float mainCamSize = mainCamera.orthographicSize;
                float fxCamSize = fxCamera.orthographicSize;

                // Zoom SFX
                if (!zoomSfxState)
                {
                    GameObject.FindObjectOfType<AudioManager>().PlaySound("Whoosh 02");
                    zoomSfxState = true;
                }

                // Check if animation is Zoom In/Out
                // If zoom in, decrease size of cameras according to area's zoom size
                if (!zoomOutState)
                {
                    if (mainCamSize >= dataController.currentArea.zoomSize && fxCamSize >= dataController.currentArea.zoomSize)
                    {
                        mainCamera.orthographicSize -= (dataController.currentArea.zoomSpeed * Time.deltaTime);
                        fxCamera.orthographicSize -= (dataController.currentArea.zoomSpeed * Time.deltaTime);
                    }
                    else
                    {
                        // When animation is done, set flgas to false
                        cameraFocusState = false;
                        focusOnSpecificArea = false;
                        zoomSfxState = false;

                        // Show Nodes
                        ShowAreaContent();
                    }
                }
                // If zoom out, increase camera size to default size
                else
                {
                    if (mainCamSize <= 200 && fxCamSize <= 200)
                    {
                        mainCamera.orthographicSize += (dataController.currentArea.zoomSpeed * Time.deltaTime);
                        fxCamera.orthographicSize += (dataController.currentArea.zoomSpeed * Time.deltaTime);
                    }
                    else
                    {
                        // When animation is done, set flgas to false
                        cameraFocusState = false;
                        focusOnSpecificArea = false;
                        zoomOutState = false;
                        zoomSfxState = false;

                        // Hide Nodes
                        HideAreaContent();
                    }
                }

            }
        }
    }
}
