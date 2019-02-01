using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectNode : MonoBehaviour {

    public NotificationManager notifManager;

    public Camera targetCamera;

    private GameObject previousSelectedNode;
    private GameObject selectedNode;
    private NodeDetails previousNodeDetails;
    private NodeDetails currentNodeDetails;

    void NodeSelection(Touch touch)
    {
        Vector2 touchPosition = targetCamera.ScreenToWorldPoint(touch.position);
        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

        Debug.Log(hit.collider != null);
        if (hit.collider != null)
        {
            selectedNode = hit.transform.gameObject;
            currentNodeDetails = selectedNode.GetComponent<NodeDetails>();

            ChangeNodeColor(selectedNode, currentNodeDetails.battleInfo.selectedColor);
            notifManager.ShowNotif(currentNodeDetails.battleInfo.name);
            Debug.Log(selectedNode.transform.position + " : " + touch.position);
        }
        Debug.Log(touch.position);
    }


    void ChangeNodeColor(GameObject node, Color color)
    {
        SpriteRenderer spriteRenderer = node.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
    }

    void DeselectNode()
    {
        previousSelectedNode = selectedNode;
        previousNodeDetails = currentNodeDetails;

        ChangeNodeColor(previousSelectedNode, previousNodeDetails.battleInfo.nodeColor);
    }

    void Update()
    {
        if (Input.touchCount.Equals(1))
        {
            Touch touch = Input.GetTouch(0);
            NodeSelection(touch);
        }
    }

}
