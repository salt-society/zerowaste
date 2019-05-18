using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryLevel : MonoBehaviour
{
    public DataController dataController;
    public Battle story;

    [Space]
    public GameObject storySelected;
    public GameObject storyInfo;

    [Space]
    public TextMeshProUGUI chapterNo;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public TextMeshProUGUI mapName;
    public Image mapIcon;

    [Space]
    public Sprite currentAreaSprite;

    [Space]
    public GameObject parentExit;

    public void InitializeStoryLevel()
    {
        chapterNo = storyInfo.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        title = storyInfo.transform.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>();
        description = storyInfo.transform.GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>();
        mapName = storyInfo.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        mapIcon = storyInfo.transform.GetChild(1).GetChild(1).GetComponent<Image>();

        if (dataController == null)
            return;
    }

    public void SelectStoryLevel()
    {
        if (dataController == null)
            return;

        HighlightStoryLevel();
        ShowStoryInfo();
        ShowHidePExit();

        dataController.currentBattle = story;
        dataController.currentStory = this;
    }

    public void HighlightStoryLevel()
    {
        storySelected.SetActive(!storySelected.activeInHierarchy);
    }

    public void ShowStoryInfo()
    {
        StartCoroutine(ShowStoryInfoIE());
    }

    public IEnumerator ShowStoryInfoIE() 
    {
        chapterNo.text = story.startCutscene.chapter;
        title.text = story.startCutscene.title;
        description.text = story.info;
        mapName.text = story.node.area.areaName;
        mapIcon.sprite = currentAreaSprite;

        yield return new WaitForSeconds(0.5f);

        storyInfo.SetActive(!storyInfo.activeInHierarchy);
    }

    public void ShowHidePExit()
    {
        parentExit.SetActive(!parentExit.activeInHierarchy);
    }

    public void CloseStoryInfo()
    {
        storyInfo.SetActive(!storyInfo.activeInHierarchy);
        HighlightStoryLevel();
        ShowHidePExit();

        dataController.currentStory = null;
        dataController.currentBattle = null;
    }
}
