using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InstanceHandler : MonoBehaviour
{
    private Player heldPlayer;

    private GameObject roster;

    public void SetPlayer(Player thisPlayer)
    {
        heldPlayer = thisPlayer;

        gameObject.GetComponent<Image>().sprite = heldPlayer.characterHalf;

        gameObject.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = heldPlayer.characterName;
        gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = heldPlayer.currentLevel.ToString();
    }

    public void SendPlayer()
    {
        roster.GetComponent<RosterHandler>().HasSelectedScavenger(heldPlayer);
        roster.GetComponent<RosterHandler>().HideRoster();
    }

    private void Awake()
    {
        roster = GameObject.Find("Scavenger Roster");
    }
}
