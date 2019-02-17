﻿using System.Collections;
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
                dataController.LoadScavengers(dataController.currentSaveData.scavengerIdList);
            }
            else
            {
                // New game data
                dataController.CreateGameData();

                // Then create a save file for player
                dataController.NewSaveData();
            }

            StartCoroutine(LoadNextScene());
        }
    }

    IEnumerator LoadNextScene()
    {
        //yield return new WaitForSeconds(0.5f);
        //fadeTransition.GetComponent<Animator>().SetBool("Fade Out", true);
        //yield return new WaitForSeconds(0.5f);
        //fadeTransition.GetComponent<Animator>().SetBool("Fade Out", false);

        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(dataController.currentGameData.NextSceneId("Title Screen"));
    }

}