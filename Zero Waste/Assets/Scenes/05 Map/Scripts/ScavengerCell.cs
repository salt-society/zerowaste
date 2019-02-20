using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengerCell : MonoBehaviour
{
    private Player scavenger;
    public int index;

    public void SetScavengerData(Player scavenger)
    {
        this.scavenger = scavenger;
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
        StartCoroutine(GameObject.FindObjectOfType<TeamSelect>().AddScavengerToTeam(index));
    }
}
