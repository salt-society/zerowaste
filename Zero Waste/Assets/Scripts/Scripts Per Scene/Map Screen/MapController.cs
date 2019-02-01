using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

    [Header("Managers")]
    public NodeSelectManager nodeSelectManager;
    public NotificationManager notifManager;
    public CompassJoystick compassJoystick;

    [Header("Area Details")]
    public Areas[] detailsOfAreas;

    [Header("Nodes for each Area")]
    public GameObject[] mareNodes;
    public GameObject[] terreNodes;
    public GameObject[] atmosNodes;
    [Space]
    public float nodePopSpeed = 0.5f;

    [Header("Effects")]
    public ParticleSystem[] mareFX;
    public ParticleSystem[] terreFX;
    public ParticleSystem[] atmosFX;

    [Header("Cameras")]
    public Camera mainCamera;
    public Camera fxCamera;
    public float zoomSpeed;
    public float moveSpeed;

    private int currentArea;
    // private int currentNode;

    private Vector2 pointA, pointB;

    private bool focusAndMoveCam = false;
    private bool focusCam = false;
    private bool movedCam = false;

    private List<GameObject[]> allNodes;
	
	void Start () {
		// Get data communication between scenes
        // Temporary
        currentArea = 0;
        // currentNode = 1;

        FocusOnCurrentArea();
	}

    void Update()
    {
        if (focusAndMoveCam)
        {
            if (movedCam)
            {
                Vector2 camCurrentPos = mainCamera.transform.position;
                if (pointB.x > 0)
                {
                    if (camCurrentPos.x <= pointB.x)
                    {
                        Vector2 offset = pointB - pointA;
                        Vector2 direction = Vector2.ClampMagnitude(offset, 1.0f);

                        mainCamera.transform.Translate((direction) * moveSpeed * Time.deltaTime);
                        fxCamera.transform.Translate((direction) * moveSpeed * Time.deltaTime);

                        Debug.Log("Moving... " + camCurrentPos + " to " + pointB);
                    }
                    else
                    {
                        focusCam = true;
                        movedCam = false;

                        Debug.Log("Done moving...");
                    }
                }
                else
                {
                    if (camCurrentPos.x >= pointB.x)
                    {
                        Vector2 offset = pointB - pointA;
                        Vector2 direction = Vector2.ClampMagnitude(offset, 1.0f);

                        mainCamera.transform.Translate((direction) * moveSpeed * Time.deltaTime);
                        fxCamera.transform.Translate((direction) * moveSpeed * Time.deltaTime);

                        Debug.Log("Moving... " + camCurrentPos + " to " + pointB);
                    }
                    else
                    {
                        focusCam = true;
                        movedCam = false;

                        Debug.Log("Done moving...");
                    }
                }
                
                
            }

            if (focusCam)
            {
                float mainCamSize = mainCamera.orthographicSize;
                float fxCamSize = fxCamera.orthographicSize;

                if (mainCamSize >= 40 && fxCamSize >= 40)
                {
                    mainCamera.orthographicSize -= (zoomSpeed * Time.deltaTime);
                    fxCamera.orthographicSize -= ( zoomSpeed * Time.deltaTime);

                    Debug.Log("Focusing...");
                }
                else
                {
                    focusCam = false;
                    focusAndMoveCam = false;

                    // check current area the change terreFX to current
                    foreach (ParticleSystem fx in terreFX)
                    {
                        fx.Clear();
                        fx.Stop();
                    }

                    // PrepareNodeDetails();
                    // ShowAllNodes();

                    Debug.Log("Done focusing...");
                }
            }
        }
    }

    void FocusOnCurrentArea()
    {
        foreach (Areas area in detailsOfAreas)
        {
            if (area.areaNumber == currentArea)
            {
                pointA = mainCamera.transform.position;
                pointB = area.coordinates;

                Debug.Log(pointB);

                focusAndMoveCam = true;
                movedCam = true;
                return;
            }
        }
    }

    void PrepareNodeDetails()
    {
        allNodes = new List<GameObject[]>();
        allNodes.Add(terreNodes);
        // allNodes.Add(mareNodes);
        // allNodes.Add(atmosNodes);

        int areaNo = 0;
        foreach (GameObject[] nodes in allNodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                NodeDetails details = nodes[i].GetComponent<NodeDetails>();
                details.battleInfo = detailsOfAreas[areaNo].battles[i];
            }

            areaNo++;
        }
    }

    void ShowAllNodes()
    {
        GameObject[][] allNodes = this.allNodes.ToArray();

        List<GameObject> unlockedNodes = new List<GameObject>();
        for (int i = 0; i < detailsOfAreas.Length; i++)
        {
            if (!detailsOfAreas[i].isAreaLocked)
            {
                BattleInstance[] battles = detailsOfAreas[i].battles;
                for (int j = 0; j < battles.Length; j++)
                {
                    Debug.Log(battles[j].isLocked);
                    if (battles[j].isLocked)
                    {
                        Debug.Log("Show nodes...");

                        if(unlockedNodes.Count == 1)
                            StartCoroutine(ShowNodes(unlockedNodes.ToArray()));
                        return;
                    }
                    else
                    {
                        unlockedNodes.Add(allNodes[i][j]);
                        Debug.Log(allNodes[i][j]);
                    }
                }
            }
        }

        StartCoroutine(ShowNodes(unlockedNodes.ToArray()));
    }

    IEnumerator ShowNodes(GameObject[] unlockedNodes)
    {
        float popSpeed = nodePopSpeed;
        for (int i = 0; i < unlockedNodes.Length; i++)
        {
            DrawPath drawPath = unlockedNodes[i].GetComponent<DrawPath>();
            unlockedNodes[i].SetActive(true);

            yield return new WaitForSeconds(popSpeed);
            popSpeed -= 0.05f;

            /*if (!(i == (unlockedNodes.Length - 1)))
            {
                drawPath.DrawLine(unlockedNodes[i].transform.position, unlockedNodes[i + 1].transform.position, unlockedNodes.Length, 0, 1);
                drawPath.EnableDraw(true);

                yield return new WaitForSeconds(3f);
            }*/

            if ((i + 1) == unlockedNodes.Length)
            {
                compassJoystick.CanMoveMapAround(true);
            }
        }
    }
}
