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
    public GameObject[] scavengerPlatforms;
    public Color defaultSlotColor;
    public Color highlightSlotColor;
    public Color selectedSlotColor;

    [Space]
    public GameObject[] questionIcons;

    [Space]
    public List<GameObject> scavengerSlots;
    
    private Player[] scavengerTeam;
    private Enemy[] wasteTeam;

    [Space]
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
            scavengerTeam[0] = dataController.scavengerRoster[0];
            scavengerTeam[1] = null;
            scavengerTeam[2] = null;

            scavengerSlots[0].GetComponent<Image>().sprite = dataController.scavengerRoster[0].characterFull;
            yield return new WaitForSeconds(1f);
            scavengerSlots[0].SetActive(true);

            wasteTeam = dataController.currentBattle.wastePool.SelectWatesFromPool();
        }
    }

    public void SelectScavenger(int position)
    {
        currentSlot = position;

        scavengerPlatforms[position].GetComponent<Image>().color = highlightSlotColor;

        StopAllCoroutines();
        StartCoroutine(ShowScavengerRoster());
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

                questionIcons[prevSlot].SetActive(true);
                StartCoroutine(RemoveScavenger(prevSlot));
                yield return new WaitForSeconds(1f);

                if (scavengerTeam[currentSlot] != null)
                {
                    Debug.Log("Removing scav on currently selected slot.");

                    questionIcons[currentSlot].SetActive(true);
                    StartCoroutine(RemoveScavenger(currentSlot));
                    yield return new WaitForSeconds(1f);
                }

                scavengerTeam[currentSlot] = scavenger;
                questionIcons[currentSlot].SetActive(false);
                scavengerPlatforms[currentSlot].GetComponent<Image>().color = selectedSlotColor;
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

                questionIcons[currentSlot].SetActive(true);
                StartCoroutine(RemoveScavenger(currentSlot));
                yield return new WaitForSeconds(1f);
            }

            scavengerTeam[currentSlot] = scavenger;
            questionIcons[currentSlot].SetActive(false);
            scavengerPlatforms[currentSlot].GetComponent<Image>().color = selectedSlotColor;
            scavengerSlots[currentSlot].GetComponent<Image>().sprite = scavenger.characterFull;
            scavengerSlots[currentSlot].SetActive(true);

            yield return new WaitForSeconds(1f);

            scavengerSlots[currentSlot].GetComponent<Image>().fillAmount = 1;
        }
    }

    IEnumerator RemoveScavenger(int position)
    {
        scavengerPlatforms[position].GetComponent<Image>().color = defaultSlotColor;
        scavengerSlots[position].GetComponent<Animator>().SetBool("Remove", true);
        yield return new WaitForSeconds(1f);
        scavengerSlots[position].GetComponent<Animator>().SetBool("Remove", false);
        scavengerSlots[position].SetActive(false);

        scavengerTeam[position] = null;
    }

    public void EnterBattle()
    {
        if (dataController != null)
        {
            if (dataController.scavengerRoster.Count > 2)
            {
                int scavengerCount = 0;
                foreach (Player scavenger in scavengerTeam)
                {
                    if (scavenger != null)
                        scavengerCount++;
                }

                if (scavengerCount == 3)
                {
                    dataController.scavengerTeam = scavengerTeam;
                    dataController.wasteTeam = wasteTeam;
                }
                else
                {

                }
            }
            else
            {
                int scavengerCount = 0;
                foreach (Player scavenger in scavengerTeam)
                {
                    if (scavenger != null)
                        scavengerCount++;
                }

                if (scavengerCount == 2)
                {
                    dataController.scavengerTeam = scavengerTeam;
                    dataController.wasteTeam = wasteTeam;
                }
                else
                {

                }
            }
        }
    }

    public void CancelBattle()
    {
        for (int i = 0; i < scavengerTeam.Length; i++)
        {
            if (scavengerTeam[i] != null)
                StartCoroutine(RemoveScavenger(i));
        }

        for (int i = 0; i < wasteTeam.Length; i++)
        {
            if (wasteTeam[i] != null)
                wasteTeam[i] = null;
        }
    }
}
