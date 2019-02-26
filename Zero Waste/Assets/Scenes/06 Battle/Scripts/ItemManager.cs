using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemManager : MonoBehaviour
{
    [Header("Booster Components")]
    public GameObject boosterIcon;
    public GameObject boosterPanel;
    public GameObject trashcanBox;

    [Space]
    public GameObject[] boosters;
    public Image scavengerIcon;

    public void DisplayTrashcanBox(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        trashcanBox.SetActive(showComponent);
    }

    public void SetBoosterPanelBg(Sprite backgroundSprite)
    {
        boosterPanel.GetComponent<Image>().sprite = backgroundSprite;
    }

    public void SetBoosters()
    {

    }

    public void SetCurrentScavenger(Player scavenger)
    {
        scavengerIcon.sprite = scavenger.characterThumb;
    }

    public void ShowBoosterPanel(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        boosterPanel.SetActive(showComponent);
    }
}
