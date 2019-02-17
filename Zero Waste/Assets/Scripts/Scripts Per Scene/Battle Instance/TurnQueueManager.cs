﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnQueueManager : MonoBehaviour {

    [Header("Queue Components")]
    public GameObject turnQueue;
    public GameObject pointer;

    [Space]
    public GameObject[] queueIcons;
    public GameObject[] queueOverlays;
    
    [Header("Managers")]
    public BattleInfoManager battleInfoManager;
    public StatusManager statusManager;

    private List<Character> characterQueue;
    private Character fastestCharacter;

    private Character currentCharacter;

    public void CalculateTurn(Player[] scavengers, Enemy[] mutants)
    {
        // Create lists for all characters [characterList]
        // and sorted characters by speed [characterQ]
        List<Character> characterList = new List<Character>();
        List<Character> characterQ = new List<Character>();

        // Counters
        int characterCount = 0;
        int fastestSpeed = -1;

        // Add non-null characters to list
        // Separate loop so player is prioritize if speed is the same
        for (int i = 0; i < scavengers.Length; i++)
        {
            if (scavengers[i] != null)
            {
                characterList.Add(scavengers[i]);
                characterCount++;
            }
        }

        for (int i = 0; i < mutants.Length; i++)
        {
            if (mutants[i] != null)
            {
                characterList.Add(mutants[i]);
                characterCount++;
            }
        }

        // Arrange characters by speed
        for (int i = 0; i < characterCount; i++)
        {
            foreach (Character character in characterList)
            {
                if (character.currentSpd > fastestSpeed)
                {
                    fastestSpeed = character.currentSpd;
                    fastestCharacter = character;
                }
            }

            characterList.Remove(fastestCharacter);
            characterQ.Add(fastestCharacter);
            fastestSpeed = -1;
        }

        // Store sorted characters to global list
        characterQueue = characterQ;
    }

    public IEnumerator DisplayTurnQueue(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;

        if (showComponent)
        {
            // Display turn queue box
            turnQueue.SetActive(showComponent);
            yield return new WaitForSeconds(.5f);

            // Display each queue cell one at a time
            int i = 0;
            foreach (Character character in characterQueue)
            {
                queueIcons[i].GetComponent<Image>().sprite = character.characterThumb;
                queueIcons[i].SetActive(true);
                i++;
            }
        }
        else
        {
            turnQueue.SetActive(showComponent);
            foreach (GameObject queueIcon in queueIcons)
            {
                queueIcon.SetActive(showComponent);
            }
        }
       

        yield return null;
    }

    public void ShowTurnQueue(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        turnQueue.GetComponent<Animator>().SetBool("Exit", showComponent);
    }

    public void HideTurnQueue(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        turnQueue.GetComponent<Animator>().SetBool("Exit Up", showComponent);
    }

    public void SetCurrentCharacter(int index)
    {
        currentCharacter = characterQueue[index];
    }

    public Character GetCurrentCharacter()
    {
        return currentCharacter;
    }

    public void PointCurrentCharacter(int index)
    {
        BoxCollider2D collider = queueIcons[index].GetComponent<BoxCollider2D>();
        float midpoint = collider.bounds.center.x;

        pointer.transform.position = new Vector3(midpoint, pointer.transform.position.y);
    }

    public void ShowPointer(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        pointer.SetActive(showComponent);
    }

    // 

    public List<Character> GetCharacterQueue()
    {
        return characterQueue;
    }
}