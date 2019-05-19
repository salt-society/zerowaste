using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveGrid : MonoBehaviour
{
    public DataController dataController;

    [Space]
    public GameObject savePrefab;

    [Space]
    public Sprite ryleighBoy;
    public Sprite ryleighBoySelected;
    public Sprite ryleighGirl;
    public Sprite ryleighGirlSelected;
    public Sprite emptySave;
    public Sprite emptySaveSelected;

    [Space]
    public List<Sprite> mapIcons;

    [Space]
    public SaveInfo selectedSave;

    public void PopulateGrid()
    {
        if (dataController == null)
            return;

        GameObject saveCell;
        for (int i = 0; i < dataController.currentGameData.maxNoOfSaveFiles; i++)
        {
            saveCell = Instantiate(savePrefab, transform);
            saveCell.GetComponent<SaveInfo>().saveCellId = i;
            saveCell.GetComponent<SaveInfo>().ryleighBoy = ryleighBoy;
            saveCell.GetComponent<SaveInfo>().ryleighBoySelected = ryleighBoySelected;
            saveCell.GetComponent<SaveInfo>().ryleighGirl = ryleighGirl;
            saveCell.GetComponent<SaveInfo>().ryleighGirlSelected = ryleighGirlSelected;
            saveCell.GetComponent<SaveInfo>().mapIcons = mapIcons;

            saveCell.GetComponent<SaveInfo>().savePopup = saveCell.transform.GetChild(9).gameObject;
            saveCell.GetComponent<SaveInfo>().saveGrid = this;

            List<SaveData> saveFiles = dataController.currentGameData.saves;
            if (i < saveFiles.Count)
            {
                Sprite saveIcon, selectedIcon;
                if (saveFiles[i].gender.Equals("Male")) 
                {
                    saveIcon = ryleighBoy;
                    selectedIcon = ryleighBoySelected;
                } 
                else 
                {
                    saveIcon = ryleighGirl;
                    selectedIcon = ryleighGirlSelected;
                }

                saveCell.transform.GetChild(0).GetComponent<Image>().sprite = selectedIcon;
                saveCell.transform.GetChild(1).GetComponent<Image>().sprite = saveIcon;
                saveCell.transform.GetChild(1).GetComponent<Image>().color = new Color(255, 255, 255, 1);
                saveCell.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Zero Waste Game 0" + i;
                saveCell.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = saveFiles[i].dateLastAccessed.ToShortDateString();

                saveCell.transform.GetChild(6).GetComponent<Image>().sprite = mapIcons[saveFiles[i].currentAreaId];
                saveCell.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = saveFiles[i].areas.Count + " Areas Unlocked";
                saveCell.transform.GetChild(8).GetComponent<TextMeshProUGUI>().text = saveFiles[i].nodes.Count + " Nodes Unlocked";


                if (dataController.currentGameData.currentSave.saveId == saveFiles[i].saveId)
                {
                    transform.GetChild(4).gameObject.SetActive(!transform.GetChild(4).gameObject.activeInHierarchy);
                    transform.GetChild(5).gameObject.SetActive(!transform.GetChild(5).gameObject.activeInHierarchy);
                }
            }
            else
            {
                saveCell.transform.GetChild(0).GetComponent<Image>().sprite = emptySaveSelected;
                saveCell.transform.GetChild(1).GetComponent<Image>().sprite = emptySave;
                saveCell.transform.GetChild(1).GetComponent<Image>().color = new Color(0, 0, 0, 1);
                saveCell.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Zero Waste Game 0" + i;
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

    public void ChooseOperation(int operationId)
    {
        if (operationId == 0)
        {

        }

        if (operationId == 1)
        {

        }

        if (operationId == 2)
        {

        }
    }
}
