using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    private Battle battle;

    private GameObject level;
    private MapController mapController;
    private NodeManager nodeManager;

    private GameObject nodeDetails;
    private GameObject levelList;

    public void SetBattleData(Battle battle, GameObject level)
    {
        this.battle = battle;
        this.level = level;
    }

    public Battle GetBattleData()
    {
        return battle;
    }

    public void SetMapController(MapController mapController)
    {
        this.mapController = mapController;
    }

    public void SetNodeManager(NodeManager nodeManager)
    {
        this.nodeManager = nodeManager;
    }

    public void SendNodeDetailComponents(GameObject nodeDetails)
    {
        this.nodeDetails = nodeDetails;
    }

    public void SendLevelListComponents(GameObject levelList)
    {
        this.levelList = levelList;
    }

    public void SelectLevel()
    {
        mapController.SetCurrentSelectedBattle(gameObject);
        StartCoroutine(ShowBattleDetails());
    }

    public void SetBattleDetails()
    {
        TextMeshProUGUI nodeName = nodeDetails.transform.GetChild(2).
            gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        nodeName.text = battle.battleName;

        TextMeshProUGUI description = nodeDetails.transform.GetChild(3).
            gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    IEnumerator ShowBattleDetails()
    {
        SetBattleDetails();

        levelList.GetComponent<Animator>().SetBool("Exit", true);
        yield return new WaitForSeconds(1f);
        levelList.GetComponent<Animator>().SetBool("Exit", false);
        levelList.SetActive(false);

        nodeDetails.SetActive(true);
    }

    public IEnumerator HideBattleDetails()
    {
        nodeDetails.GetComponent<Animator>().SetBool("Exit", true);
        yield return new WaitForSeconds(1f);
        nodeDetails.GetComponent<Animator>().SetBool("Exit", false);
        nodeDetails.SetActive(false);

        levelList.SetActive(true);
    }
}
