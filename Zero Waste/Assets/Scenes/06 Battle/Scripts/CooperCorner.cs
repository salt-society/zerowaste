using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CooperCorner : MonoBehaviour
{
    [Header("Sequence")]
    public List<string> tutorialSequence;

    [Header("Talks")]
    public GameObject cooperIcon;
    public TextMeshProUGUI talkSection;
    public string[] talks;
    public int currentTalkIndex;
    

    [Header("Sections")]
    public SpriteMask[] sectionMasks;
    public TextMeshProUGUI[] sectionNames;
    public TextMeshProUGUI[] sectionDefs;
    public int currentSectionIndex;

    [Header("Interact")]
    public GameObject[] objectToInteract;

    [Space]
    public GameObject arrowObject;
    public Vector2[] arrowXY;
    public Quaternion[] arrowRotation;

    [Space]
    public string[] instructions;
    public string[] descriptions;
    public TextMeshProUGUI instructionObject;
    public Vector2[] instructionXY;
    public Vector2[] descriptionXY;
    public int currentInteractIndex;

    [Space]
    public GameObject tutorialSection;

    [Space]
    public bool talkFinished;
    public bool sectionFinished;

    [Space]
    public bool interactiveFinished;
    public bool disableRaycastBlock;
    public bool instructionShown;

    public int currentTutorialIndex;
    public bool proceed;
    public bool tutorialPlaying;
    
    public void PlayTutorial(int tutorialIndex)
    {
        currentTutorialIndex = tutorialIndex;
        proceed = false;

        if (tutorialSequence[tutorialIndex].Equals("Cooper Talk"))
        {
            StartCoroutine(TypeTalk());
            tutorialPlaying = true;
        }

        if (tutorialSequence[tutorialIndex].Equals("Section"))
        {
            StartCoroutine(ShowSectionTutorial());
            tutorialPlaying = true;
        }

        if (tutorialSequence[tutorialIndex].Equals("Interact"))
        {
            StartCoroutine(InteractTutorial());
            tutorialPlaying = true;
        }
    }

    void Update()
    {
        if (!tutorialPlaying)
            return;

        if (Input.touchCount > 0)
        {
            if (TouchPhase.Ended == Input.GetTouch(0).phase)
            {
                Vector3 touchPos = Input.GetTouch(0).position;

                if (currentTutorialIndex <= tutorialSequence.Count - 1)
                {
                    if (tutorialSequence[currentTutorialIndex].Equals("Cooper Talk"))
                    {
                        if (talkFinished)
                        {
                            FindObjectOfType<AudioManager>().PlaySound("Click 01");

                            proceed = true;
                            tutorialPlaying = false;
                            currentTalkIndex++;

                            StopAllCoroutines();
                        }
                        else
                        {
                            FindObjectOfType<AudioManager>().PlaySound("Beep Denied");
                        }
                    }

                    if (tutorialSequence[currentTutorialIndex].Equals("Section"))
                    {
                        if (sectionFinished)
                        {
                            FindObjectOfType<AudioManager>().PlaySound("Click 01");

                            proceed = true;
                            tutorialPlaying = false;
                            currentSectionIndex++;

                            StopAllCoroutines();
                        }
                        else
                        {
                            FindObjectOfType<AudioManager>().PlaySound("Beep Denied");
                        }
                    }

                    if (tutorialSequence[currentTutorialIndex].Equals("Interact"))
                    {
                        if (!interactiveFinished)
                        {
                            if (instructionShown)
                            {
                                FindObjectOfType<AudioManager>().PlaySound("Click 01");

                                disableRaycastBlock = true;
                                tutorialPlaying = false;
                            }
                            else
                            {
                                FindObjectOfType<AudioManager>().PlaySound("Beep Denied");
                            }
                        }
                    }
                }
                else
                {
                    HideTalk();
                    HideSectionTutorial();
                    
                    tutorialPlaying = false;
                }
            }
        }
    }

    public bool CanProceed()
    {
        return proceed;
    }

    public IEnumerator TypeTalk()
    {
        HideSectionTutorial();

        cooperIcon.SetActive(true);
        talkSection.gameObject.SetActive(true);

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
                yield return new WaitForSeconds(0.0001f);
                letterCount++;

                if (lastIndex == letterCount)
                {
                    talkFinished = true;
                }
            }
        }
    }

    public void HideTalk()
    {
        cooperIcon.SetActive(false);
        talkSection.gameObject.SetActive(false);
    }

    public IEnumerator ShowSectionTutorial()
    {
        HideTalk();
        HideSectionTutorial();

        sectionFinished = false;

        sectionNames[currentSectionIndex].gameObject.SetActive(true);
        sectionMasks[currentSectionIndex].gameObject.SetActive(true);
        sectionDefs[currentSectionIndex].gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        sectionFinished = true;
    }

    public void HideSectionTutorial()
    {
        if (currentSectionIndex >= 1)
        {
            sectionNames[currentSectionIndex - 1].gameObject.SetActive(false);
            sectionMasks[currentSectionIndex - 1].gameObject.SetActive(false);
            sectionDefs[currentSectionIndex - 1].gameObject.SetActive(false);
        }
    }

    public IEnumerator InteractTutorial()
    {
        HideTalk();
        HideSectionTutorial();

        interactiveFinished = false;

        arrowObject.transform.localPosition = arrowXY[currentInteractIndex];
        arrowObject.transform.localRotation = arrowRotation[currentInteractIndex];

        instructionObject.transform.localPosition = instructionXY[currentInteractIndex];
        instructionObject.text = string.Empty;
        instructionObject.text = instructions[currentInteractIndex];

        arrowObject.SetActive(true);
        instructionObject.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        instructionShown = true;

        yield return new WaitUntil(isRaycastBlockEnabled);
        gameObject.GetComponent<Image>().raycastTarget = false;

        yield return new WaitUntil(isGameObjectActive);

        arrowObject.SetActive(false);
        instructionObject.gameObject.SetActive(false);

        instructionObject.transform.localPosition = descriptionXY[currentInteractIndex];
        instructionObject.text = string.Empty;
        instructionObject.text = descriptions[currentInteractIndex];

        yield return new WaitForSeconds(0.5f);
        tutorialSection.SetActive(true);
        instructionObject.gameObject.SetActive(true);

        yield return new WaitWhile(isGameObjectActive);
        tutorialSection.SetActive(false);
        instructionObject.gameObject.SetActive(false);
        gameObject.GetComponent<Image>().raycastTarget = true;

        interactiveFinished = true;

        currentInteractIndex++;
        proceed = true;
        tutorialPlaying = false;

        FindObjectOfType<AudioManager>().PlaySound("Click 01");

        StopAllCoroutines();
    }

    public bool isGameObjectActive()
    {
        return objectToInteract[currentInteractIndex].activeInHierarchy;
    }

    public bool isRaycastBlockEnabled()
    {
        return disableRaycastBlock;
    }
}
