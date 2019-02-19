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
                PrintSave();
            }
            else
            {
                // New game data
                dataController.CreateGameData();

                // Then create a save file for player
                dataController.NewSaveData();
            }

            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(dataController.currentGameData.NextSceneId("Title Screen"));
    }

    void PrintSave()
    {
        Debug.Log("Save File: " + dataController.currentSaveData.fileName);
        Debug.Log("Date Last Acessed: " + dataController.currentSaveData.dateLastAccessed);
        Debug.Log("Gender: " + dataController.currentSaveData.gender);
        Debug.Log("Game Progress");
        Debug.Log("");
        Debug.Log("Scavenger Count: " + dataController.currentSaveData.scavengerList.Count);
        Debug.Log("Current Area: " + dataController.currentSaveData.currentAreaId);
        Debug.Log("Current Node: " + dataController.currentSaveData.currentNodeId);
        Debug.Log("Current Battle: " + dataController.currentSaveData.currentBattleId);
    }

}
