using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelList : MonoBehaviour
{
    public DataController dataController;

    [Space]
    public GameObject levelCellPrefab;

    [Space]
    public Color finishedColor;
    public Color currentColor;

    [Space]
    public Node nodeData;
    public int tempUnlockedBattleCount;

    private int maxNoCells;

    private MapController mapController;
    private NodeManager nodeManager;

    private GameObject nodeDetails;
    private GameObject levelList;

    public void SendBattleComponents(GameObject nodeDetails, GameObject levelList)
    {
        this.nodeDetails = nodeDetails;
        this.levelList = levelList;
    }

    public void SendScripts(MapController mapController, NodeManager nodeManager)
    {
        this.mapController = mapController;
        this.nodeManager = nodeManager;

        dataController = mapController.dataController;
    }

    public void RemoveCellsfFromGrid()
    {
        if (transform.childCount > 0)
        {
            foreach (Transform cell in transform)
            {
                Debug.Log("Destroying " + cell.gameObject.name);
                Destroy(cell.gameObject);
            }

            gameObject.SetActive(false);
        }
    }

    public void PopulateGrid()
    {
        GameObject levelCell;
        maxNoCells = nodeData.battles.Count;

        for (int i = 0; i < maxNoCells; i++)
        {
            levelCell = Instantiate(levelCellPrefab, transform);
            levelCell.GetComponent<LevelManager>().SetBattleData(nodeData.battles[i], levelCell);
            levelCell.GetComponent<LevelManager>().SetMapController(mapController);
            levelCell.GetComponent<LevelManager>().SetNodeManager(nodeManager);
            levelCell.GetComponent<LevelManager>().SendNodeDetailComponents(nodeDetails);
            levelCell.GetComponent<LevelManager>().SendLevelListComponents(levelList);

            levelCell.transform.GetChild(0).transform.gameObject.
                GetComponent<TextMeshProUGUI>().text = i.ToString();

            if (dataController != null)
            {
                if (dataController.currentSaveData.currentBattleId == -1)
                {
                    dataController.currentSaveData.currentBattleId++;
                    dataController.currentSaveData.UnlockedBattle();
                }

                if (i <= dataController.currentSaveData.currentBattleId)
                {
                    levelCell.GetComponent<Button>().interactable = true;

                    if (dataController.currentSaveData.unlockedBattles[i])
                        levelCell.GetComponent<Image>().color = finishedColor;
                    else
                        levelCell.GetComponent<Image>().color = currentColor;

                    levelCell.transform.GetChild(0).transform.gameObject.SetActive(true);
                    levelCell.transform.GetChild(1).transform.gameObject.SetActive(false);
                }
                else
                {
                    levelCell.GetComponent<Button>().interactable = false;
                }
            }
            else
            {
                if (i <= tempUnlockedBattleCount)
                {
                    if(i != tempUnlockedBattleCount)
                        levelCell.GetComponent<Image>().color = finishedColor;
                    else
                        levelCell.GetComponent<Image>().color = currentColor;

                    levelCell.GetComponent<Button>().interactable = true;
                    levelCell.transform.GetChild(0).transform.gameObject.SetActive(true);
                    levelCell.transform.GetChild(1).transform.gameObject.SetActive(false);
                }
                else
                {
                    levelCell.GetComponent<Button>().interactable = false;
                }
            }
        }
    }
}
