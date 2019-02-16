using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour {

    [Header("Background Components")]
    public GameObject background;

    [Header("Animators")]
    public Animator backgroundAnimator;
    public Animator transitionAnimator;

    [Header("Managers")]
    public DialogueManager dialogueManager;

    [Space]
    private Sprite currentBackground;
    private Sprite previousBackground;

    private Color currentBgColor;

    public IEnumerator BackgroundChangeAndTransition(Sprite backgroundSprite, Color backgroundColor)
    {
        if (previousBackground == null)
        {
            background.GetComponent<Image>().sprite = backgroundSprite;
            background.GetComponent<Image>().color = backgroundColor;

            yield return new WaitForSeconds(1f);

            transitionAnimator.SetBool("Idle", true);
        }
        else if (!(previousBackground.Equals(backgroundSprite)))
        {
            transitionAnimator.SetBool("Change", true);
            currentBackground = backgroundSprite;
            currentBgColor = backgroundColor;
        }

        previousBackground = backgroundSprite;
    }

    public void ChangeBackground()
    {
        background.GetComponent<Image>().sprite = currentBackground;
        background.GetComponent<Image>().color = currentBgColor;
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
