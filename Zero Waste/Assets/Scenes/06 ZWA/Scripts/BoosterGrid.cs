using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoosterGrid : MonoBehaviour
{
    public DataController dataController;

    [Space]
    public GameObject instruction;
    public GameObject boosterInfo;

    [Space]
    public GameObject boosterPrefab;

    public void PopulateGrid()
    {
        if (dataController == null)
            return;

        GameObject boosterCell;

        RemoveCells();

        List<Booster> boosterList = dataController.boosters;
        for (int i = 0; i < boosterList.Count; i++)
        {
            if (dataController.currentSaveData.boosterList.ContainsKey(boosterList[i].boosterId)) 
            {
                boosterCell = Instantiate(boosterPrefab, transform);

                boosterCell.GetComponent<BoosterInfo>().dataController = dataController;
                boosterCell.GetComponent<BoosterInfo>().booster = boosterList[i];
                boosterCell.GetComponent<BoosterInfo>().quantity = dataController.currentSaveData.boosterList[boosterList[i].boosterId];

                boosterCell.GetComponent<BoosterInfo>().instruction = instruction;
                boosterCell.GetComponent<BoosterInfo>().boosterInfo = boosterInfo;

                boosterCell.transform.GetChild(0).GetComponent<Image>().sprite = boosterList[i].icon;
                boosterCell.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = boosterList[i].selectedIcon;

                boosterCell.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    dataController.currentSaveData.boosterList[boosterList[i].boosterId].ToString();

                boosterCell.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = boosterList[i].boosterName;
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
        }
    }

    private void Awake()
    {
        dataController = FindObjectOfType<DataController>();
    }
}
