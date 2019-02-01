using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour {

    private Queue<string> dialogue;

	void Start () {
		// initialize queue
        dialogue = new Queue<string>();
	}

    public void DisplayNextLine()
    {
        if (dialogue.Count.Equals(0))
        {
            return;
        }
    }
}
