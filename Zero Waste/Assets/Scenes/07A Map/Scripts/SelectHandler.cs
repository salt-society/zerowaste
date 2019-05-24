using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectHandler : MonoBehaviour
{
    private Player heldPlayer;

    public void SetPlayer(Player player)
    {
        heldPlayer = player;

        DesignSlot();
    }

    private void DesignSlot()
    {
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = heldPlayer.characterHalf;
        gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = heldPlayer.currentLevel.ToString();
        gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = heldPlayer.characterName;
        gameObject.transform.GetChild(3).GetComponent<Image>().sprite = heldPlayer.characterClass.roleLogo;
    }

    private void Start()
    {
        heldPlayer = new Player();
    }
}
