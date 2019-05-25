using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HQController : MonoBehaviour
{
    public DataController dataController;

    [Space]
    public List<GameObject> partHighlights;
    public List<GameObject> partNames;
    public List<GameObject> partContents;

    [Space]
    private int currentPartIndex;

    void Start()
    {
        StartCoroutine(DisplayPartNames());
    }

    IEnumerator DisplayPartNames()
    {
        foreach (GameObject tooltip in partNames)
            tooltip.SetActive(!tooltip.activeInHierarchy);

        yield return new WaitForSeconds(1.0f);

        foreach (GameObject tooltip in partNames)
            tooltip.GetComponent<Animator>().SetBool("Hide", true);

        yield return new WaitForSeconds(1.0f);

        foreach (GameObject tooltip in partNames)
        {
            tooltip.SetActive(!tooltip.activeInHierarchy);
            tooltip.GetComponent<Animator>().SetBool("Hide", false);
        }
    }

    public void OpenPart(int index)
    {
        partNames[index].SetActive(!partNames[index].activeInHierarchy);
        partHighlights[index].SetActive(!partHighlights[index].activeInHierarchy);
        currentPartIndex = index;
    }

    public void HidePart(int index)
    {
        StartCoroutine(HidePartIE(index));
    }

    IEnumerator HidePartIE(int index)
    {
        partNames[index].GetComponent<Animator>().SetBool("Hide", true);
        partHighlights[index].SetActive(!partHighlights[index].activeInHierarchy);

        yield return new WaitForSeconds(0.75f);

        partNames[index].SetActive(!partNames[index].activeInHierarchy);
        partNames[index].GetComponent<Animator>().SetBool("Hide", false);
    }

    public void OpenStory()
    {
        partContents[currentPartIndex].SetActive(!partContents[currentPartIndex].activeInHierarchy);

        if (partContents[currentPartIndex].activeInHierarchy == false)
            currentPartIndex = 0;
    }

    public void Map()
    {
        
    }

    
}
