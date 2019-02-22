using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelect : MonoBehaviour
{
    public DataController dataController;
    public ScavengerRoster scavengerRosterManager;

    [Space]
    public GameObject scavengerRoster;
    public GameObject boosterRoster;

    [Space]
    public Color defaultSlotColor;
    public Color highlightSlotColor;
    public Color selectedSlotColor;

    [Space]
    public GameObject[] questionIcons;

    [Space]
    public List<GameObject> scavengerSlots;
    
    public Player[] scavengerTeam;
    private int currentSlot;

    void Start()
    {
        dataController = GameObject.FindObjectOfType<DataController>();
        scavengerTeam = new Player[3];
    }

    public IEnumerator SetDefaultScavengers()
    {
        if (dataController != null)
        {
            // Add Ryleigh as Default Scavenger
            scavengerTeam[0] = dataController.scavengerRoster[0];
            scavengerTeam[1] = null;
            scavengerTeam[2] = null;

            scavengerSlots[0].GetComponent<Image>().sprite = dataController.scavengerRoster[0].characterFull;
            yield return new WaitForSeconds(1f);
            scavengerSlots[0].SetActive(true);
        }
    }

    public void SelectScavenger(int position)
    {
        currentSlot = position;

        scavengerSlots[position].GetComponent<Image>().color = highlightSlotColor;

        StopAllCoroutines();
        StartCoroutine(ShowScavengerRoster());

        Debug.Log("Slot clicked: " + position);
    }

    IEnumerator ShowScavengerRoster()
    {
        if (!scavengerRoster.activeInHierarchy)
        {
            if (boosterRoster.activeInHierarchy)
                boosterRoster.SetActive(false);

            yield return null;
        }

        scavengerRoster.SetActive(true);
    }

    public IEnumerator AddScavengerToTeam(Player scavenger)
    {
        bool isInTeam = false;
        int prevSlot = 0;

        foreach(Player scav in scavengerTeam) 
        {
            if (scav == scavenger)
            {
                if (prevSlot != currentSlot)
                {
                    isInTeam = true;
                    break;
                }
                else
                {
                    isInTeam = false;
                }
            }

            prevSlot++;
        }

        if (isInTeam)
        {
            if (prevSlot != currentSlot)
            {
                Debug.Log("Transfering scav to other slot.");

                scavengerTeam[prevSlot] = null;
                questionIcons[prevSlot].SetActive(true);
                StartCoroutine(RemoveScavenger(prevSlot));
                yield return new WaitForSeconds(1f);

                if (scavengerTeam[currentSlot] != null)
                {
                    Debug.Log("Removing scav on currently selected slot.");

                    scavengerTeam[currentSlot] = null;
                    questionIcons[currentSlot].SetActive(true);
                    StartCoroutine(RemoveScavenger(currentSlot));
                    yield return new WaitForSeconds(1f);
                }

                scavengerTeam[currentSlot] = scavenger;
                questionIcons[currentSlot].SetActive(false);
                scavengerSlots[currentSlot].GetComponent<Image>().color = selectedSlotColor;
                scavengerSlots[currentSlot].GetComponent<Image>().sprite = scavenger.characterFull;
                scavengerSlots[currentSlot].SetActive(true);

                yield return new WaitForSeconds(1f);

                scavengerSlots[currentSlot].GetComponent<Image>().fillAmount = 1;
            }
            else
            {
                Debug.Log("Scav is already in place.");
            }
        }
        else
        {
            Debug.Log("Adding scavenger to slot.");
            if (scavengerTeam[currentSlot] != null)
            {
                Debug.Log("Removing scav on currently selected slot.");

                scavengerTeam[currentSlot] = null;
                questionIcons[currentSlot].SetActive(true);
                StartCoroutine(RemoveScavenger(currentSlot));
                yield return new WaitForSeconds(1f);
            }

            scavengerTeam[currentSlot] = scavenger;
            questionIcons[currentSlot].SetActive(false);
            scavengerSlots[currentSlot].GetComponent<Image>().sprite = scavenger.characterFull;
            scavengerSlots[currentSlot].SetActive(true);

            yield return new WaitForSeconds(1f);

            scavengerSlots[currentSlot].GetComponent<Image>().fillAmount = 1;
        }
    }

    public IEnumerator AddScavengerToTeam(int scavengerIndex)
    {
        bool isInTeam = false;
        int prevSlot = 0;

        Player scavenger = Instantiate(dataController.scavengerRoster[scavengerIndex]);

        foreach (Player scav in scavengerTeam)
        {
            if (scav == scavenger)
            {
                if (prevSlot != currentSlot)
                {
                    isInTeam = true;
                    break;
                }
                else
                {
                    isInTeam = false;
                }
            }

            prevSlot++;
        }

        if (isInTeam)
        {
            if (prevSlot != currentSlot)
            {
                Debug.Log("Transfering scav to other slot.");
                scavengerTeam[prevSlot] = null;
                StartCoroutine(RemoveScavenger(prevSlot));
                yield return new WaitForSeconds(1f);

                if (scavengerTeam[currentSlot] != null)
                {
                    Debug.Log("Removing scav on currently selected slot.");

                    scavengerTeam[currentSlot] = null;
                    StartCoroutine(RemoveScavenger(currentSlot));
                    yield return new WaitForSeconds(1f);
                }

                scavengerTeam[currentSlot] = dataController.scavengerRoster[scavengerIndex];
                scavengerSlots[currentSlot].GetComponent<Image>().color = selectedSlotColor;
                scavengerSlots[currentSlot].GetComponent<Image>().sprite = scavenger.characterFull;
                scavengerSlots[currentSlot].SetActive(true);

                yield return new WaitForSeconds(1f);

                scavengerSlots[currentSlot].GetComponent<Image>().fillAmount = 1;
            }
            else
            {
                Debug.Log("Scav is already in place.");
            }
        }
        else
        {
            Debug.Log("Adding scavenger to slot.");
            if (scavengerTeam[currentSlot] != null)
            {
                Debug.Log("Removing scav on currently selected slot.");

                scavengerTeam[currentSlot] = null;
                StartCoroutine(RemoveScavenger(currentSlot));
                yield return new WaitForSeconds(1f);
            }

            scavengerTeam[currentSlot] = dataController.scavengerRoster[scavengerIndex];
            scavengerSlots[currentSlot].GetComponent<Image>().color = selectedSlotColor;
            scavengerSlots[currentSlot].GetComponent<Image>().sprite = scavenger.characterFull;
            scavengerSlots[currentSlot].SetActive(true);

            yield return new WaitForSeconds(1f);

            scavengerSlots[currentSlot].GetComponent<Image>().fillAmount = 1;
        }
    }

    IEnumerator RemoveScavenger(int position)
    {
        scavengerSlots[position].GetComponent<Image>().color = defaultSlotColor;
        scavengerSlots[position].GetComponent<Animator>().SetBool("Remove", true);
        yield return new WaitForSeconds(1f);
        scavengerSlots[position].GetComponent<Animator>().SetBool("Remove", false);
        scavengerSlots[position].SetActive(false);
    }
}
