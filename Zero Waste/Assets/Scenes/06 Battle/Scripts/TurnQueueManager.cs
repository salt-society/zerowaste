using System.Collections;
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
    public GameObject[] queueBars;
    
    [Header("Managers")]
    public BattleInfoManager battleInfoManager;
    public StatusManager statusManager;

    private List<Character> characterQueue;
    private List<int> deadCharacterList;
    private Character fastestCharacter;
    private Character currentCharacter;

    void Start()
    {
        deadCharacterList = new List<int>();
    }

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

    public IEnumerator DisplayTurnQueue()
    {
        turnQueue.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        int i = 0;
        int j = 0;
        foreach (Character character in characterQueue)
        {
            queueIcons[i].GetComponent<Image>().sprite = character.characterThumb;
            queueIcons[i].SetActive(true);

            if (deadCharacterList != null)
            {
                if (deadCharacterList.Count > j)
                {
                    if (deadCharacterList[j] == i)
                    {
                        queueOverlays[i].SetActive(true);
                        j++;
                    }
                    else
                    {
                        queueOverlays[i].SetActive(false);
                    }
                }
                else
                {
                    queueOverlays[i].SetActive(false);
                }
            }
            else
            {
                queueOverlays[i].SetActive(false);
            }

            queueBars[i].SetActive(true);
            i++;

            yield return null;
        }
    }

    public IEnumerator HideTurnQueue()
    {
        int i = 0;
        foreach (Character character in characterQueue)
        {
            queueIcons[i].GetComponent<Image>().sprite = character.characterThumb;
            queueIcons[i].SetActive(false);

            i++;
        }

        yield return new WaitForSeconds(0.5f);

        turnQueue.SetActive(false);
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

    public List<Character> GetCharacterQueue()
    {
        return characterQueue;
    }

    // <summary>
    // Dims character's queue icon to emphasize remaining characters
    // that will still attack
    // </summary>
    public void FinishedTurn(Character finishedCharacter)
    {
        int i = 0;
        foreach (Character character in characterQueue)
        {
            if (character.GetInstanceID().Equals(finishedCharacter.GetInstanceID()))
            {
                queueOverlays[i].SetActive(true);
                break;
            }
            i++;
        }
    }

    // <summary>
    // Crosses character out from the queue
    // </summary>
    public void CharacterDead(Character deadCharacter)
    {
        int i = 0;
        foreach (Character character in characterQueue)
        {
            if (character.GetInstanceID().Equals(deadCharacter.GetInstanceID()))
            {
                queueOverlays[i].SetActive(true);
                deadCharacterList.Add(i);
                break;
            }
            i++;
        }
    }
}
