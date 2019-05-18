using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DisclaimerController : MonoBehaviour
{
    public GameObject disclaimer;
    public GameObject privacyPolicy;
    public ScrollRect scrollRect;
    public GameObject scrollGuide;
    public GameObject clickGuide;

    [Space]
    public bool clickGuideShown;

    [Space]
    public GameObject fadeTransition;
    public int nextScene;

    void Start()
    {
        StartCoroutine(ShowDisclaimerAndPP());
        clickGuideShown = false;
    }

    IEnumerator ShowDisclaimerAndPP()
    {
        disclaimer.SetActive(!disclaimer.activeInHierarchy);
        yield return new WaitForSeconds(2.0f);
        disclaimer.GetComponent<Animator>().SetBool("Fade Out", true);
        disclaimer.SetActive(!disclaimer.activeInHierarchy);

        privacyPolicy.SetActive(!privacyPolicy.activeInHierarchy);
        yield return new WaitForSeconds(0.5f);
        scrollGuide.SetActive(!scrollGuide.activeInHierarchy);
        yield return new WaitForSeconds(3.0f);
        scrollGuide.SetActive(!scrollGuide.activeInHierarchy);
    }

    public void ContinueGame() 
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", true);
        yield return new WaitForSeconds(2f);
        fadeTransition.GetComponent<Animator>().SetBool("Fade Out", false);

        SceneManager.LoadScene(nextScene);
    }

    public void Cancel()
    {
        Application.Quit();
    }

    IEnumerator ScrollEndTigger()
    {
        clickGuide.SetActive(!clickGuide.activeInHierarchy);
        yield return new WaitForSeconds(3.0f);
        clickGuide.SetActive(!clickGuide.activeInHierarchy);
    }

    public void RedirectUrl(string url)
    {
        Application.OpenURL(url);
    }

    void Update()
    {
        if (clickGuideShown)
            return;

        Vector2 scrollEnd = new Vector2(0f, 0f);
        if (scrollRect.normalizedPosition == scrollEnd)
        {
            StartCoroutine(ScrollEndTigger());
            clickGuideShown = true;
        }
    }
}
