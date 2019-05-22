using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZWAController : MonoBehaviour
{
    private DataController dataController;

    [Space]
    public List<GameObject> locations;
    public List<GameObject> locationDoorHighlights;
    public List<GameObject> doorTooltips;

    [Space]
    public List<GameObject> newIcons;
    
    [Space]
    public GameObject fadeObject;

    void Start()
    {
        dataController = FindObjectOfType<DataController>();
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
}
