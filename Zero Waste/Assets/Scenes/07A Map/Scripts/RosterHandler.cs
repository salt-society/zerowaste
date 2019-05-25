using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RosterHandler : MonoBehaviour
{

    #region Editor Variables

    [Header("Team Select")]
    public GameObject teamSelectHandler;

    [Header("Scroll View")]
    public GameObject scrollview;

    [Header("Prefabs")]
    public GameObject scavengerPrefab;

    #endregion

    #region Private Variables

    private int chosenSlot;

    private DataController dataController;

    #endregion

    // Show Roster Screen and populate
    public void ShowRoster(int index)
    {
        chosenSlot = index;

        List<Player> roster = dataController.scavengerRoster;

        ClearRoster();

        for(int CTR = 0; CTR < roster.Count; CTR++)
        {
            if (CTR == 0)
                continue;

            else
            {
                GameObject instancePrefab = Instantiate(scavengerPrefab, scrollview.transform);
                instancePrefab.GetComponent<InstanceHandler>().SetPlayer(roster[CTR]);
            }
        }
    }

    public void HasSelectedScavenger(Player player)
    {
        teamSelectHandler.GetComponent<TeamSelectHandler>().AddScavenger(chosenSlot, player);
    }

    public void HideRoster()
    {
        gameObject.SetActive(false);
    }

    // Clear contents
    private void ClearRoster()
    {
        if(scrollview.transform.childCount > 0)
        {
            foreach (Transform child in scrollview.transform)
                Destroy(child.gameObject);
        }
    }

    private void Awake()
    {
        dataController = FindObjectOfType<DataController>();
    }
}
