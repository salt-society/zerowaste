using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSelectManager : MonoBehaviour {

    public NotificationManager notifManager;
    public CompassJoystick joystick;

    private bool userClicks = false;

    private Vector2 touchPosition;

    private GameObject currentNode;
    private GameObject prevNode;

    private NodeDetails currDetails;
    private NodeDetails prevDetails;

	void Update () {
        if (Input.GetMouseButton(0))
        {
            touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                userClicks = true;
            }
            else
            {
                userClicks = false;
            }

        }
        else
        {
            userClicks = false;
        }
	}

    void FixedUpdate()
    {
        if (userClicks && !joystick.IsMoving())
        {
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
            if (hit.collider != null)
            {
                GameObject node = hit.transform.gameObject;
                NodeDetails details = node.GetComponent<NodeDetails>();

                if (!details.battleInfo.isLocked)
                {

                    if (currentNode == null)
                    {
                        currentNode = node;
                        currDetails = details;

                        SelectNode();
                    }

                    else if (currentNode.name == node.name)
                    {
                        ChangeNodeColor(currentNode, currDetails.battleInfo.nodeColor);
                        currentNode = null;
                        currDetails = null;
                    }

                    else if (currentNode.name != node.name)
                    {
                        ChangeNodeColor(currentNode, currDetails.battleInfo.nodeColor);

                        currentNode = node;
                        currDetails = details;

                        SelectNode();
                    }

                }
                else
                {
                    if(currentNode != null) {
                        ChangeNodeColor(currentNode, currDetails.battleInfo.nodeColor);
                        currentNode = null;
                        currDetails = null;
                    }

                    notifManager.ShowAndHideNotif("Battle is locked.");
                }
            }
            else
            {
                Debug.Log("Raycast didn't hit any collider.");
            }
        }
    }

    void SelectNode()
    {
        ChangeNodeColor(currentNode, currDetails.battleInfo.selectedColor);
        notifManager.ShowAndHideNotif(currDetails.battleInfo.name);
    }

    void ChangeNodeColor(GameObject node, Color color)
    {
        SpriteRenderer spriteRenderer = node.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
    }

}
