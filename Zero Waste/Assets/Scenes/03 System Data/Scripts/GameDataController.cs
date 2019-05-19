using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameDataController : MonoBehaviour
{
    public DataController dataController;
    public TextMeshProUGUI message;

    [Space]
    public GameObject fadeTransition;

    void Start()
    {
        // Get reference to Data Controller
        dataController = GameObject.FindObjectOfType<DataController>();
        if (dataController != null)
        {
            // Check if there's already a Game Data 
            // and create if it doesn't exist
            if (dataController.GameDataExists())
            {
                message.text = "Game data exists. Loading your save...";
                dataController.ReadGameData();

                // Check if scavenger list is null
                if (dataController.currentSaveData.scavengerList != null)
                {
                    dataController.LoadScavengers(dataController.currentSaveData.scavengerList);
                    dataController.currentSaveData.LaunchGameDetails();
                }

                // Load next scene
                StartCoroutine(LoadScene());
            }
            else
            {
                StartCoroutine(CreateGameData());
            }
        }
    }

    IEnumerator CreateGameData()
    {
        message.text = "Creating game data...";
        yield return new WaitForSeconds(0.5f);

        dataController.CreateGameData();
        dataController.NewSaveData();
        message.text = "Game data created. Auto Save feature is on.";

        yield return new WaitForSeconds(0.5f);

        // Load next scene
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2f);
        AsyncOperation async = SceneManager.LoadSceneAsync(dataController.GetNextSceneId("Title Screen"));
        async.allowSceneActivation = true;

        while (!async.isDone)
        {
            yield return null;
        }
    }

    void PrintSaveDetails()
    {
        Debug.Log("Save File: " + dataController.currentSaveData.fileName);
        Debug.Log("Date Last Acessed: " + dataController.currentSaveData.dateLastAccessed);
        Debug.Log("Gender: " + dataController.currentSaveData.gender);
        Debug.Log("Game Progress");
        Debug.Log("");
        Debug.Log("Scavenger Count: " + dataController.currentSaveData.scavengerList.Count);
        Debug.Log("Area Count: " + dataController.currentSaveData.areas.Count);
        Debug.Log("Node Count: " + dataController.currentSaveData.nodes.Count);
        Debug.Log("Battle Count: " + dataController.currentSaveData.battles.Count);
    }

}
