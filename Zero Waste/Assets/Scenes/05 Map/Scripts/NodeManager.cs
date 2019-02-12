using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeManager : MonoBehaviour
{
    private GameObject node, previousNode;
    public Battle battle;
    private Battle previousBattle;
    private MapController mapController;

    [Space]
    private TextMeshProUGUI focusedAreaName;
    private TextMeshProUGUI focusedSubname;

    [Space]
    private GameObject nodeDetails;

    private bool canSelectNode;

    public void SetBattleData(Battle battle, GameObject node)
    {
        this.battle = battle;
        this.node = node;
    }

    public void SetPreviousBattleData(Battle previousBattle, GameObject previousNode)
    {
        this.previousBattle = previousBattle;
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
                            Debug.Log("Clicked " + battle.battleName);
                            canSelectNode = false;

                            mapController.SetCurrentSelectedNode(node);
                            StartCoroutine(ShowNodeDetails());
                        }
                    }
                    else
                    {
                        Debug.Log("No node clicked.");
                    }
                }
            }
        }
    }

    IEnumerator ShowNodeDetails()
    {
        SetNodeDetails();

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

        nodeDetails.SetActive(true);
    }

    public IEnumerator HideNodeDetails()
    {
        canSelectNode = true;

        focusedAreaName.gameObject.SetActive(true);
        focusedSubname.gameObject.SetActive(true);

        nodeDetails.GetComponent<Animator>().SetBool("Exit", true);
        yield return new WaitForSeconds(1f);
        nodeDetails.SetActive(false);
        nodeDetails.GetComponent<Animator>().SetBool("Exit", false);

        RemovePointer(node);
        yield return new WaitForSeconds(.5f);
        PointCurrentNode(previousNode);
    }

    private void PointCurrentNode(GameObject node)
    {
        Transform pointerTransform = node.transform.GetChild(0);
        pointerTransform.gameObject.SetActive(true);
    }

    private void RemovePointer(GameObject node)
    {
        Transform pointerTransform = node.transform.GetChild(0);
        pointerTransform.gameObject.SetActive(false);
    }

    public void SetNodeDetails()
    {
        TextMeshProUGUI nodeName = nodeDetails.transform.GetChild(2).
            gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        nodeName.text = battle.battleName;

        TextMeshProUGUI description = nodeDetails.transform.GetChild(3).
            gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public IEnumerator HideNode()
    {
        node.transform.GetChild(0).gameObject.SetActive(false);
        node.GetComponent<Animator>().SetBool("Fade Out", true);

        if(battle.hasPath)
            node.transform.GetChild(1).gameObject.GetComponent<Animator>().SetBool("Lock Path", true);

        yield return new WaitForSeconds(1f);

        node.GetComponent<Animator>().SetBool("Fade Out", false);

        if(battle.hasPath)
            node.transform.GetChild(1).gameObject.GetComponent<Animator>().SetBool("Lock Path", false);

        node.SetActive(false);

        if(battle.hasPath)
            node.transform.GetChild(1).gameObject.SetActive(false);
    }
}
