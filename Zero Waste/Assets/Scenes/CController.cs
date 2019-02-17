using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CController : MonoBehaviour
{
    public DataController dataController;
    public Cutscene currentCutscene;

    [Space]
    public List<Cutscene> cutsceneList;
    public DManager dialogueManager;

    [Space]
    public GameObject historyBox;

    void Start()
    {
        dataController = GameObject.FindObjectOfType<DataController>();
        if (dataController != null)
        {
            if (dataController.currentCutscene != null)
            {
                currentCutscene = dataController.currentCutscene;
            }
            else
            {
                int cutsceneId = dataController.currentSaveData.currentCutscene;
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
        dialogueManager.SetDialogues(currentCutscene.dialogues);
        StartCoroutine(dialogueManager.DisplayDialogue());
    }

    public void ShowHistory()
    {
        if(dataController != null)
            GameObject.FindObjectOfType<AudioManager>().PlaySound("Rotating Button");
        historyBox.SetActive(!historyBox.activeInHierarchy);
        dialogueManager.canSkipDialogue = !dialogueManager.canSkipDialogue;
    }
}
