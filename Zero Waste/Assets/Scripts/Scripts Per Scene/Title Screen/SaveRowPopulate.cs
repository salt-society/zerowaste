using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveRowPopulate : MonoBehaviour {

    [Header("Scene Controller")]
    [SerializeField]
    private TitleScreenController sceneController;
    [Space]
    [Header("Cell Prefab")]
    [SerializeField]
    private GameObject cell;
    [SerializeField]
    private Sprite notEmptyIcon;
    [SerializeField]
    private Sprite emptyIcon;
    [Space]
    [Header("Panels")]
    [SerializeField]
    private GameObject confirmation;
    [SerializeField]
    private GameObject message;

    private String buttonClicked;
    private int maxNoOfCells;
    private FileInfo[] fileDirectory;
    private SystemData systemData;
    private SaveData[] saveFiles;
    private SaveData[] notEmptySaves;

    void Start()
    {
        PrepareData();
        PopulateGrid();
    }

    public void PrepareData()
    {
        // initialize needed values to create a row of save files
        buttonClicked = sceneController.GetButtonClicked();
        systemData = sceneController.GetSystemData();
        maxNoOfCells = systemData.maxSaveFiles;
        fileDirectory = sceneController.GetSaveFileNames();
        saveFiles = sceneController.GetSaveData();
        notEmptySaves = sceneController.GetNotEmptySaveData();

        // turn of message on load panel if there's no
        // save files that aren't empty
        if (buttonClicked.Equals("Continue"))
        {
            if (notEmptySaves.Length > 0)
                message.SetActive(false);
        }
    }

    void PopulateGrid()
    {
        // objects need for creating a cell
        GameObject saveCell;
        TextMeshProUGUI[] texts;
        Image[] images;

        // get number of cells base on button clicked
        if (buttonClicked.Equals("New Game"))
        {
            maxNoOfCells = systemData.maxSaveFiles;
        }

        if (buttonClicked.Equals("Continue"))
        {
            maxNoOfCells = notEmptySaves.Length;
        }
            
        // create cells
        for (int i = 0; i < maxNoOfCells; i++)
        {
            // create a new instance of cell
            saveCell = (GameObject)Instantiate(cell, transform);

            // set the cell's save data
            SetSaveData(saveCell, i);

            // set the text values
            texts = saveCell.GetComponentsInChildren<TextMeshProUGUI>();
            SaveFileInfo(texts, i);

            // set the images of the cell
            images = saveCell.GetComponentsInChildren<Image>();
            SetSaveIcon(images, i);
        }
    }

    void SaveFileInfo(TextMeshProUGUI[] texts, int i)
    {
        // loop through all the texts and change
        // according to save data
        foreach (TextMeshProUGUI text in texts)
        {
            // set file name
            if (text.name.Equals("FileName"))
            {
                if (buttonClicked.Equals("New Game"))
                    text.text = saveFiles[i].fileName;

                if (buttonClicked.Equals("Continue"))
                    text.text = notEmptySaves[i].fileName;
            }
                

            // set last save date
            if (text.name.Equals("LastSaveDate"))
            {
                if (buttonClicked.Equals("New Game"))
                    text.text = saveFiles[i].lastSaveDate;

                if (buttonClicked.Equals("Continue"))
                    text.text = notEmptySaves[i].lastSaveDate;
            }
                
            // set player name
            if (text.name.Equals("PlayerName"))
            {
                if (buttonClicked.Equals("New Game"))
                    text.text = saveFiles[i].firstName + " " + saveFiles[i].lastName;

                if (buttonClicked.Equals("Continue"))
                    text.text = notEmptySaves[i].firstName + " " + notEmptySaves[i].lastName;
            }

            // set progress
            if (text.name.Equals("Progress"))
            {
                if (buttonClicked.Equals("New Game"))
                    text.text = "Progress: " + saveFiles[i].gameCompletion.ToString() + "% Complete";

                if (buttonClicked.Equals("Continue"))
                    text.text = "Progress: " + notEmptySaves[i].gameCompletion.ToString() + "% Complete";
            }
                
        }
    }

    void SetSaveIcon(Image[] images, int i)
    {
        foreach (Image image in images)
        {
            // get the image for save icon
            if (image.name.Equals("FileImage"))
            {
                // if save file is not empty, change icon
                if (buttonClicked.Equals("New Game"))
                    if (!saveFiles[i].isSaveEmpty)
                        image.sprite = notEmptyIcon;

                if (buttonClicked.Equals("Continue"))
                    if (!notEmptySaves[i].isSaveEmpty)
                        image.sprite = notEmptyIcon;
            }

            // set current save badge
            if (image.name.Equals("BadgeBorder"))
                SetCurrentSave(image, i);
        }
    }

    void SetCurrentSave(Image badge, int i)
    {
        // get current save text
        TextMeshProUGUI text = badge.GetComponentInChildren<TextMeshProUGUI>();

        // hide badge by changing alpha to 0
        badge.color = new Color(0, 0, 0, 0); // black
        text.color = new Color(255, 255, 255, 0); // white

        // check if save file is current save
        if (buttonClicked.Equals("New Game"))
        {
            if (systemData.currentSave == null)
                return;

            if (systemData.currentSaveName.Equals(saveFiles[i].fileName + ".save"))
            {
                // change alpha to show badge
                badge.color = new Color(0, 0, 0, 0.75f);
                text.color = new Color(255, 255, 255, 255);
            }
        }

        if (buttonClicked.Equals("Continue"))
        {
            if (systemData.currentSave == null)
                return;

            if (systemData.currentSaveName.Equals(notEmptySaves[i].fileName + ".save"))
            {
                // change alpha to show badge
                badge.color = new Color(0, 0, 0, 0.75f);
                text.color = new Color(255, 255, 255, 255);
            }
        }
        
        
    }

    void SetSaveData(GameObject cell, int i)
    {
        // get the filename with extension
        String fileName = Path.GetFileName(fileDirectory[i].ToString());

        // pass values to SaveRowClick script
        cell.GetComponentInChildren<SaveRowClick>().SetConfirmationPanel(confirmation);
        cell.GetComponentInChildren<SaveRowClick>().SetButtonClicked(buttonClicked);
        cell.GetComponentInChildren<SaveRowClick>().SetFileName(fileName);
        cell.GetComponentInChildren<SaveRowClick>().SetSystemData(systemData);
        cell.GetComponentInChildren<SaveRowClick>().SetSceenController(sceneController);

        if (buttonClicked.Equals("New Game"))
            cell.GetComponentInChildren<SaveRowClick>().SetSaveData(saveFiles[i]);

        if (buttonClicked.Equals("Continue"))
            cell.GetComponentInChildren<SaveRowClick>().SetSaveData(notEmptySaves[i]);
    }
}
