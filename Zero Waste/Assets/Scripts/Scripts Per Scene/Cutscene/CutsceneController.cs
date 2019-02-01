using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CutsceneController : MonoBehaviour {

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
    public int characterLimit;
    public float textSpeed;
    public float nextLineSpeed;

    [Space]
    [Header("Narration")]
    public TextMeshProUGUI narrationText;
    public GameObject narrationBox;

    

    [Space]
    public GameObject dialogueIndicator;
    public GameObject background;

    private int cutsceneNo;
    private Cutscene currentCutscene;
    private Dialogue[] dialogues;
    private int currentDialogueNo = 0;
    private List<string> parsedLines = new List<string>();
    private Coroutine dialogueTypeFx;
    

    void Start()
    {
        GetCutsceneNumber();
        PrepareCutscene();
        InitialDialogue();
    }

    void GetCutsceneNumber()
    {
        // Get cutscene number from persistent data
        // for now, temporary
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
    }

    void InitialDialogue()
    {
        currentDialogueNo = 0;
        PlayCutscene(currentDialogueNo);
    }

    void PlayCutscene(int dialogueNo)
    {
        ClearDialogue();
        StopAllCoroutines();
        dialogueTypeFx = StartCoroutine(DisplayDialogue(dialogues[dialogueNo]));
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
        StartCoroutine(backgroundManager.
            BackgroundChangeAndTransition(dialogue.background));

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

        string content = dialogue.content;

        if (dialogue.isNarration)
        {
            parsedLines.Add(content);
        }
        else
        {
            if (content.Length > characterLimit)
                ParseContent(content);
            else
                parsedLines.Add(content);
        }
        
            

        string lastLine = parsedLines[parsedLines.Count - 1];

        foreach (string line in parsedLines)
        {
            char[] charArray = line.ToCharArray();

            foreach (char character in charArray)
            {
                textbox.text += character;
                yield return new WaitForSeconds(textSpeed);
            }

            if (!(lastLine.Equals(line)))
            {
                textbox.SetText("");
                yield return new WaitForSeconds(nextLineSpeed);
            }
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

    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (currentDialogueNo < dialogues.Length - 1)
                {
                    currentDialogueNo++;
                    PlayCutscene(currentDialogueNo);
                }
            }
        }

        
    }
}
