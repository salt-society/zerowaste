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

            // If there's a current battle, check if this cutscene is
            // a major cutscene or not
            if (dataController.currentBattle != null)
            {
                // If major cutscene, treat this cutscene as a battle
                // Set battle as played to stop it from being unlocked all over again
                if (dataController.currentBattle.isMajorCutscene)
                    dataController.currentSaveData.PlayBattle(dataController.currentBattle.battleId);
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
            if (dataController.currentBattle != null)
            {
                // Get current id of battle and check if its a major cutscene or end cutscene of a battle
                int battleId = dataController.currentBattle.battleId;
                if (dataController.currentBattle.isMajorCutscene || dataController.currentBattle.cutsceneAtEnd)
                {
                    // Battles can be plain cutscenes or with cutscene at start/end
                    // If a battle is a major cutscene/end cutscene, mark it finished
                    // dataController.currentSaveData.FinishedBattle(battleId);

                    // Unlock next level(s)
                    // dataController.UnlockLevels(dataController.currentCutscene.nextLevels, dataController.currentCutscene.levelIds);

                    // Save whatever progress made
                    // dataController.SaveSaveData();
                    // dataController.SaveGameData();
                }
            }
            else
            {

                if (currentCutscene.cutsceneId == 0)
                {
                    dataController.currentBattle = dataController.allBattles[0];
                }

                // Cutscenes that are not part of battles, for example, prologue/epilogue/zwa
                // can unlock stuffs such as next level to make the game progress
                // dataController.currentSaveData.FinishedCutscene(currentCutscene.cutsceneId);
                // dataController.UnlockLevels(currentCutscene.nextLevels, currentCutscene.levelIds);

                // Save whatever progress made
                // dataController.SaveSaveData();
                // dataController.SaveGameData();
            }

            // Load next scene base on cutscene's nextSceneId
            nextSceneId = dataController.GetNextSceneId(currentCutscene.nextScene);
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", true);
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", false);

        SceneManager.LoadScene(nextSceneId);
    }
}
