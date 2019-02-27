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

    [SerializeField]
    public Node nodeData;

    private MapController mapController;
    private NodeManager nodeManager;

    private GameObject nodeDetails;
    private GameObject levelList;

    private GameObject cellToUnlock;
    private List<GameObject> cellsToUnlock;

    public void SetNodeData(Node nodeData)
    {
        this.nodeData = nodeData;
    }

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
                if (dataController != null)
                {
                    if (cell != dataController.currentBattleObject)
                    {
                        Debug.Log("Destroying " + cell.gameObject.name);
                        Destroy(cell.gameObject);
                    }
                }
                else
                {
                    Debug.Log("Destroying " + cell.gameObject.name);
                    Destroy(cell.gameObject);
                }
            }

            gameObject.SetActive(false);
        }
    }

    public void PopulateGrid()
    {
        GameObject battleCell;
        cellsToUnlock = new List<GameObject>();

        int nodeDataIndex = 0;
        for (int battleIndex = nodeData.battleStartIndex; battleIndex <= nodeData.battleEndIndex; battleIndex++)
        {
            Debug.Log("Loop " + (nodeDataIndex + 1));

            battleCell = Instantiate(levelCellPrefab, transform);
            battleCell.GetComponent<LevelManager>().SetBattleData(nodeData.battles[nodeDataIndex], battleCell);
            Debug.Log("Current Data: nodeData.battles[" + nodeDataIndex + "]");
            nodeDataIndex++;

            battleCell.GetComponent<LevelManager>().SetMapController(mapController);
            battleCell.GetComponent<LevelManager>().SetNodeManager(nodeManager);
            battleCell.GetComponent<LevelManager>().SendNodeDetailComponents(nodeDetails);
            battleCell.GetComponent<LevelManager>().SendLevelListComponents(levelList);

            battleCell.transform.GetChild(0).transform.gameObject.
                GetComponent<TextMeshProUGUI>().text = battleIndex.ToString();

            if (dataController != null)
            { 
                // Battle is unlocked if Battle Id is already in Battle Dictionary
                // Make cell interactable if Battle is unlocked
                if (dataController.currentSaveData.battles.ContainsKey(battleIndex))
                {
                    battleCell.GetComponent<Button>().interactable = true;

                    // Battle unlock doesn't mean its finished
                    // Check if battle is done and mark it green
                    if (dataController.currentSaveData.battles[battleIndex])
                    {
                        battleCell.GetComponent<Image>().color = finishedColor;
                        battleCell.transform.GetChild(0).transform.gameObject.SetActive(true);
                        battleCell.transform.GetChild(1).transform.gameObject.SetActive(false);
                    }
                    // If not finished, show unlocked animation
                    // Its possible to unlock more than 1 battle inside a node
                    else
                    {
                        // Check if battle is already played
                        // If yes, do not repeat unlock animation
                        if (dataController.currentSaveData.isBattlePlayed[battleIndex])
                        {
                            battleCell.GetComponent<Image>().color = currentColor;
                            battleCell.transform.GetChild(0).transform.gameObject.SetActive(true);
                            battleCell.transform.GetChild(1).transform.gameObject.SetActive(false);

                            Debug.Log("Battle " + battleIndex + ": Played but not yet finished.");
                        }
                        else
                        {
                            Debug.Log("Battle " + battleIndex + ": Not played.");
                            cellsToUnlock.Add(battleCell);
                        }
                    }
                }
                // Cell is not interactable if battle isn't unlocked
                else
                {
                    battleCell.GetComponent<Button>().interactable = false;
                }
            }
        }
    }

    public IEnumerator UnlockBattles()
    {
        foreach (GameObject cellToUnlock in cellsToUnlock)
        {
            yield return new WaitForSeconds(.5f);

            cellToUnlock.transform.GetComponent<Animator>().SetBool("Unlock", true);
            yield return new WaitForSeconds(1f);
            cellToUnlock.transform.GetComponent<Animator>().SetBool("Unlock", false);
            cellToUnlock.transform.GetChild(1).gameObject.SetActive(false);

            cellToUnlock.transform.GetChild(0).transform.gameObject.SetActive(true);
            cellToUnlock.GetComponent<Image>().color = currentColor;
        }
    }
}
