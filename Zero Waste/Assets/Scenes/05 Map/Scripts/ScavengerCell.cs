using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScavengerCell : MonoBehaviour
{
    private Player scavenger;
    private TeamSelect teamSelectManager;
    public int index;

    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(SelectScavenger);
    }

    public void SetScavengerData(Player scavenger, TeamSelect teamSelectManager)
    {
        this.scavenger = scavenger;
        this.teamSelectManager = teamSelectManager;
    }

    public Player GetScavengerData()
    {
        return scavenger;
    }

    public void SetScavengerIndex(int index)
    {
        this.index = index;
    }

    public void SelectScavenger()
    {
        Debug.Log(index);
        StartCoroutine(teamSelectManager.AddScavengerToTeam(index));
    }
}
