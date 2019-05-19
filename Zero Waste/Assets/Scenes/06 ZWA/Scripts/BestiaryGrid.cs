using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BestiaryGrid : MonoBehaviour
{
    public DataController dataController;

    [Space]
    public GameObject mutantDataPrefab;
    public GameObject unknownPrefab;

    [Space]
    public GameObject mutantInfoPanel;
    public GameObject instruction;
    public GameObject haventEncountered;
    public GameObject parentExit;

    [Space]
    public List<Sprite> areaSprites;
    public List<string> areaNames;
    public List<Sprite> mutantSelected;

    public void PopulateGrid()
    {
        if (dataController == null)
            return;

        GameObject mutantCell;

        if (dataController.currentSaveData.mutantsEncounteredList.Count > 0)
        {
            instruction.SetActive(true);
            haventEncountered.SetActive(false);

            List<Enemy> mutantList = dataController.allWasteList;
            for (int i = 0; i < mutantList.Count; i++)
            {
                if (dataController.currentSaveData.mutantsEncounteredList.Contains(mutantList[i].characterId)) 
                {
                    mutantCell = Instantiate(mutantDataPrefab, transform);

                    mutantCell.GetComponent<MutantInfo>().dataController = dataController;
                    mutantCell.GetComponent<MutantInfo>().mutant = mutantList[i];

                    mutantCell.GetComponent<MutantInfo>().mutantInfoPanel = mutantInfoPanel;
                    mutantCell.GetComponent<MutantInfo>().parentExit = parentExit;
                    mutantCell.GetComponent<MutantInfo>().areaEcountered = areaSprites[mutantList[i].areaEncountered];
                    mutantCell.GetComponent<MutantInfo>().areaName = areaNames[mutantList[i].areaEncountered];

                    mutantCell.transform.GetChild(0).GetComponent<Image>().sprite = mutantList[i].characterThumb;
                    mutantCell.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = mutantSelected[mutantList[i].characterId];
                    mutantCell.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = (mutantList[i].characterId + 1).ToString();
                }
                else
                {
                    mutantCell = Instantiate(unknownPrefab, transform);

                    mutantCell.transform.GetChild(1).GetComponent<Image>().sprite = mutantList[i].characterThumb;
                    mutantCell.transform.GetChild(0).GetComponent<Image>().sprite = mutantSelected[mutantList[i].characterId];
                }
            }
        }
        else
        {
            instruction.SetActive(false);
            haventEncountered.SetActive(true);
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
}
