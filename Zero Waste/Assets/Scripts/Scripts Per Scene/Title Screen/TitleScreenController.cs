using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using TMPro;

public class TitleScreenController : BaseSceneController {

    [Space]
    [Header("Button Functions")]
    [SerializeField]
    private Button[] menuButtons;
    [Space]
    [Header("Overlay Panels")]
    [SerializeField]
    private GameObject[] overlayPanels;
    [SerializeField]
    private Animator[] animators;
    [Space]
    [Header("Information")]
    [SerializeField]
    private TextMeshProUGUI currentVersion;
    [Space]
    [Header("System Data")]
    [SerializeField]
    private string SYSTEM_DATA_FILE_NAME;
    [SerializeField]
    private string SYSDATA_EXT;

    private String buttonClicked;
    private Color active, inactive;
    private SystemData systemData;
    private SaveData currentSave;
    private SaveData[] savefiles;
    private SaveData[] notEmptySaves;

    protected override void Start()
    {
        base.Start();

        // get and deserialize system data
        DeserializeSystemData();

        // get colors
        active = new Color(menuButtons[0].image.color.r, menuButtons[0].image.color.g,
            menuButtons[0].image.color.b, menuButtons[0].image.color.a);
        inactive = new Color(menuButtons[0].image.color.r, menuButtons[0].image.color.g,
            menuButtons[0].image.color.b, 0);
    }

    protected override void Update()
    {
        base.Update();
    }

    public void DeserializeSystemData()
    {
        // read system data
        if (File.Exists(Application.persistentDataPath + "/" +
            SYSTEM_DATA_FILE_NAME + SYSDATA_EXT))
        {
            Debug.Log("Reading system data..."); // logs

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(Application.persistentDataPath + "/" +
                SYSTEM_DATA_FILE_NAME + SYSDATA_EXT, FileMode.Open);

            // get system data
            systemData = (SystemData)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
        }
    }

    public FileInfo[] GetSaveFileNames()
    {
        // get all save files
        String persistentDataPath = Application.persistentDataPath + "/";
        DirectoryInfo directory = new DirectoryInfo(persistentDataPath);
        FileInfo[] fileNames = directory.GetFiles("*.save");

        Debug.Log("Save Files: ");
        foreach (FileInfo fileName in fileNames)
            Debug.Log(fileName.ToString()); // logs

        return fileNames;
    }

    public void GetSaves(FileInfo[] fileNames)
    {
        if (File.Exists(Application.persistentDataPath + "/" +
            SYSTEM_DATA_FILE_NAME + SYSDATA_EXT))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            int index = 0;

            foreach (FileInfo fileName in fileNames)
            {
                String name = Path.GetFileName(fileName.ToString());
                FileStream fileStream = File.Open(Application.persistentDataPath +
                    "/" + name, FileMode.Open);
                savefiles[index] = (SaveData)binaryFormatter.Deserialize(fileStream);
                index++;
                fileStream.Close();
            }
        }
    }

    public void GetNotEmptySaves(FileInfo[] fileNames)
    {
        List<SaveData> notEmptySaves = new List<SaveData>();

        // just to be sure, check if there's a system data
        if (File.Exists(Application.persistentDataPath + "/" +
            SYSTEM_DATA_FILE_NAME + SYSDATA_EXT)) {

            BinaryFormatter binaryFormatter = new BinaryFormatter();

            // loop through all the file directories
            foreach (FileInfo fileName in fileNames)
            {
                String name = Path.GetFileName(fileName.ToString());
                FileStream fileStream = File.Open(Application.persistentDataPath +
                    "/" + name, FileMode.Open);

                // check if save is not empty
                SaveData save = (SaveData)binaryFormatter.Deserialize(fileStream);
                if (!save.isSaveEmpty)
                {
                    notEmptySaves.Add(save);
                    Debug.Log(fileName);
                }
                
                fileStream.Close();
            }

            // convert list to array
            this.notEmptySaves = notEmptySaves.ToArray();
        }
    }

    public void ButtonClick(int btnNo)
    {
        // loop through buttons and change alphas
        for (int i = 0; i < menuButtons.Length; i++)
        {
            if (i.Equals(btnNo))
                menuButtons[btnNo].image.color = active; // active
            else
                menuButtons[i].image.color = inactive; // inactive
        }

        // function for each button
        switch (btnNo)
        {
            case 0:
                {
                    buttonClicked = "New Game";
                    NewGame(btnNo);
                    break;
                }
            case 1:
                {
                    buttonClicked = "Continue";
                    Continue(btnNo);
                    break;
                }
            default:
                {
                    // do nothing
                    break;
                }
        }
    }

    // function called for creating new save
    public void NewGame(int panelNo)
    {
        // instantiate save files array
        savefiles = new SaveData[systemData.maxSaveFiles];

        // get save file names
        FileInfo[] fileNames = GetSaveFileNames();
        GetSaves(fileNames);

        // show panel
        // SaveRowPopulate saveGridController = overlayPanels[panelNo].GetComponentInChildren<SaveRowPopulate>();
        // saveGridController.PrepareData();

        overlayPanels[panelNo].SetActive(true);
        if (enableAnimation[panelNo]) // if enabled, show animation
            animators[panelNo].SetTrigger(animationEvents[panelNo].animationName);
    }

    public void Continue(int panelNo)
    {
        // get save file names
        FileInfo[] fileNames = GetSaveFileNames();
        GetNotEmptySaves(fileNames);

        // show panel
        // SaveRowPopulate saveGridController = overlayPanels[panelNo].GetComponentInChildren<SaveRowPopulate>();
        // saveGridController.PrepareData();

        overlayPanels[panelNo].SetActive(true);
        if (enableAnimation[panelNo]) // if enabled, show animation
            animators[panelNo].SetTrigger(animationEvents[panelNo].animationName);
    }

    public void StartGame()
    {

    }

    // closes overlay panel
    public void ClosePanel(int btnNo)
    {
        overlayPanels[btnNo].SetActive(false);
    }

    // getters and setters
    // returns what button is clided
    public String GetButtonClicked()
    {
        return buttonClicked;
    }

    // returns system data
    public SystemData GetSystemData()
    {
        return systemData;
    }

    // returns save data
    public SaveData[] GetSaveData()
    {
        return savefiles;
    }

    public SaveData[] GetNotEmptySaveData()
    {
        return notEmptySaves;
    }
}
