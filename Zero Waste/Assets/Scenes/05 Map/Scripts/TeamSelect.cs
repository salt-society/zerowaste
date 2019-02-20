using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelect : MonoBehaviour
{
    public DataController dataController;

    [Space]
    public GameObject scavengerRoster;
    public GameObject boosterRoster;

    [Space]
    public List<GameObject> scavengerSlots;
    
    private Player[] scavengerTeam;
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

            scavengerSlots[0].GetComponent<Image>().sprite = dataController.scavengerRoster[0].characterFull;
            yield return new WaitForSeconds(1f);
            scavengerSlots[0].SetActive(true);
        }
    }

    public void SelectScavenger(int position)
    {
        currentSlot = position;
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
        if (!scavengerSlots[currentSlot].activeInHierarchy)
        {
            scavengerSlots[currentSlot].GetComponent<Animator>().SetBool("Remove", true);
            yield return new WaitForSeconds(.8f);
            scavengerSlots[currentSlot].GetComponent<Animator>().SetBool("Remove", false);
            scavengerSlots[currentSlot].SetActive(false);
            yield return null;
        }

        scavengerTeam[currentSlot] = scavenger;

        scavengerSlots[currentSlot].GetComponent<Image>().sprite = scavenger.characterFull;
        scavengerSlots[currentSlot].SetActive(true);
    }
}
