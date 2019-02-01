using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleInfoManager : MonoBehaviour {

    // This class will handle battle details such as
    // battle name, damage points, number of turns

    [Header("Battle Information")]
    public TextMeshProUGUI battleNo;
    public TextMeshProUGUI nodeName;
    public GameObject battleInfo;

    [Header("Pop Ups")]
    public GameObject battleStart;
    public GameObject turnProcessSign;

    public void DisplayStart(bool isActive)
    {
        battleStart.SetActive(isActive);
    }

    public void DisplayTurnProcess(bool isActive)
    {
        turnProcessSign.SetActive(isActive);
    }

    public void DisplayBattleDetails(string battleNo, string nodeName)
    {
        this.battleNo.text = battleNo;
        this.nodeName.text = nodeName;
        battleInfo.SetActive(true);
    }
}
