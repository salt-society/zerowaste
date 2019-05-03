using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public DataController dataController;
    public AudioManager audioManager;
    public BackgroundManager backgroundManager;
    public HistoryGrid historyGrid;

    [Space]
    public GameObject dialogueBox;
    public GameObject narrationBox;
    public GameObject itemBox;
    public GameObject choiceBox;
    public GameObject notification;

    [Space]
    public bool canSkipDialogue;
    public bool historyOn;

    private Cutscene cutscene;
    private List<Dialogue> dialogues;
    private Dialogue currentDialogue;
    private int dialogueIndex;

    private bool dialogueFinished;

    void Start()
    {
        dataController = GameObject.FindObjectOfType<DataController>();
        audioManager = GameObject.FindObjectOfType<AudioManager>();
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
        if (audioManager != null)
        {
            PlayBGM();
        }

        // State of dialogue as of the moment
        dialogueFinished = false;
        canSkipDialogue = false;

        if (!backgroundManager.CompareBackground(currentDialogue.background))
        {
            dialogueBox.SetActive(false);
            narrationBox.SetActive(false);
        }

        // Check type if narration or dialogue
        TextMeshProUGUI textMesh;
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
                narrationBox.transform.GetChild(0).gameObject.
                   GetComponent<TextMeshProUGUI>().textInfo.Clear();
            }
            else
            {
                narrationBox.transform.GetChild(0).gameObject.
                    GetComponent<TextMeshProUGUI>().text = string.Empty;
                narrationBox.transform.GetChild(0).gameObject.
                    GetComponent<TextMeshProUGUI>().textInfo.Clear();
            }

            textMesh = narrationBox.transform.GetChild(0).gameObject.
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
                dialogueBox.transform.GetChild(1).gameObject.
                    GetComponent<TextMeshProUGUI>().textInfo.Clear();
            }
            else
            {
                dialogueBox.transform.GetChild(1).gameObject.
                    GetComponent<TextMeshProUGUI>().text = string.Empty;
                dialogueBox.transform.GetChild(1).gameObject.
                    GetComponent<TextMeshProUGUI>().textInfo.Clear();
            }

            if (currentDialogue.characterHalf == null)
            {
                if (currentDialogue.characterName.Equals("Ryleigh"))
                {
                    if (dataController != null)
                    {
                        dialogueBox.transform.GetChild(3).GetChild(0).GetChild(1).gameObject.
                                GetComponent<Image>().sprite = dataController.scavengerRoster[0].characterHalf;

                        yield return null;

                        dialogueBox.transform.GetChild(3).GetChild(0).GetChild(1).gameObject.SetActive(true);
                        dialogueBox.transform.GetChild(3).GetChild(0).GetChild(0).gameObject.SetActive(false);
                    }
                }
                else if (currentDialogue.characterName.Equals("Paige"))
                {
                    if (dataController != null)
                    {
                        dialogueBox.transform.GetChild(3).GetChild(0).GetChild(1).gameObject.
                                GetComponent<Image>().sprite = dataController.scavengerRoster[1].characterHalf;

                        yield return null;

                        dialogueBox.transform.GetChild(3).GetChild(0).GetChild(1).gameObject.SetActive(true);
                        dialogueBox.transform.GetChild(3).GetChild(0).GetChild(0).gameObject.SetActive(false);
                    }
                }
                else
                {
                    dialogueBox.transform.GetChild(3).GetChild(0).GetChild(1).gameObject.SetActive(false);
                    dialogueBox.transform.GetChild(3).GetChild(0).GetChild(0).gameObject.SetActive(true);
                }
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

            textMesh = dialogueBox.transform.GetChild(1).gameObject.
                GetComponent<TextMeshProUGUI>();
        }

        // Change scene background
        StartCoroutine(backgroundManager.ChangeBackground(currentDialogue.background,
            currentDialogue.backgroundColor));

        yield return new WaitForSeconds(1.5f);
        textMesh.richText = true;
        textMesh.textInfo.Clear();

        textMesh.gameObject.SetActive(true);
        canSkipDialogue = true;

        if (currentDialogue.isNarration)
            narrationBox.SetActive(true);
        else
            dialogueBox.SetActive(true);

        char[] characterArray = currentDialogue.content.ToCharArray();
        if (characterArray.Length > 0)
        {
            int lastIndex = characterArray.Length - 1;
            int letterCount = 0;

            StartCoroutine(PlaySFX(currentDialogue.startSFX));
            
            foreach (char letter in characterArray)
            {
                textMesh.text += letter;
                yield return new WaitForSeconds(0.02f);
                letterCount++;

                if (lastIndex == letterCount)
                {
                    historyGrid.AddCell(currentDialogue, null, false);

                    if (currentDialogue.withChoices)
                    {
                        canSkipDialogue = false;
                        ShowChoices();
                    }

                    if (currentDialogue.withItem)
                    {
                        canSkipDialogue = false;
                        StartCoroutine(ShowItem());
                    }

                    if (currentDialogue.withUnfamiliarWord)
                    {
                        canSkipDialogue = false;
                        dialogueFinished = false;
                    }
                    else
                    {
                        dialogueFinished = true;
                    }

                    yield return null;
                    StartCoroutine(PlaySFX(currentDialogue.endSFX));
                }
            } 

            if (currentDialogue.withUnfamiliarWord)
            {
                CheckForUnfamiliarWords(textMesh);

                yield return new WaitForSeconds(2f);

                canSkipDialogue = true;
                dialogueFinished = false;
            }
        }
        else if (characterArray.Length == 0)
        {
            if (currentDialogue.withItem)
            {
                canSkipDialogue = false;
                StartCoroutine(ShowItem());
            }
        }
    }

    void PlayBGM()
    {
        if (currentDialogue.dialogueId == 0)
        {
            foreach (string bgm in currentDialogue.BGM)
            {
                audioManager.PlaySound(bgm);
            }
        }
        else
        {
            foreach (string bgm in currentDialogue.BGM)
            {
                if (audioManager.IsSoundPlaying(bgm))
                {
                    StartCoroutine(audioManager.StopSound(bgm, 2f));
                }
                else
                {
                    audioManager.PlaySound(bgm);
                }
            }
        }
    }

    IEnumerator PlaySFX(List<string> SFXs)
    {
        foreach (string sfx in SFXs)
        {
            if (currentDialogue.startSimultaneousSFX)
            {
                audioManager.PlaySound(sfx);
            }
            else
            {
                audioManager.PlaySound(sfx);
                yield return new WaitForSeconds(audioManager.SoundLength(sfx));
            }
        }
    }

    void CheckForUnfamiliarWords(TextMeshProUGUI textMesh)
    {
        if (textMesh.textInfo.wordCount > 0)
        {
            TMP_WordInfo[] wordInfos = textMesh.textInfo.wordInfo;

            int i = 0;
            foreach (TMP_WordInfo wordInfo in wordInfos)
            {
                foreach (string word in currentDialogue.words)
                {
                    if (word.Equals(wordInfo.GetWord()))
                    {
                        for (int j = 0; j < wordInfo.characterCount; j++)
                        {
                            int characterIndex = wordInfo.firstCharacterIndex + j;
                            int meshIndex = textMesh.textInfo.
                                characterInfo[characterIndex].materialReferenceIndex;
                            int vertexIndex = textMesh.textInfo.
                                characterInfo[characterIndex].vertexIndex;

                            Color32[] vertexColors = textMesh.textInfo.
                                meshInfo[meshIndex].colors32;
                            vertexColors[vertexIndex + 0] = currentDialogue.wordColors[i];
                            vertexColors[vertexIndex + 1] = currentDialogue.wordColors[i];
                            vertexColors[vertexIndex + 2] = currentDialogue.wordColors[i];
                            vertexColors[vertexIndex + 3] = currentDialogue.wordColors[i];
                        }

                        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
                        i++;
                    }
                }

                if (i == currentDialogue.words.Count)
                    break;
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

    IEnumerator ShowItem()
    {
        itemBox.transform.GetChild(1).gameObject.GetComponent<Image>().
            sprite = currentDialogue.itemSprite;

        yield return null;

        itemBox.SetActive(true);
        yield return new WaitForSeconds(2f);
        itemBox.SetActive(false);

        canSkipDialogue = true;
        dialogueFinished = true;
    }

    IEnumerator ShowNotification()
    {
        notification.SetActive(!notification.activeInHierarchy);
        yield return new WaitForSeconds(1f);
        notification.SetActive(!notification.activeInHierarchy);
    }

    void Update()
    {
        if (dataController == null)
            return;

        if (!canSkipDialogue)
            return;

        if (historyOn)
            return;

        if (Input.touchCount > 0)
        {
            if (TouchPhase.Ended == Input.GetTouch(0).phase)
            {
                if (dialogueIndex < dialogues.Count - 1)
                {
                    if (dialogueFinished)
                    {
                        GameObject.FindObjectOfType<AudioManager>().PlaySound("Click 01");

                        dialogueIndex++;
                        currentDialogue = dialogues[dialogueIndex];

                        StopAllCoroutines();
                        StartCoroutine(DisplayDialogue());
                    }
                    else
                    {
                        GameObject.FindObjectOfType<AudioManager>().PlaySound("Click 01");
                        GameObject.FindObjectOfType<CustceneController>().StopAllCoroutines();
                        StopAllCoroutines();

                        DisplayFullDialogue();
                    }
                }
                else
                {
                    if (!dialogueFinished)
                    {
                        GameObject.FindObjectOfType<AudioManager>().PlaySound("Click 01");

                        StopAllCoroutines();
                        DisplayFullDialogue();
                    }
                    else
                    {
                        canSkipDialogue = false;

                        GameObject.FindObjectOfType<AudioManager>().PlaySound("Crumpling Paper");
                        GameObject.FindObjectOfType<CustceneController>().CutsceneFinished();
                    }
                }
            }
        }
    }
}
