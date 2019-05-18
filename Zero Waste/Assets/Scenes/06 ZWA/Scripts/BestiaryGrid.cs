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
    public List<Sprite> mutantSelected;

    public void PopulateGrid()
    {
        if (dataController == null)
            return;

        GameObject mutantCell;

        if (dataController.currentSaveData.mutantsEncounteredList.Count > 0)
        {
            List<Enemy> mutantList = dataController.allWasteList;
            for (int i = 0; i < mutantList.Count; i++)
            {
                if (dataController.currentSaveData.mutantsEncounteredList.Contains(mutantList[i].characterId)) 
                {
                    mutantCell = Instantiate(mutantDataPrefab, transform);



                    mutantCell.transform.GetChild(0).GetComponent<Image>().sprite = mutantList[i].characterThumb;
                    mutantCell.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = mutantSelected[mutantList[i].characterId];
                    mutantCell.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = mutantList[i].characterName;
                }
                else
                {
                    mutantCell = Instantiate(unknownPrefab, transform);

                }
            }
        }
        else
        {

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
