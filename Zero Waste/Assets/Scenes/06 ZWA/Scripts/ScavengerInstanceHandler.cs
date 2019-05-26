using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScavengerInstanceHandler : MonoBehaviour
{
    private Player heldPlayer;

    private GameObject trainingLab;

    public void SetPlayer(Player thisPlayer)
    {
        heldPlayer = thisPlayer;

        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = heldPlayer.characterThumb;

        gameObject.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = heldPlayer.currentLevel.ToString();

        gameObject.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = heldPlayer.characterName;
    }

    public void SendPlayer()
    {
        trainingLab.GetComponent<TrainingController>().SetSelectedScavenger(heldPlayer);
    }

    private void Awake()
    {
        trainingLab = GameObject.Find("Training Lab");
    }
}
