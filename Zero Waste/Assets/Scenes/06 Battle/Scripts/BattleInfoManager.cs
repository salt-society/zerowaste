using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleInfoManager : MonoBehaviour {

    [Header("Battle Information")]
    public TextMeshProUGUI battleNo;
    public TextMeshProUGUI nodeName;
    public GameObject battleInfo;

    [Header("Pop Up Components")]
    public GameObject battleStart;
    public GameObject turnProcessSign;
    public GameObject turnSignPanel;
    public GameObject turnSignInnerBox;
    public Image currentCharacter;
    public TextMeshProUGUI turnNumber;

    [Space]
    public GameObject battleResult;

    public void ShowStartAnimation(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        battleStart.SetActive(showComponent);
    }

    public void SetMiddleMessage(string message)
    {
        turnProcessSign.GetComponentInChildren<TextMeshProUGUI>().text = message;
    }

    public void DisplayMiddleMessage(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        turnProcessSign.SetActive(showComponent);
    }

    public void ShowMiddleMessage(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        turnProcessSign.GetComponent<Animator>().SetBool("Target", showComponent);
    }

    public void HideMiddleMessage(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        turnProcessSign.GetComponent<Animator>().SetBool("Hide", showComponent);
    }

    public bool GetMiddleMessageState()
    {
        bool visible = turnProcessSign.GetComponent<Animator>().GetBool("Target");
        return visible;
    }

    public void SetBattleDetails(string battleNo, string nodeName)
    {
        this.battleNo.text = battleNo;
        this.nodeName.text = nodeName;
    }

    public void DisplayBattleDetails(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        battleInfo.SetActive(showComponent);
    }

    public void SetCurrentTurn(Sprite currentCharacter, string name)
    {
        this.currentCharacter.sprite = currentCharacter;
        this.turnNumber.text = name + "'s Turn";
    }

    public void DisplayNextTurnPanel(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        turnSignPanel.SetActive(showComponent);
    }

    public void DisplayNextTurnSign(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        turnSignInnerBox.SetActive(showComponent);
    }

    public void DisplayBattleResult()
    {
        battleResult.SetActive(true);
    }
}
