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
    public List<Sprite> mapIcons;

    [Space]
    public GameObject parentExit;

    public void SelectStoryLevel()
    {
        if (dataController == null)
            return;

        HighlightStoryLevel();
        ShowStoryInfo();
        ShowHidePExit();

        dataController.currentBattle = story;
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
        mapName.text = story.node.area.areaName;
        mapIcon.sprite = mapIcons[story.node.area.areaId];

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

        dataController.currentBattle = null;
        dataController.currentStory = null;
    }
}
