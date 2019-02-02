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

    [Header("Pop Up Components")]
    public GameObject battleStart;
    public GameObject turnProcessSign;

    // Shows start animation upon battle load
    public void ShowStartAnimation(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        battleStart.SetActive(showComponent);
    }

    // Sets message to display on middle box
    public void SetMiddleMessage(string message)
    {
        turnProcessSign.GetComponentInChildren<TextMeshProUGUI>().text = message;
    }

    // Shows or hide message on top center
    public void DisplayMiddleMessage(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        turnProcessSign.SetActive(showComponent);
    }

    // Sets battle details that can be seen on the top left
    public void SetBattleDetails(string battleNo, string nodeName)
    {
        this.battleNo.text = battleNo;
        this.nodeName.text = nodeName;
    }

    // Displays or hide battle details
    public void DisplayBattleDetails(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        battleInfo.SetActive(showComponent);
    }
}
