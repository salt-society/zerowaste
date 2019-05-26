using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomController : MonoBehaviour
{
    public DataController dataController;

    [Space]
    public List<GameObject> partHighlights;
    public List<GameObject> partNames;
    public List<GameObject> partContents;
    public List<GameObject> partButtons;

    [Header("Shelf")]
    public GameObject shelfExitButton;
    public GameObject booksButtonPanel;

    [Space]
    public BestiaryGrid bestiaryGrid;
    public GameObject bestiaryPanel;

    [Space]
    public Image scavBookButton;
    public GameObject scavPanel;

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

    public void EnableDisableButtons()
    {
        foreach (GameObject button in partButtons)
            button.SetActive(!button.activeInHierarchy);
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

    public void ShowContent()
    {
        EnableDisableButtons();
        partContents[currentPartIndex].SetActive(!partContents[currentPartIndex].activeInHierarchy);

        if (partContents[currentPartIndex].activeInHierarchy == false)
            currentPartIndex = 0;
    }

    public void OpenScavengerBook()
    {
        // shelfExitButton.SetActive(!shelfExitButton.activeInHierarchy);
        booksButtonPanel.SetActive(!booksButtonPanel.activeInHierarchy);

        scavPanel.SetActive(!scavPanel.activeInHierarchy);
    }

    public void OpenBestiary()
    {
        shelfExitButton.SetActive(!shelfExitButton.activeInHierarchy);
        booksButtonPanel.SetActive(!booksButtonPanel.activeInHierarchy);

        bestiaryPanel.SetActive(!booksButtonPanel.activeInHierarchy);
    }
}
