using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ZWAController : MonoBehaviour
{
    private DataController dataController;
    private AudioManager audioManager;

    [Space]
    public List<GameObject> locations;
    public List<GameObject> locationDoorHighlights;
    public List<GameObject> doorTooltips;

    [Space]
    public List<GameObject> newIcons;
    
    [Space]
    public GameObject fadeObject;

    [Space]
    public GameObject scrapHolder;

    void Start()
    {
        dataController = FindObjectOfType<DataController>();
        audioManager = FindObjectOfType<AudioManager>();

        audioManager.PlaySound("Astral Journey");
        StartCoroutine(UpdateScrapHolder());
    }

    public void OpenLocation(int locId)
    {
        if (locations[locId].activeInHierarchy == false)
        {
            locations[locId].SetActive(!locations[locId].activeInHierarchy);
        }
        else
        {
            if (locId == 0)
            {
                locations[locId].SetActive(!locations[locId].activeInHierarchy);
                return;
            }
            else
            {
                StartCoroutine(CloseLocation(locId));
            }
        }
        
    }

    IEnumerator CloseLocation(int locId)
    {
        doorTooltips[locId].SetActive(!doorTooltips[locId].activeInHierarchy);
        locationDoorHighlights[locId].SetActive(!locationDoorHighlights[locId].activeInHierarchy);

        yield return new WaitForSeconds(0.75f);

        doorTooltips[locId].GetComponent<Animator>().SetBool("Hide", true);
        locationDoorHighlights[locId].SetActive(!locationDoorHighlights[locId].activeInHierarchy);

        yield return new WaitForSeconds(0.15f);
        doorTooltips[locId].SetActive(!doorTooltips[locId].activeInHierarchy);
        doorTooltips[locId].GetComponent<Animator>().SetBool("Hide", false);

        locations[locId].SetActive(!locations[locId].activeInHierarchy);
    }

    public void ScrapUpdate()
    {
        StartCoroutine(UpdateScrapHolder());
    }

    // Update Scrap Holder
    public IEnumerator UpdateScrapHolder()
    {
        int currentScrap =
            Convert.ToInt32(scrapHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);

        int updatedScrap = dataController.currentSaveData.scraps;

        int difference = updatedScrap - currentScrap;

        if(difference > 0)
        {
            int addPerUpdate = difference / 8;

            while(difference > 0)
            {
                if (difference > addPerUpdate)
                    currentScrap += addPerUpdate;

                else
                    currentScrap += difference;

                scrapHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = currentScrap.ToString();
                yield return new WaitForSeconds(0.2f);

                difference -= addPerUpdate;
            }
        }

        else if (difference < 0)
        {
            int minPerUpdate = difference / 8;

            while (difference < 0)
            {
                if (difference < minPerUpdate)
                    currentScrap += minPerUpdate;

                else
                    currentScrap += difference;

                scrapHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = currentScrap.ToString();
                yield return new WaitForSeconds(0.2f);

                difference -= minPerUpdate;
            }
        }
    }
}
