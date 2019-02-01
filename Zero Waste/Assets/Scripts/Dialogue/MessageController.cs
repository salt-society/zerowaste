using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageController: MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI messageHolder;
    [SerializeField]
    private Message messages;
    [Space]
    [SerializeField]
    private float speed;
    [SerializeField]
    private float delay;

    private int currentChar, totalChar;
    private bool started;
    private bool finished;

    void Start()
    {
        currentChar = totalChar = 0;
        finished = false;
        started = false;
    }

    public void DisplayMessage(int messageIndex)
    {
        // if queue is not empty, display message
        string message = messages.messageArray[messageIndex];
        totalChar = message.ToCharArray().Length;

        // stop first all active coroutines
        StopAllCoroutines();
        StartCoroutine(TypeMessage(message)); // start displaying message
    }

    IEnumerator TypeMessage(string message)
    {
        // delay before starting animation
        yield return new WaitForSeconds(delay);

        // clear messageHolder contents and display message
        messageHolder.text = "";
        foreach (char character in message.ToCharArray())
        {
            messageHolder.text += character;
            yield return new WaitForSeconds(speed); // speed

            currentChar++;
        }

        if (currentChar.Equals(totalChar))
            MessageEnded();
    }

    void MessageEnded()
    {
        finished = true;
        Debug.Log("Finished displaying message!");
    }


    public bool getFinishedValue() {
        return finished;
    }

    public void setFinishedValue(bool finished)
    {
        this.finished = finished;
    }

    public bool getStartedValue()
    {
        return started;
    }

    public void setStartedValue(bool started)
    {
        this.started = started;
    }
}
