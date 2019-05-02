using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CooperCorner : MonoBehaviour
{
    [Header("Sequence")]
    public string[] tutorialSequence;

    [Header("Talks")]
    public string[] talks;
    public bool[] talkContCoroutine;
    public int currentTalkIndex;
    public TextMeshProUGUI talkSection;

    [Header("Sections")]
    public SpriteMask[] sectionMasks;
    public TextMeshProUGUI[] sectionNames;
    public TextMeshProUGUI[] sectionDefs;
    public bool[] sectionContCoroutine;
    public int currentSectionIndex;

    [Header("Interact")]
    public GameObject arrow;
    public Vector2 arrowXY;
    public TextMeshProUGUI instruction;
    public Vector2 instructionXY;
    public bool[] interactContCoroutine;
    public int currentInteractIndex;

    public bool talkFinished;
    public bool sectionFinished;
    public bool interactiveFinsihed;
    public bool proceed;

    public IEnumerator InitCooper()
    {
        foreach (string tutorial in tutorialSequence)
        {
            if (tutorial.Equals("Talk"))
            {
                StartCoroutine(TypeTalk());
                yield return new WaitUntil(NextTutorial);
                Debug.Log("Hello");
            }

            yield return null;
        }
    }

    public void PlayTutorial(int tutorialIndex)
    {

    }

    void Update()
    {
        if (Input.touchCount > 0)
        {

        }
    }

    public bool NextTutorial()
    {
        return proceed;
    }

    public IEnumerator TypeTalk()
    {
        talkSection.text = string.Empty;
        talkFinished = false;

        char[] charArray = talks[currentTalkIndex].ToCharArray();
        if (charArray.Length > 0)
        {
            int lastIndex = charArray.Length - 1;
            int letterCount = 0;

            foreach (char letter in charArray)
            {
                talkSection.text += letter;
                yield return null;
                letterCount++;

                if (lastIndex == letterCount)
                {
                    talkFinished = true;
                    currentTalkIndex++;
                }
            }
        }
    }

    public bool TalkFinished()
    {
        return talkFinished;
    }

    public void TypeInstruction()
    {

    }

    public void TypeDefinition()
    {

    }
    

}
