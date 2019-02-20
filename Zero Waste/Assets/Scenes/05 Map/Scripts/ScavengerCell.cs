using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengerCell : MonoBehaviour
{
    public Player player;

    public void SelectScavenger()
    {
        StartCoroutine(GameObject.FindObjectOfType<TeamSelect>().AddScavengerToTeam(player));
    }
}
