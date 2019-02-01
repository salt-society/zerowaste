using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnQueueManager : MonoBehaviour {

    [Header("Queue UI")]
    public GameObject[] queueIcons;
    public GameObject[] queueOverlays;
    public GameObject turnQueue;

    [Header("Managers")]
    public BattleInfoManager battleInfoManager;
    public StatManager statManager;


    // Globals
    private Character[] characterQueue;
    private Character fastestCharacter;
    private Player[] playerOrder;

    public void CalculateTurn(Player[] players, Enemy[] enemies)
    {
        for (int i = 0; i < players.Length; i++)
            players[i].OnInitialize();

        for (int i = 0; i < enemies.Length; i++)
            enemies[i].OnInitialize();

        // Create lists for all characters [characterList]
        // and sorted characters by speed [characterQ]
        List<Character> characterList = new List<Character>();
        List<Character> characterQ = new List<Character>();

        // Counters
        int characterCount = 0;
        int fastestSpeed = -1;

        // Add non-null characters to list
        // Separate loop so player is prioritize if speed is the same
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                characterList.Add(players[i]);
                characterCount++;
            }
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                characterList.Add(enemies[i]);
                characterCount++;
            }
        }

        // Arrange characters by speed
        playerOrder = new Player[players.Length];

        for (int i = 0; i < characterCount; i++)
        {
            foreach (Character character in characterList)
            {
                Debug.Log(character.currentSpd + " vs " + fastestSpeed);
                if (character.currentSpd > fastestSpeed)
                {
                    fastestSpeed = character.currentSpd;
                    fastestCharacter = character;

                    // Check if character is player then add
                    if (character is Player)
                    {
                        
                    }
                }

                
            }
            

            characterList.Remove(fastestCharacter);
            characterQ.Add(fastestCharacter);
            fastestSpeed = -1;
        }

        // Store sorted characters to global list
        characterQueue = characterQ.ToArray();
    }

    public IEnumerator DisplayTurnQueue()
    {
        // Display turn queue box
        turnQueue.SetActive(true);
        yield return new WaitForSeconds(.5f);

        // Display each queue cell one at a time
        int i = 0;
        foreach(Character character in characterQueue) 
        {
            Debug.Log(i);
            queueIcons[i].GetComponent<Image>().sprite = character.characterPortrait;
            queueIcons[i].SetActive(true);
            yield return new WaitForSeconds(.5f);
            i++;
        }

        yield return null;
    }
}
