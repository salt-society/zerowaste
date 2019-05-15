using System.Collections;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    #region Properties
    private DataController dataController;

    public DataController DataController
    {
        get { return dataController; }
        set { dataController = value; }
    }

    private MapController mapController;

    public MapController MapController
    {
        get { return mapController; }
        set { mapController = value; }
    }

    private NodeManager nodeManager;

    public NodeManager NodeManager
    {
        get { return nodeManager; }
        set { nodeManager = value; }
    }

    private Battle battle;

    public Battle Battle
    {
        get { return battle; }
        set { battle = value; }
    }

    private GameObject battleObject;

    public GameObject BattleObject
    {
        get { return battleObject; }
        set { battleObject = value; }
    }

    private GameObject battleDetails;

    public GameObject BattleDetails
    {
        get { return battleDetails; }
        set { battleDetails = value; }
    }

    private GameObject levelList;

    public GameObject LevelList
    {
        get { return levelList; }
        set { levelList = value; }
    }
    #endregion

    public void SetBattleData(Battle battle, GameObject level)
    {
        this.battle = battle;
        this.battleObject = level;
    }

    public Battle GetBattleData()
    {
        return battle;
    }

    public void SetMapController(MapController mapController)
    {
        this.mapController = mapController;
        dataController = mapController.dataController;
    }

    public void SetNodeManager(NodeManager nodeManager)
    {
        this.nodeManager = nodeManager;
    }

    public void SendNodeDetailComponents(GameObject nodeDetails)
    {
        this.battleDetails = nodeDetails;
    }

    public void SendLevelListComponents(GameObject levelList)
    {
        this.levelList = levelList;
    }

    public void SelectBattle()
    {
        // Just to be safe, always check if there's a data controller before execution
        if (dataController != null)
        {
            // Play SFX
            GameObject.FindObjectOfType<AudioManager>().PlaySound("Button Click 3");

            // Set current battle selected and its gameObject
            dataController.currentBattle = battle;
            dataController.currentBattleObject = gameObject;

            // Show players details of battle
            StartCoroutine(ShowBattleDetails());
        } 
    }

    public void SetBattleDetails()
    {
        // Set battle name
        TextMeshProUGUI nodeName = battleDetails.transform.GetChild(2).
            gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        nodeName.text = battle.battleName;

        // Set battle description
        TextMeshProUGUI description = battleDetails.transform.GetChild(3).
            gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    IEnumerator ShowBattleDetails()
    {
        // Set details of battle
        SetBattleDetails();

        // Hide list of battles
        levelList.GetComponent<Animator>().SetBool("Exit", true);
        yield return new WaitForSeconds(1f);
        levelList.GetComponent<Animator>().SetBool("Exit", false);
        levelList.SetActive(false);

        // Show battle details
        battleDetails.SetActive(true);
    }

    public IEnumerator HideBattleDetails()
    {
        // Hide battle details
        battleDetails.GetComponent<Animator>().SetBool("Exit", true);
        yield return new WaitForSeconds(1f);
        battleDetails.GetComponent<Animator>().SetBool("Exit", false);
        battleDetails.SetActive(false);

        // Show list of levels again
        levelList.SetActive(true);
    }
}
