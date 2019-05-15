using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryGrid : MonoBehaviour
{
    public DataController dataController;
    public GameObject storyPrefab;

    [Space]
    public List<Sprite> mapIcons;
    public List<Sprite> mapIconsSelected;
    public GameObject storyInfo;
    public GameObject parentExit;

    [Space]
    public List<Battle> zwaStory;

    public void PopulateGrid()
    {
        if (dataController == null)
            return;

        GameObject storyCell;

        for (int i = 0; i < zwaStory.Count; i++)
        {
            if (dataController.currentSaveData.battles.ContainsKey(zwaStory[i].battleId))
            {
                storyCell = Instantiate(storyPrefab, transform);

                storyCell.GetComponent<StoryLevel>().dataController = dataController;

                storyCell.GetComponent<StoryLevel>().story = zwaStory[i];
                storyCell.GetComponent<StoryLevel>().currentAreaSprite = mapIcons[zwaStory[i].node.area.areaId];
                storyCell.GetComponent<StoryLevel>().storySelected = storyCell.transform.GetChild(0).GetChild(0).gameObject;
                storyCell.GetComponent<StoryLevel>().storyInfo = storyInfo;
                storyCell.GetComponent<StoryLevel>().parentExit = parentExit;

                storyCell.transform.GetChild(0).GetComponent<Image>().sprite = mapIcons[zwaStory[i].node.area.areaId];
                storyCell.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = mapIconsSelected[zwaStory[i].node.area.areaId];
                storyCell.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = zwaStory[i].startCutscene.chapter;

                if(!dataController.currentSaveData.battles[zwaStory[i].battleId]) 
                {
                    storyCell.transform.GetChild(2).gameObject.SetActive(!storyCell.transform.GetChild(2).gameObject.activeInHierarchy);
                }
                else
                {
                    storyCell.transform.GetChild(2).gameObject.SetActive(false);
                }
            }
        }
    }

    public void RemoveCells()
    {
        if (dataController == null)
            return;

        if (transform.childCount > 0)
        {
            foreach (Transform cell in transform)
                Destroy(cell.gameObject);

            gameObject.SetActive(false);
        }
    }
}
