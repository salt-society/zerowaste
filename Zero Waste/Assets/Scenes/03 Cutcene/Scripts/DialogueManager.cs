using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

    [Header("Animators")]
    public Animator dialogueTextAnimator;
    public Animator nameAnimator;

    public void NameEntry()
    {
        nameAnimator.SetBool("Fade In Down", true);
    }

    public void NameIdle()
    {
        nameAnimator.SetBool("Idle", true);
    }

    public void NameExit()
    {
        nameAnimator.SetBool("Idle", false);
    }

    public void DialogueFadeIn() 
    {
        dialogueTextAnimator.SetBool("Fade In", true);
    }

    public void DialogueFadeOut() 
    {
        dialogueTextAnimator.SetBool("Fade Out", true);
    }

    public void DialogueFadeOutStop() 
    {
        dialogueTextAnimator.SetBool("Fade Out", false);
    }

    public void DialogueIdle() 
    {
        dialogueTextAnimator.SetBool("Fade In", false);
    }

    public void ExitDialogue() 
    {
        dialogueTextAnimator.SetBool("Exit", true);
    }
}
