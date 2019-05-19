using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveInfo : MonoBehaviour
{
    public DataController dataController;
    public int saveCellId;
    public string operation;

    [Space]
    public Sprite ryleighBoy;
    public Sprite ryleighBoySelected;
    public Sprite ryleighGirl;
    public Sprite ryleighGirlSelected;
    public Sprite emptySave;
    public Sprite emptySaveSelected;
    public List<Sprite> mapIcons;

    [Space]
    public SaveGrid saveGrid;
    public SaveData saveData;

    [Space]
    public GameObject savePopup;

    public void SelectSave()
    {
        if (saveGrid.selectedSave == null)
        {
            HighlightSave();

            
        }
        else if (saveGrid.selectedSave == this)
        {

        }
        else
        {

        }
    }

    public void HighlightSave()
    {
        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeInHierarchy);
    }

    public void ShowPopup()
    {
        savePopup.SetActive(!savePopup.activeInHierarchy);
    }

    public void ChangeSaveInfo()
    {
        Sprite saveIcon, selectedIcon;
        if (saveData.gender.Equals("Male"))
        {
            saveIcon = ryleighBoy;
            selectedIcon = ryleighBoySelected;
        }
        else
        {
            saveIcon = ryleighGirl;
            selectedIcon = ryleighGirlSelected;
        }

        transform.GetChild(0).GetComponent<Image>().sprite = selectedIcon;
        transform.GetChild(1).GetComponent<Image>().sprite = saveIcon;
        transform.GetChild(1).GetComponent<Image>().color = new Color(255, 255, 255, 1);
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Zero Waste Game 0" + saveData.saveId;
        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = saveData.dateLastAccessed.ToShortDateString();

        transform.GetChild(6).GetComponent<Image>().sprite = mapIcons[saveData.currentAreaId];
        transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = saveData.areas.Count + " Areas Unlocked";
        transform.GetChild(8).GetComponent<TextMeshProUGUI>().text = saveData.nodes.Count + " Nodes Unlocked";

        transform.GetChild(4).gameObject.SetActive(!transform.GetChild(4).gameObject.activeInHierarchy);
        transform.GetChild(5).gameObject.SetActive(!transform.GetChild(5).gameObject.activeInHierarchy);
    }

    public void ChangeBadge()
    {
        transform.GetChild(4).gameObject.SetActive(!transform.GetChild(4).gameObject.activeInHierarchy);
        transform.GetChild(5).gameObject.SetActive(!transform.GetChild(5).gameObject.activeInHierarchy);
    }

    public void OverwriteSave()
    {
        if (dataController == null)
            return;

        dataController.SaveSaveData();
        dataController.SaveGameData();

        dataController.currentGameData.saves[saveCellId] = saveData;

        dataController.SaveSaveData();
        dataController.SaveGameData();
    }
}
