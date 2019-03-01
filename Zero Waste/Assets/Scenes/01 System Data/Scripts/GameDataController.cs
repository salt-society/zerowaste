using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataController : MonoBehaviour
{
    [Header("Data Controller")]
    public DataController dataController;

    [Header("Transition Components")]
    public GameObject fadeTransition;

    void Start()
    {
        // Get reference to Data Controller
        dataController = GameObject.FindObjectOfType<DataController>();
        if (dataController != null)
        {
            Debug.Log("Game Data Exists: " + dataController.GameDataExists());

            // Check if there's already a Game Data 
            // and create if it doesn't exist
            if (dataController.GameDataExists())
            {
                dataController.ReadGameData();
                dataController.LoadScavengers(dataController.currentSaveData.scavengerList);
                dataController.currentSaveData.LaunchGameDetails();
                PrintSaveDetails();
            }
            else
            {
                // New game data
                dataController.CreateGameData();

                // Then create a save file for player
                dataController.NewSaveData();
            }

            // Load next scene
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(dataController.GetNextSceneId("Title Screen"));
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
