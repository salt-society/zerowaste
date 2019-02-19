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
    private bool historyOpen;

    void Start()
    {
        historyOpen = false;

        dataController = GameObject.FindObjectOfType<DataController>();
        if (dataController != null)
        {
            if (dataController.currentCutscene != null)
            {
                currentCutscene = dataController.currentCutscene;
            }
            else
            {
                int cutsceneId = dataController.currentSaveData.currentCutsceneId;
                currentCutscene = cutsceneList[cutsceneId];
            }

            PlayCutscene();
        }
        else
        {
            currentCutscene = cutsceneList[0];
            PlayCutscene();
        }
    }

    public void PlayCutscene()
    {
        backgroundManager.ChangeBackground(currentCutscene.firstBackground);
        dialogueManager.SetDialogues(currentCutscene.dialogues);
        StartCoroutine(dialogueManager.DisplayDialogue());
    }

    public void ShowHistory()
    {
        if(dataController != null)
            GameObject.FindObjectOfType<AudioManager>().PlaySound("Rotating Button");
        historyBox.SetActive(!historyBox.activeInHierarchy);
        dialogueManager.canSkipDialogue = !historyOpen;
    }

    public void CutsceneFinished()
    {
        if (dataController != null)
        {
            Debug.Log(currentCutscene.chapter + " finished.");

            dataController.currentSaveData.FinishedCutscene();
            dataController.currentSaveData.currentCutsceneId++;

            nextSceneId = dataController.currentGameData.NextSceneId(currentCutscene.nextLevel);
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
