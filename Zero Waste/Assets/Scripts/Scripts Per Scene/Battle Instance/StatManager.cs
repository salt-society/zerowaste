using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatManager : MonoBehaviour {

    [Header("Statistics UI")]
    [Header("Scavengers")]
    public TextMeshProUGUI currentScavengerName;
    public Image currentScavengerClassIcon;
    public TextMeshProUGUI currentScavengerLevel;
    public TextMeshProUGUI currentScavengerClass;

    [Space]
    public Image[] scavengerIcon;
    public Image[] scavengerHealthBars;
    public Image[] scavengerAntidoteBars;

    public GameObject[] damageCounters;

    public void UpdateStat(Player scavenger, int position)
    {
        if (position.Equals(0)) { // Current Scavenger
            scavengerIcon[position].sprite = scavenger.characterImage;
            currentScavengerName.text = scavenger.characterName;
            currentScavengerClassIcon.sprite = scavenger.characterClass.roleLogo;
            currentScavengerClass.text = scavenger.characterClass.roleName;

        } else { // Remaining Scavenger

        }
    }

}
