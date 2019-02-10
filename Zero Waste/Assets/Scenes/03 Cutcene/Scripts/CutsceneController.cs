using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CutsceneController : MonoBehaviour {

    [Header("Data Controller")]
    public DataController dataController;

    [Header("Cutscenes Data")]
    public Cutscene[] cutscenes;

    [Header("Managers")]
    public BackgroundManager backgroundManager;
    public DialogueManager dialogueManager;

    [Header("Components")]
    [Header("Dialogue")]
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueBox;
    public GameObject dialogueIndicator;

    [Space]
    public int characterLimit;
    public float textSpeed;
    public float nextLineSpeed;

    [Space]
    [Header("Choice")]
    public GameObject choiceBox;
    public Button[] choiceButtons;
    public TextMeshProUGUI[] choices;

    [Space]
    [Header("Narration")]
    public TextMeshProUGUI narrationText;
    public GameObject narrationBox;

    private int cutsceneNo;
    private int currentDialogueNo;
    private int skipCount;
    private int currentLine;

    private Cutscene currentCutscene;
    private Dialogue[] dialogues;
    private List<string> parsedLines;

    private bool skipDialogue;
    private bool choiceSelected;
    private Choice currentChoice;
    private Effect[] effects;

    void Start()
    {
        GetCutsceneNumber();
        PrepareCutscene();
        InitialDialogue();
    }

    void GetCutsceneNumber()
    {
        // Get reference to Data Controller
        //dataController = GameObject.FindObjectOfType<DataController>();
        //cutsceneNo = dataController.currentSaveData.currentCutscene;
        cutsceneNo = 0;
    }

    void PrepareCutscene()
    {
        // Get cutscene data
        foreach (Cutscene cutscene in cutscenes) 
        {
            if (cutscene.cutsceneNo == cutsceneNo)
            {
                currentCutscene = cutscene;
                dialogues = currentCutscene.dialogues.ToArray();
            }
        }

        // Set background
        StartCoroutine(backgroundManager.
            BackgroundChangeAndTransition(currentCutscene.firstBackground));

        skipCount = 0;
        choiceSelected = false;
        skipDialogue = true;
    }

    void InitialDialogue()
    {
        parsedLines = new List<string>();
        currentDialogueNo = 0;

        PlayCutscene(currentDialogueNo);
    }

    void PlayCutscene(int dialogueNo)
    {
        ClearDialogue();
        StopAllCoroutines();
        StartCoroutine(DisplayDialogue(dialogues[dialogueNo]));
    }

    void ClearDialogue()
    {
        narrationText.text = string.Empty;
        dialogueText.text = string.Empty;
        characterName.text = string.Empty;
        parsedLines.Clear();
    }

    private IEnumerator DisplayDialogue(Dialogue dialogue)
    {
        TextMeshProUGUI textbox;

        // Change background
        StartCoroutine(backgroundManager.
            BackgroundChangeAndTransition(dialogue.background));


        // Check if dialogue is normal dialogue or a narration
        // Narration
        if (dialogue.isNarration)
        {
            dialogueBox.SetActive(false);
            narrationBox.SetActive(true);
            textbox = narrationText;
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            narrationBox.SetActive(false);
            dialogueBox.SetActive(true);
            dialogueManager.DialogueFadeIn();
            textbox = dialogueText;

            // Apply transition
            if (dialogue.transition.Equals("Start") || dialogue.transition.Equals("Change"))
            {
                yield return new WaitForSeconds(1f);
                characterName.text = dialogue.characterName;
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                characterName.text = dialogue.characterName;
            }
        }

        // Limit characters to avoid overflow
        string content = dialogue.content;
        if (dialogue.isNarration) { parsedLines.Add(content); } // No need for narration
        else
        {
            // Break dialogue into chunks if over character limit
            if (content.Length > characterLimit)
                ParseContent(content);
            else
                parsedLines.Add(content);
        }
        
        // Get last line
        string lastLine = parsedLines[parsedLines.Count - 1];

        // Apply typing effect on dialogue by looping through all
        // the lines that have been separated by character limit
        foreach (string line in parsedLines)
        {
            char[] charArray = line.ToCharArray();

            // Create typing effect by displaying one character at a time
            foreach (char character in charArray)
            {
                textbox.text += character;
                yield return new WaitForSeconds(textSpeed);
            }

            // When typing effect is finished, check if current line is 
            // last line and if not, clear text box for next line
            if (!(lastLine.Equals(line)))
            {
                textbox.SetText("");
                currentLine++;
                yield return new WaitForSeconds(nextLineSpeed);
            }
            // When last line is reached, check if there are choices
            else
            {
                if (dialogue.withChoices)
                {
                    ShowChoiceBox(dialogue);
                    choiceSelected = true;
                    skipDialogue = false;
                }
            }
        }
    }

    private void DisplayFullLine(Dialogue dialogue, bool withChoice)
    {
        string content = dialogue.content;
        characterName.text = dialogue.characterName;

        if (dialogue.isNarration)
            narrationText.text = content;
        if (!dialogue.isNarration)
            dialogueText.text = content;

        if(withChoice)
            ShowChoiceBox(dialogue);
    }

    IEnumerator DisplayMultipleFullLine()
    {
        for (int i = currentLine; i < parsedLines.Count; i++)
        {
            dialogueText.text = parsedLines[i];
            yield return new WaitForSeconds(3f);
        }
    }

    private void ParseContent(string content)
    {
        string[] words = content.Split(' ');

        string substring = "";
        int characterCount = 0;
        for (int i = 0; i < words.Length; i++)
        {
            if (characterCount <= characterLimit)
            {
                substring += words[i] + " ";
                characterCount = substring.Length + 1;

                if (i == words.Length - 1)
                {
                    substring = substring.Trim();
                    parsedLines.Add(substring);
                    return;
                }
            }
            else
            {
                substring = substring.TrimEnd();
                int lastIndex = substring.LastIndexOf(' ');
                int currentWordLength = words[i].Length;
                int lastWordIndex = substring.Length - (currentWordLength);

                substring = substring.Remove(lastIndex);
                parsedLines.Add(substring);

                content = content.Remove(0, substring.Length);
                content = content.Trim();
                ParseContent(content);

                break;
            }
        }
    }

    private void ShowChoiceBox(Dialogue dialogue)
    {
        int i = 0;
        foreach (TextMeshProUGUI choice in choices)
        {
            choice.text = dialogue.choices[i].choice;
            i++;
        }
            
        choiceBox.SetActive(true);
    }

    public void SelectChoice(int choiceNo)
    {
        Choice[] choices = dialogues[currentDialogueNo].choices;
        choiceButtons[choiceNo].GetComponent<Image>().color = choices[choiceNo].color;

        effects = choices[choiceNo].effects;
        currentChoice = choices[choiceNo];

        StartCoroutine(HideChoiceBox(choices[choiceNo]));
    }

    IEnumerator HideChoiceBox(Choice choice)
    {
        yield return new WaitForSeconds(0.8f);
        choiceBox.GetComponent<Animator>().SetBool("Exit", true);
        yield return new WaitForSeconds(0.4f);
        choiceBox.GetComponent<Animator>().SetBool("Exit", false);
        choiceBox.SetActive(false);

        ShowResponse(choice);
    }

    public void ShowResponse(Choice choice)
    {
        ClearDialogue();
        StopAllCoroutines();
        StartCoroutine(DisplayDialogue(choice.response));

        skipDialogue = true;
    }

    void Update()
    {
        if (!skipDialogue)
            return;

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                skipCount++;
                // Skip to next dialogue if current doesn't have options
                if (!dialogues[currentDialogueNo].withChoices)
                {
                    if (skipCount == 1)
                    {
                        if (parsedLines.Count > 1)
                        {
                            StopAllCoroutines();
                            dialogueText.text = parsedLines[currentLine];
                            currentLine++;

                            if (currentLine < parsedLines.Count)
                                skipCount = 0;
                            else
                                skipCount = 1;
                        }
                        else
                        {
                            StopAllCoroutines();
                            ClearDialogue();
                            DisplayFullLine(dialogues[currentDialogueNo], false);
                        }
                    }
                    else
                    {
                        skipCount = 0;

                        if (currentDialogueNo < dialogues.Length - 1)
                        {
                            currentDialogueNo++;
                            PlayCutscene(currentDialogueNo);
                        }
                        else
                        {

                        }
                    }
                    
                }
                // Instead of skipping to next option, skip dialogue
                // and display option
                else
                {
                    if (!choiceSelected)
                    {
                        choiceSelected = true;
                        skipDialogue = false;

                        StopAllCoroutines();
                        ClearDialogue();
                        DisplayFullLine(dialogues[currentDialogueNo], true);

                        skipCount = 0;
                    }
                    else
                    {
                        if (skipCount == 1)
                        {
                            if (parsedLines.Count > 1)
                            {
                                StopAllCoroutines();
                                dialogueText.text = parsedLines[currentLine];
                                currentLine++;

                                if (currentLine < parsedLines.Count)
                                    skipCount = 0;
                                else
                                    skipCount = 1;
                            }
                            else
                            {
                                StopAllCoroutines();
                                ClearDialogue();
                                DisplayFullLine(currentChoice.response, false);

                                skipDialogue = true;
                            }
                        }
                        else
                        {
                            skipCount = 0;
                            if (currentDialogueNo < dialogues.Length - 1)
                            {
                                currentDialogueNo++;
                                PlayCutscene(currentDialogueNo);
                            }
                            else
                            {

                            }
                        }
                        
                    }
                }
            }
        }
    }

    IEnumerator LoadScene()
    {
        yield return null;
    }
}
