using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour {

    [Header("Background Components")]
    public GameObject background;
    public GameObject scrollingBackground;

    [Header("Animators")]
    public Animator backgroundAnimator;
    public Animator transitionAnimator;
    public Animator scrollingBackgroundAnimator;

    [Header("Managers")]
    public DialogueManager dialogueManager;

    [Space]
    private Sprite currentBackground;
    private Sprite previousBackground;

    public IEnumerator BackgroundChangeAndTransition(Sprite backgroundSprite)
    {
        if (previousBackground == null)
        {
            background.GetComponent<Image>().sprite = backgroundSprite;
            yield return new WaitForSeconds(1f);
            transitionAnimator.SetBool("Idle", true);
        }
        else if (!(previousBackground.Equals(backgroundSprite)))
        {
            transitionAnimator.SetBool("Change", true);
            currentBackground = backgroundSprite;
        }

        previousBackground = backgroundSprite;

        yield return null;
    }

    public void ChangeBackground()
    {
        background.GetComponent<Image>().sprite = currentBackground;
    }

    public void ChangeBackgroundStop()
    {
        transitionAnimator.SetBool("Change", false);
    }

    public void FadeOutTransition()
    {
        transitionAnimator.SetBool("Idle", false);
        transitionAnimator.SetBool("Fade Out", true);
    }

    public void NameEntry()
    {
        dialogueManager.NameEntry();
    }

    public void DialogueFadeIn()
    {
        dialogueManager.DialogueFadeIn();
    }

    public void ExitDialogue()
    {
        dialogueManager.ExitDialogue();
    }
}
