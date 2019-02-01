using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveRowClick : MonoBehaviour {

    private GameObject confirmation;
    private String fileName;
    private SaveData saveFile;
    private SystemData systemData;

    private string SYSTEM_DATA_FILE_NAME = "SystemData";
    private string SYSDATA_EXT = ".sysdata";

    private TitleScreenController sceneController;
    private String buttonClicked;

    public void ShowButtonInfo()
    {
        // show on console
        Debug.Log("File Name: " + fileName);
        Debug.Log("Is Empty? " + saveFile.isSaveEmpty);
    }

    public void ShowConfirmation()
    {
        confirmation.SetActive(true);
    }

    public void HideConfirmation()
    {
        confirmation.SetActive(false);
    }

    private void ClosePanel()
    {
        int panelNo = 0;
        if (buttonClicked.Equals("New Game"))
            panelNo = 0;

        if (buttonClicked.Equals("Continue"))
            panelNo = 1;

        sceneController.ClosePanel(panelNo);
    }

    public void AddOnClickListeners()
    {
        // get all buttons in a row
        Button[] buttons = confirmation.GetComponentsInChildren<Button>();

        // loop through all buttons
        foreach (Button button in buttons)
        {
            // if button is named cancel, add hide function
            if (button.name.Equals("BtnCancel"))
                button.onClick.AddListener(HideConfirmation);

            if (button.name.Equals("BtnSave"))
            {
                if (buttonClicked.Equals("New Game"))
                {
                    button.onClick.AddListener(OverwriteData);
                    button.onClick.AddListener(HideConfirmation);
                    button.onClick.AddListener(ClosePanel);
                }

                if (buttonClicked.Equals("Continue"))
                {
                    button.onClick.AddListener(LoadData);
                    button.onClick.AddListener(HideConfirmation);
                    button.onClick.AddListener(ClosePanel);
                }
            }
        }
    }

    private void OverwriteData()
    {
        // set save data to not empty
        saveFile.isSaveEmpty = false;

        // set last save date
        saveFile.lastSaveDate = DateTime.Now.ToString();

        // set current save data
        systemData.currentSave = saveFile;
        systemData.currentSaveName = fileName;

        // overwrite system data
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(Application.persistentDataPath + "/" +
            SYSTEM_DATA_FILE_NAME + SYSDATA_EXT);
        binaryFormatter.Serialize(fileStream, systemData);
        fileStream.Close();

        // overwrite save data
        fileStream = File.Create(Application.persistentDataPath + "/" + fileName);
        binaryFormatter.Serialize(fileStream, saveFile);
        fileStream.Close();
    }

    private void LoadData()
    {
        // set current save data
        systemData.currentSave = saveFile;
        systemData.currentSaveName = fileName;

        // overwrite system data
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(Application.persistentDataPath + "/" +
            SYSTEM_DATA_FILE_NAME + SYSDATA_EXT);
        binaryFormatter.Serialize(fileStream, systemData);
        fileStream.Close();
    }

    // getter and setters
    public void SetSceenController(TitleScreenController sceneController)
    {
        this.sceneController = sceneController;
    }

    public void SetButtonClicked(String buttonClicked)
    {
        this.buttonClicked = buttonClicked;
    }

    public void SetFileName(String fileName)
    {
        this.fileName = fileName;
    } 

    public void SetSystemData(SystemData systemData)
    {
        this.systemData = systemData;
    }

    public void SetSaveData(SaveData saveFile)
    {
        this.saveFile = saveFile;
    }

    public void SetConfirmationPanel(GameObject confirmation)
    {
        this.confirmation = confirmation;
    }
}
