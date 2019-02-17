using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DManager : MonoBehaviour
{
    public DataController dataController;
    public BManager backgroundManager;
    public HistoryGrid historyGrid;

    [Space]
    public GameObject dialogueBox;
    public GameObject narrationBox;
    public GameObject itemBox;
    public GameObject choiceBox;
    public GameObject notification;

    [Space]
    public bool canSkipDialogue;

    private List<Dialogue> dialogues;
    private Dialogue currentDialogue;
    private int dialogueIndex;

    private bool dialogueFinished;

    void Start()
    {
        dataController = GameObject.FindObjectOfType<DataController>();
    }

    public void SetDialogues(List<Dialogue> dialogues)
    {
        this.dialogues = dialogues;

        dialogueIndex = 0;
        currentDialogue = dialogues[dialogueIndex];

        canSkipDialogue = true;
        dialogueFinished = false;
    }

    public IEnumerator DisplayDialogue()
    {
        // State of dialogue as of the moment
        TextMeshProUGUI commonText;
        dialogueFinished = false;

        canSkipDialogue = !currentDialogue.withChoices;

        // Check type if narration or dialogue
        if (currentDialogue.isNarration)
        {
            if (dialogueBox.activeInHierarchy)
            {
                dialogueBox.transform.GetChild(1).gameObject.
                    GetComponent<Animator>().SetBool("Fade Out", true);
                dialogueBox.GetComponent<Animator>().SetBool("Down", true);
                yield return new WaitForSeconds(0.5f);
                dialogueBox.transform.GetChild(1).gameObject.
                    GetComponent<Animator>().SetBool("Fade Out", false);
                dialogueBox.GetComponent<Animator>().SetBool("Down", false);

                dialogueBox.transform.GetChild(1).gameObject.SetActive(false);
                dialogueBox.SetActive(false);
            }           

            if (!narrationBox.activeInHierarchy)
            {
                narrationBox.transform.GetChild(0).gameObject.
                    GetComponent<TextMeshProUGUI>().text = string.Empty;
                narrationBox.SetActive(true);
            }
            else
            {
                narrationBox.transform.GetChild(0).gameObject.
                    GetComponent<TextMeshProUGUI>().text = string.Empty;
            }

            commonText = narrationBox.transform.GetChild(0).gameObject.
                GetComponent<TextMeshProUGUI>();
        }
        else
        {
            if (narrationBox.activeInHierarchy)
            {
                narrationBox.GetComponent<Animator>().SetBool("Fade Out", true);
                yield return new WaitForSeconds(0.5f);
                narrationBox.GetComponent<Animator>().SetBool("Fade Out", false);
                narrationBox.SetActive(false);
            }

            if (!dialogueBox.activeInHierarchy)
            {
                dialogueBox.transform.GetChild(1).gameObject.
                    GetComponent<TextMeshProUGUI>().text = string.Empty;
                dialogueBox.SetActive(true);
            }
            else
            {
                dialogueBox.transform.GetChild(1).gameObject.
                    GetComponent<TextMeshProUGUI>().text = string.Empty;
            }

            if (currentDialogue.characterHalf == null)
            {
                dialogueBox.transform.GetChild(3).GetChild(0).GetChild(1).gameObject.SetActive(false);
                dialogueBox.transform.GetChild(3).GetChild(0).GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                dialogueBox.transform.GetChild(3).GetChild(0).GetChild(1).gameObject.
                    GetComponent<Image>().sprite = currentDialogue.characterHalf;

                yield return null;

                dialogueBox.transform.GetChild(3).GetChild(0).GetChild(1).gameObject.SetActive(true);
                dialogueBox.transform.GetChild(3).GetChild(0).GetChild(0).gameObject.SetActive(false);
            }

            dialogueBox.transform.GetChild(2).GetChild(0).GetChild(0).gameObject.
                GetComponent<TextMeshProUGUI>().text = currentDialogue.characterName;

            commonText = dialogueBox.transform.GetChild(1).gameObject.
                GetComponent<TextMeshProUGUI>();
        }

        // Change scene background
        StartCoroutine(backgroundManager.ChangeBackground(currentDialogue.background,
            currentDialogue.backgroundColor));

        yield return new WaitForSeconds(1.5f);
        commonText.gameObject.SetActive(true);

        char[] characterArray = currentDialogue.content.ToCharArray();
        int lastIndex = characterArray.Length - 1;

        int letterCount = 0;
        foreach (char letter in characterArray)
        {
            commonText.text += letter;
            yield return new WaitForSeconds(0.02f);
            letterCount++;

            if (lastIndex == letterCount)
            {
                if (currentDialogue.withChoices)
                {
                    canSkipDialogue = false;
                    ShowChoices();
                }

                historyGrid.AddCell(currentDialogue, null, false);
                dialogueFinished = true;
            }
        }
    }

    void DisplayFullDialogue()
    {
        if (currentDialogue.isNarration)
        {
            historyGrid.AddCell(currentDialogue, null, false);
            narrationBox.transform.GetChild(0).gameObject.
                    GetComponent<TextMeshProUGUI>().text = currentDialogue.content;
            dialogueFinished = true;
        }

        if (!currentDialogue.isNarration)
        {
            historyGrid.AddCell(currentDialogue, null, false);
            dialogueBox.transform.GetChild(1).gameObject.
                GetComponent<TextMeshProUGUI>().text = currentDialogue.content;
            dialogueFinished = true;
        }
    }

    public void ShowChoices()
    {
        choiceBox.transform.GetChild(0).GetChild(0).
            gameObject.GetComponent<TextMeshProUGUI>().text = currentDialogue.choices[0].choice;
        choiceBox.transform.GetChild(1).GetChild(0).
            gameObject.GetComponent<TextMeshProUGUI>().text = currentDialogue.choices[1].choice;

        choiceBox.SetActive(true);
    }

    public void SelectChoice(int choice)
    {
        if (dataController != null)
            GameObject.FindObjectOfType<DataController>().
                battleModifiers = currentDialogue.choices[choice].effects;

        if (choice == 0)
        {
            choiceBox.transform.GetChild(0).gameObject.
                GetComponent<Image>().color = currentDialogue.choices[choice].color;
        }

        if (choice == 1)
        {
            choiceBox.transform.GetChild(1).gameObject.
                GetComponent<Image>().color = currentDialogue.choices[choice].color;
        }

        historyGrid.AddCell(null, currentDialogue.choices[choice], true);
        StartCoroutine(HideChoices(currentDialogue.choices[choice].response));
    }

    IEnumerator HideChoices(Dialogue response)
    {
        yield return new WaitForSeconds(1f);
        choiceBox.GetComponent<Animator>().SetBool("Slide Out", true);
        yield return new WaitForSeconds(1f);
        choiceBox.GetComponent<Animator>().SetBool("Slide Out", false);
        choiceBox.SetActive(false);

        ShowChoiceResponse(response);
    }

    void ShowChoiceResponse(Dialogue response)
    {
        dialogueBox.transform.GetChild(2).GetChild(0).GetChild(0).gameObject.
            GetComponent<TextMeshProUGUI>().text = response.characterName;

        dialogueBox.transform.GetChild(1).gameObject.
            GetComponent<TextMeshProUGUI>().text = string.Empty;

        currentDialogue = response;

        StopAllCoroutines();
        StartCoroutine(DisplayDialogue());

        canSkipDialogue = true;
    }

    IEnumerator ShowNotification()
    {
        notification.SetActive(!notification.activeInHierarchy);
        yield return new WaitForSeconds(1f);
        notification.SetActive(!notification.activeInHierarchy);
    }

    void Update()
    {
        if (!canSkipDialogue)
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            if (TouchPhase.Ended == Input.GetTouch(0).phase)
            {
                if (dialogueFinished)
                {
                    if (dataController != null)
                        GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 2");

                    dialogueIndex++;
                    currentDialogue = dialogues[dialogueIndex];

                    if (dialogueIndex < dialogues.Count)
                    {
                        StopAllCoroutines();
                        StartCoroutine(DisplayDialogue());
                    }
                    else
                    {

                    }
                }
                else
                {
                    if (dialogueIndex < dialogues.Count)
                    {
                        if (dataController != null)
                            GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 1");

                        if (dialogueIndex == 0)
                            GameObject.FindObjectOfType<CController>().StopAllCoroutines();

                        StopAllCoroutines();
                        DisplayFullDialogue();
                    }
                    else
                    {

                    }
                }
            }
        }
    }
}
