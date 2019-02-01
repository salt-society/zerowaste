using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour {

    // hidden values
    [SerializeField]
    private Animator animator;
    [HideInInspector]
    public bool finished = false;
    [HideInInspector]
    public bool playing = false;

    // public values
    public string animationName;

    // called by animation events 
    // at end of each animation
    public void animationPlaying()
    {
        playing = true;
        Debug.Log(animationName + " animation playing.");
    }

    public void animationFinished()
    {
        finished = true;
        playing = false;
        Debug.Log(animationName + " animation done.");
    }
}
