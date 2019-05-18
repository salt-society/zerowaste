using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingController : MonoBehaviour
{
    public DataController dataController;

    [Space]
    public TextMeshProUGUI tips;
    public GameObject loadingPercentage;
    public int nextScene;

    void Start()
    {
        dataController = FindObjectOfType<DataController>();

        if (dataController != null)
        {
            

            if (dataController.nextScene == 0)
            {

            }
            else if (dataController.nextScene == 1)
            {

            }
            else if (dataController.nextScene == 3)
            {
                if (dataController.currentCutscene != null)
                {
                    nextScene = dataController.GetNextSceneId("Cutscene");
                    StartCoroutine(LoadScene());
                }
            }
            else if (dataController.nextScene == 4)
            {

            }
            else if (dataController.nextScene == 5)
            {

            }
            else if (dataController.nextScene == 6)
            {
                nextScene = dataController.GetNextSceneId("ZWA");
                StartCoroutine(LoadScene());
            }
            else if (dataController.nextScene == 7)
            {

            }
            else if (dataController.nextScene == 8)
            {

            }
        }
        else
        {
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(nextScene);
        async.allowSceneActivation = true;

        while (!async.isDone)
        {
            loadingPercentage.GetComponent<TextMeshProUGUI>().text = ((int)((async.progress * 100f) + 10f)).ToString() + "%";
            yield return null;
        }
    }
}
