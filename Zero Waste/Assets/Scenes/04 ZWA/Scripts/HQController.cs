using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HQController : AreaController
{
    [Header("Unique in HQ")]
    public GameObject exitButton;

    [Space]
    public GameObject storyInfo;

    // HQ
    public void ShowHideExit()
    {
        exitButton.SetActive(!exitButton.activeInHierarchy);
    }

    public override IEnumerator ShowPart(PartIdentifier partIdentifier)
    {
        ShowHideExit();
        return base.ShowPart(partIdentifier);
    }

    public override IEnumerator ClosePart()
    {
        ShowHideExit();
        return base.ClosePart();
    }

    public void Close()
    {
        StartCoroutine(ClosePart());
    }

    // Story
    public void ReadStory()
    {
        if (dataController == null)
            return;


    }

    public void CloseStoryInfo()
    {
        if (dataController == null)
            return;

        dataController.currentStory.CloseStoryInfo();
    }

    public override void Update()
    {
        if (isInteractable)
        {
            if (Input.touchCount == 1)
            {
                if (TouchPhase.Began == Input.GetTouch(0).phase)
                {
                    Vector2 origin = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero);

                    if (hit.collider != null)
                    {
                        GameObject part = hit.transform.gameObject;
                        if (part.GetComponent<PartIdentifier>() != null)
                        {
                            PartIdentifier partIdentifier = part.GetComponent<PartIdentifier>();
                            StartCoroutine(ShowPart(partIdentifier));
                        }
                        else
                        {

                        }
                    }
                    else
                    {

                    }
                }
            }
        }
    }
}
