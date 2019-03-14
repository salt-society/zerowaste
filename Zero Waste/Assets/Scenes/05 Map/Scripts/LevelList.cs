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

    private GameObject battleDetails;
    private GameObject levelList;

    private GameObject cellToUnlock;
    private List<GameObject> cellsToUnlock;

    public void SetNodeData(Node nodeData)
    {
        this.nodeData = nodeData;
    }

    public void SendBattleComponents(GameObject nodeDetails, GameObject levelList)
    {
        this.battleDetails = nodeDetails;
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
        // Just to be safe, always check if there's a data controller before execution
        if (dataController == null)
            return;

        // Reason for doing this is that the game makes use of one battle
        // grid view that will be called every node is clicked to show
        // list of battles at the same time, cells created on grid view stays

        // Check if there's a child, battle cell, first to destroy
        if (transform.childCount > 0)
        {
            // Get all the transforms
            foreach (Transform cell in transform)
            {
                // Destroy cell if its not the current battle object
                if (cell != dataController.currentBattleObject)
                {
                    Debug.Log("Destroying " + cell.gameObject.name);
                    Destroy(cell.gameObject);
                }
            }

            // Deactivates grid view so if called again
            // cells will repopulate base on the new node data
            gameObject.SetActive(false);
        }

        // [NOTE]
        // Will try object pooling as creating objects and destroying
        // can take too much resource especially on mobile devices
    }

    public void PopulateGrid()
    {
        // Just to be safe, always check if there's a data controller before execution
        if (dataController == null)
            return;

        // Create one gameObject that will serve as container of the prefab
        // and initialize list of gameObject where battles to be unlocked will be stored
        GameObject battleCell;
        cellsToUnlock = new List<GameObject>();

        // Since battle id increment from first to last area, index in the loop 
        // should start with the starting index of a battle within each node
        // and end with the last battle's index. Use another counter for getting battles
        int nodeDataIndex = 0;
        for (int battleIndex = nodeData.battleStartIndex; battleIndex <= nodeData.battleEndIndex; battleIndex++)
        {
            // Create prefab instance
            battleCell = Instantiate(levelCellPrefab, transform);

            // Set every the battle object needs to know so it can handle itself
            battleCell.GetComponent<LevelManager>().Battle = nodeData.battles[nodeDataIndex];
            nodeDataIndex++;

            battleCell.GetComponent<LevelManager>().BattleObject = battleCell; 
            battleCell.GetComponent<LevelManager>().MapController = mapController;
            battleCell.GetComponent<LevelManager>().DataController = mapController.dataController;
            battleCell.GetComponent<LevelManager>().NodeManager = nodeManager;
            battleCell.GetComponent<LevelManager>().BattleDetails = battleDetails;
            battleCell.GetComponent<LevelManager>().LevelList = levelList;

            // Set battle/level number
            battleCell.transform.GetChild(0).transform.gameObject.
                GetComponent<TextMeshProUGUI>().text = (battleIndex + 1).ToString();

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
                // If not finished, show unlock animation
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
                    // Always play unlock animation to remind players
                    // that that battle is still untouched
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

    public IEnumerator UnlockBattles()
    {
        // Loops through all battles to unlock and unlocks one at a time
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
