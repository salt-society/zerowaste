using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQController : AreaController
{
    [Header("Unique in HQ")]
    public GameObject exitButton;

    [Space]
    public GameObject storyInfo;
    public GameObject storyGrid;
    public GameObject parentExit;

    private string currentPartName;

    public override void Start()
    {
        base.Start();
    }

    // HQ
    public void ShowHideExit()
    {
        exitButton.SetActive(!exitButton.activeInHierarchy);
    }

    public override IEnumerator ShowPartIE(PartIdentifier partIdentifier)
    {
        ShowHideExit();
        currentPartName = partIdentifier.partName;

        if (partIdentifier.partName.Equals("Story"))
        {
            storyGrid.GetComponent<StoryGrid>().dataController = dataController;
            storyGrid.GetComponent<StoryGrid>().storyInfo = storyInfo;
            storyGrid.GetComponent<StoryGrid>().parentExit = parentExit;
            storyGrid.GetComponent<StoryGrid>().PopulateGrid();
        }
        else if (partIdentifier.partName.Equals("Map"))
        {
            dataController.nextScene = dataController.GetNextSceneId("Map");
            StartCoroutine(LoadScene());
        }
        
        return base.ShowPartIE(partIdentifier);
    }

    public override IEnumerator ClosePartIE()
    {
        ShowHideExit();

        if (currentPartName.Equals("Story"))
        {
            storyGrid.GetComponent<StoryGrid>().RemoveCells();
        }

        return base.ClosePartIE();
    }

    public void Close()
    {
        StartCoroutine(ClosePartIE());
    }

    public override void CloseArea()
    {
        base.CloseArea();
    }

    // Story
    public void ReadStory()
    {
        if (dataController == null)
            return;

        dataController.nextScene = dataController.GetNextSceneId(dataController.currentStory.story.nextScene);
        dataController.currentBattle = dataController.currentStory.story;
        dataController.currentCutscene = dataController.currentStory.story.startCutscene;
        StartCoroutine(LoadScene());
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
                            StartCoroutine(ShowPartIE(partIdentifier));
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
