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
            /*if (dataController.nextScene == 0)
            {
                nextScene = dataController.GetNextSceneId("Splash Screen");
                StartCoroutine(LoadScene());
            }
            else if (dataController.nextScene == 1)
            {
                nextScene = dataController.GetNextSceneId("Disclaimer");
                StartCoroutine(LoadScene());
            }
            else if (dataController.nextScene == 3)
            {
                nextScene = dataController.GetNextSceneId("Loading Data");
                StartCoroutine(LoadScene());
            }
            else if (dataController.nextScene == 4)
            {
                nextScene = dataController.GetNextSceneId("Title Screen");
                StartCoroutine(LoadScene());
            }
            else if (dataController.nextScene == 5)
            {
                nextScene = dataController.GetNextSceneId("Cutscene");
                StartCoroutine(LoadScene());
            }
            else if (dataController.nextScene == 6)
            {
                nextScene = dataController.GetNextSceneId("ZWA");
                StartCoroutine(LoadScene());
            }
            else if (dataController.nextScene == 7)
            {
                nextScene = dataController.GetNextSceneId("Map");
                StartCoroutine(LoadScene());
            }
            else if (dataController.nextScene == 8)
            {
                nextScene = dataController.GetNextSceneId("Battle");
                StartCoroutine(LoadScene());
            }*/

            nextScene = dataController.nextScene;
            StartCoroutine(LoadScene());
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
