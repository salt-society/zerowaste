using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SaveGrid : MonoBehaviour
{
    public DataController dataController;

    [Space]
    public GameObject notePanel;

    [Space]
    public GameObject saveCell;

    [Space]
    public Sprite ryleighBoy;
    public Sprite ryleighBoySelected;
    public Sprite ryleighGirl;
    public Sprite ryleighGirlSelected;

    [Space]
    public List<Sprite> mapIcons;

    [Space]
    public string currentOperation = string.Empty;

    [Space]
    public GameObject fadeTransition;

    public void DisplaySaveDetails()
    {
        if (dataController == null)
            return;

        SaveData saveFile = dataController.currentSaveData;

        Sprite saveIcon, selectedIcon;
        if (saveFile.gender.Equals("Male"))
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
        saveCell.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Zero Waste Game";
        saveCell.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = saveFile.dateLastAccessed.ToShortDateString();

        saveCell.transform.GetChild(6).GetComponent<Image>().sprite = mapIcons[saveFile.currentAreaId];
        saveCell.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = saveFile.areas.Count + " Areas Unlocked";
        saveCell.transform.GetChild(8).GetComponent<TextMeshProUGUI>().text = saveFile.nodes.Count + " Nodes Unlocked";
    }

    public void ChooseOperation(int operationId)
    {
        if (string.IsNullOrEmpty(currentOperation))
        {
            if (operationId == 0)
            {
                currentOperation = "Save";
                ShowNotePanel("Overwrite save file?");
            }

            if (operationId == 1)
            {
                currentOperation = "Reset";
                ShowNotePanel("Are you sure you want to reset your progress?");
            }
        }
        else
        {
            currentOperation = string.Empty;
        }
    }

    public void Confirm()
    {
        if (currentOperation.Equals("Save"))
        {
            dataController.SaveSaveData();
            dataController.SaveGameData();

            currentOperation = string.Empty;
            ShowNotePanel("");
        }

        if (currentOperation.Equals("Reset"))
        {
            dataController.SaveSaveData();
            dataController.SaveGameData();

            dataController.NewSaveData();

            dataController.currentGameData.saves[0] = dataController.currentSaveData;
            dataController.currentGameData.saves.RemoveAt(1);
            dataController.SaveGameData();

            dataController.nextScene = dataController.GetNextSceneId("Loading Data");
            StartCoroutine(LoadScene());
        }
    }

    public void Cancel()
    {
        currentOperation = string.Empty;
        ShowNotePanel("");
    }

    public void ShowNotePanel(string message)
    {
        notePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        notePanel.SetActive(!notePanel.activeInHierarchy);    
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", true);
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", false);
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(dataController.GetNextSceneId("Loading"));
    }
}
