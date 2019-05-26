using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustceneController : MonoBehaviour
{
    public DataController dataController;
    public Cutscene currentCutscene;

    [Space]
    public List<Cutscene> cutsceneList;
    public DialogueManager dialogueManager;
    public BackgroundManager backgroundManager;

    [Space]
    public GameObject historyBox;

    [Space]
    public GameObject fadeTransition;

    private int nextSceneId;

    void Start()
    {
        // Get data controller
        dataController = GameObject.FindObjectOfType<DataController>();

        // Just to be safe, always check if there's a data controller before execution
        if (dataController != null)
        {
            // Check if there's a cutscene passed in data controller
            // If there's one, set that cutscene to current cutscene
            if (dataController.currentCutscene != null)
            {
                currentCutscene = dataController.currentCutscene;
            }
            // If none, play cutscene base on player's current scene id
            else
            {
                int cutsceneId = dataController.currentSaveData.currentCutsceneId;
                currentCutscene = cutsceneList[cutsceneId];
            }

            PlayCutscene();
        }
    }

    public void PlayCutscene()
    {
        // Change background
        backgroundManager.ChangeBackground(currentCutscene.firstBackground);

        // Set to cutscene dialogues and display
        dialogueManager.SetDialogues(currentCutscene.dialogues);
        StartCoroutine(dialogueManager.DisplayDialogue());
    }

    public void ShowHistory()
    {
        // Just to be safe, always check if there's a data controller before execution
        if (dataController == null)
            return;

        // Play SFX
        GameObject.FindObjectOfType<AudioManager>().PlaySound("Rotating Button");

        // Show history box
        historyBox.SetActive(!historyBox.activeInHierarchy);
        dialogueManager.historyOn = historyBox.activeInHierarchy;
    }

    public void CutsceneFinished()
    {
        // Just to be safe, always check if there's a data controller before execution
        if (dataController != null)
        {
            // If there is a current battle, it means this cutscene is either a start/end or major cutscene
            if (dataController.currentNode != null)
            {
                // Save whatever progress made
                dataController.SaveSaveData();
                dataController.SaveGameData();

                dataController.nextScene = dataController.GetNextSceneId("ZWA");
            }

            else
            {
                // Tutorial
                if (currentCutscene.cutsceneId == 0)
                {
                    // Place tutorial on currentBattle
                    dataController.currentNode = dataController.allNodes[0];
                    dataController.currentSaveData.currentNodeId = -1;
                    dataController.nextScene = dataController.GetNextSceneId("Battle");
                }
            }

            // Load next scene base on cutscene's nextSceneId
            nextSceneId = dataController.GetNextSceneId("Loading");
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", true);
        yield return new WaitForSeconds(1f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", false);
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(nextSceneId);
    }
}
