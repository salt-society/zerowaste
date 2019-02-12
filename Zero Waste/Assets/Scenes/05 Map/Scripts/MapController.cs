using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapController : MonoBehaviour
{
    public DataController dataController;

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
    private int currentBattleId;

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
    
    void Start()
    {
        // dataController = GameObject.FindObjectOfType<DataController>();
        // 0 indicates tutorial/start game
        SetDefaultValues();

        AssignBattleData();
        UnlockMaps();
    }

    void SetDefaultValues()
    {
        canSelectMap = false;
        focusArea = false;
        move = false;
        focus = false;
        zoomOut = false;

        currentAreaId = 0;
        currentBattleId = 4;
    }

    void AssignBattleData()
    {
        int i = 0;
        foreach (Areas area in areaData)
        {
            GameObject parent = nodeParents[area.areaId];
            List<Battle> battles = area.battles;

            if (battles.Count > 0)
            {
                foreach (Battle battle in battles)
                {
                    GameObject node = Instantiate(nodePrefab, parent.transform);
                    node.transform.localPosition = area.positions[i];

                    if (battle.hasPath)
                    {
                        GameObject path = Instantiate(pathPrefab, node.transform);
                        path.transform.localScale = battle.pathScale;
                        path.transform.localPosition = battle.pathPosition;
                        path.transform.Rotate(battle.pathRotation);

                        path.SetActive(false);
                    }
                    
                    node.SetActive(false);
                    nodes.Add(node);

                    nodes[i].GetComponent<NodeManager>().SetBattleData(Instantiate(battle), node);
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
        if (area.areaId <= currentAreaId)
        {
            locks[area.areaId].GetComponent<Animator>().SetBool("Unlock Map", true);
            yield return new WaitForSeconds(1.5f);
            locks[area.areaId].GetComponent<Animator>().SetBool("Unlock Map", false);
            locks[area.areaId].SetActive(false);
        }
    }

    public void UnlockNodes()
    {
        foreach (Areas area in areaData)
        {
            if (area.areaId <= currentAreaId)
            {
                foreach (Battle battle in area.battles)
                {
                    if (battle.battleId <= currentBattleId)
                    {
                        nodes[battle.battleId].SetActive(true);

                        if (battle.hasPath && (battle.battleId < currentBattleId))
                        {
                            Transform pathTransform = nodes[battle.battleId].transform.GetChild(1);
                            GameObject path = pathTransform.gameObject;
                            path.SetActive(true);
                        }

                        if (battle.battleId == currentBattleId)
                            PointCurrentNode(battle.battleId);
                    }
                }

                nodeParents[area.areaId].SetActive(true);
            }
        }
    }

    public void LockNodes()
    {
        foreach (Areas area in areaData)
        {
            if (area.areaId <= currentAreaId)
            {
                foreach (Battle battle in area.battles)
                {
                    if (battle.battleId <= currentBattleId)
                    {
                        nodes[battle.battleId].SetActive(false);

                        if (battle.hasPath && (battle.battleId < currentBattleId))
                        {
                            Transform pathTransform = nodes[battle.battleId].transform.GetChild(1);
                            GameObject path = pathTransform.gameObject;
                            path.SetActive(false);
                        }

                        if (battle.battleId == currentBattleId)
                            PointCurrentNode(battle.battleId);
                    }
                }

                nodeParents[area.areaId].SetActive(false);
            }
        }
    }

    public void PointCurrentNode(int battleId)
    {
        Transform pointerTransform = nodes[battleId].transform.GetChild(0);
        pointerTransform.gameObject.SetActive(true);
    }

    public void RemovePointer(int battleId)
    {
        Transform pointerTransform = nodes[battleId].transform.GetChild(0);
        pointerTransform.gameObject.SetActive(false);
    }

    IEnumerator UnlockNodeWithAnimation(Battle battle)
    {
        if (battle.battleId <= currentBattleId)
        {
            if(battle.battleId != 0) 
            {
                Battle previousBattle = areaData[currentAreaId].battles[battle.battleId - 1];
                if (previousBattle.hasPath)
                {
                    Transform pathTransform = nodes[previousBattle.battleId].transform.GetChild(1);
                    GameObject path = pathTransform.gameObject;
                    path.SetActive(true);

                    RemovePointer(previousBattle.battleId);
                }

                yield return new WaitForSeconds(.5f);
            }
            
            nodes[battle.battleId].SetActive(true);

            if (battle.battleId == currentBattleId)
                PointCurrentNode(battle.battleId);
                
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

                        Debug.Log("X greater than 0");
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

                        Debug.Log("X lesser than 0");
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
        foreach (GameObject node in nodes)
            StartCoroutine(node.GetComponent<NodeManager>().HideNode());

        yield return new WaitForSeconds(1f);

        camDefaultPosition = mainCamera.transform.position;
        destination = new Vector3(0, 0, -10);

        canSelectMap = true;
        focusArea = true;
        move = true;
        zoomOut = true;
    }

    private void ShowFocusedAreaContent()
    {
        infectedAreasSign.SetActive(false);
        zoomOutButton.gameObject.SetActive(true);

        focusedAreaName.text = currentSelectMapData.areaName;
        focusedAreaName.gameObject.SetActive(true);

        focusedSubname.text = currentSelectMapData.subtitle;
        focusedSubname.gameObject.SetActive(true);

        UnlockNodes();

        foreach (GameObject node in nodes)
        {
            node.GetComponent<NodeManager>().SetMapController(this);
            node.GetComponent<NodeManager>().AllowNodeSelection(1);
            node.GetComponent<NodeManager>().SetPreviousBattleData(currentSelectMapData.battles[currentBattleId], nodes[currentBattleId]);
            node.GetComponent<NodeManager>().SendAreaNameComponents(focusedAreaName, focusedSubname);
            node.GetComponent<NodeManager>().SendNodeDetailComponents(nodeDetails);
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

    public void RetreatFromNode()
    {
        nodeDetails.GetComponent<Animator>().SetBool("Exit", true);
        StartCoroutine(currentSelectedNode.GetComponent<NodeManager>().HideNodeDetails());
    }

    public void HuntNode()
    {
        StartCoroutine(ShowTeamSelect());
    }

    IEnumerator ShowTeamSelect()
    {
        RetreatFromNode();
        yield return new WaitForSeconds(2f);
        teamSelect.SetActive(true);
    }

    public void CancelTeamSelection()
    {
        StartCoroutine(HideTeamSelect());
    }

    IEnumerator HideTeamSelect()
    {
        teamSelect.GetComponent<Animator>().SetBool("Exit", true);
        yield return new WaitForSeconds(2f);
        teamSelect.GetComponent<Animator>().SetBool("Exit", false);
        teamSelect.SetActive(false);
    }
}
