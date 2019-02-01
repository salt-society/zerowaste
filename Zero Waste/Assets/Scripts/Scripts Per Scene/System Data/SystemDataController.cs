using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class SystemDataController : BaseSceneController {

	[Header("System Data")]
    [SerializeField]
    private string SYSTEM_DATA_FILE_NAME;
    [SerializeField]
    private string SYSDATA_EXT;
    [Space]
    [Header("Save Data")]
    [SerializeField]
    private string SAVE_DATA_FILE_NAME;
    [SerializeField]
    private string SAVE_EXT;
    [Space]
    [Header("Message Controller")]
    [SerializeField]
    private MessageController messageController;

    private Queue<int> messagesIndex;
    private bool checkingFinished;
    private bool dataExists;

    protected override void Start()
    {
        // common to all scenes
        base.Start();

        // unique to this scene
        // initialize
        checkingFinished = false;
        dataExists = false;

        // check system data
        CheckSystemData();
        StartCoroutine(DisplayMessages());
    }

	protected override void Update () 
    {
        // common to all scenes
        base.Update();

        // unique to this scene
        if (checkingFinished)
            StartCoroutine(base.LoadNextScene());
	}

    private void CheckSystemData()
    {
        // check if system data exists
        if(File.Exists(Application.persistentDataPath + "/" +
            SYSTEM_DATA_FILE_NAME + SYSDATA_EXT)) 
        {
            // display message
            dataExists = true;
            Debug.Log("Game data exists!");
        }
        else
        {
            // display message
            dataExists = false;
            Debug.Log("Persistent Data Path: " + Application.persistentDataPath); // logs

            // create system data
            CreateSystemData(); // creates one system data
            CreateSaveData(); // creates save files
        }
    }

    // create system data (contains player settings)
    private void CreateSystemData()
    {
        SystemData systemData = new SystemData(); // create an instance of system data
        
        BinaryFormatter binaryFormatter = new BinaryFormatter(); // convert to binary
        FileStream fileStream = File.Create(Application.persistentDataPath + "/" +
            SYSTEM_DATA_FILE_NAME + SYSDATA_EXT);
        binaryFormatter.Serialize(fileStream, systemData);
        fileStream.Close();

        // messageController.DisplayMessage(2);
        Debug.Log("System Data created."); // logs
    }

    // create save data (contains player progress)
    private void CreateSaveData()
    {
        // read system data
        if (File.Exists(Application.persistentDataPath + "/" +
            SYSTEM_DATA_FILE_NAME + SYSDATA_EXT))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(Application.persistentDataPath + "/" +
                SYSTEM_DATA_FILE_NAME + SYSDATA_EXT, FileMode.Open);
            SystemData systemData = (SystemData)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            Debug.Log("Reading system data."); // logs
            
            // create save data
            Debug.Log("Creating " + systemData.maxSaveFiles + " save data files..."); // logs

            for (int i = 1; i <= systemData.maxSaveFiles; i++)
            {
                SaveData save = new SaveData();
                binaryFormatter = new BinaryFormatter(); // convert to binary
                if (i < 10)
                {
                    fileStream = File.Create(Application.persistentDataPath + "/" +
                    SAVE_DATA_FILE_NAME + "0" + i + SAVE_EXT);

                    // file name
                    save.fileName = SAVE_DATA_FILE_NAME + "0" + i + SAVE_EXT;
                    save.fileName = Path.GetFileNameWithoutExtension(save.fileName);
                }
                else
                {
                    fileStream = File.Create(Application.persistentDataPath + "/" +
                    SAVE_DATA_FILE_NAME + i + SAVE_EXT);

                    // file name
                    save.fileName = SAVE_DATA_FILE_NAME + i + SAVE_EXT;
                    save.fileName = Path.GetFileNameWithoutExtension(save.fileName);
                }

                // date save created
                save.lastSaveDate = DateTime.Now.ToString();
                Debug.Log(save.lastSaveDate);

                binaryFormatter.Serialize(fileStream, save);
                fileStream.Close();
            }

            Debug.Log("Save data files created."); // logs
        }
    }

    IEnumerator DisplayMessages()
    {
        // first message
        messageController.DisplayMessage(0);
        yield return new WaitForSeconds(3f);

        // message after checking data
        if (dataExists)
        {
            messageController.DisplayMessage(3);
            checkingFinished = true;
        }
        else
        {
            messageController.DisplayMessage(1);
            yield return new WaitForSeconds(3f);
            messageController.DisplayMessage(2);
            checkingFinished = true;
        }
    }

    // by sh0
}
