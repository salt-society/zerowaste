using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScavengerRoster : MonoBehaviour
{
    public DataController dataController;

    [Space]
    public GameObject scavengerCellPrefab;

    [Space]
    public List<Player> tempoScavRoster;

    private int maxNoOfCells;

    void Start()
    {
        dataController = GameObject.FindObjectOfType<DataController>();
        if (dataController != null)
        {
            maxNoOfCells = dataController.scavengerRoster.Count;
        }
        else
        {
            maxNoOfCells = tempoScavRoster.Count;
        }
        
        PopulateGrid();
    }

    void PopulateGrid()
    {
        GameObject scavengerCell;

        for (int i = 1; i < maxNoOfCells; i++)
        {
            scavengerCell = Instantiate(scavengerCellPrefab, transform);

            if (dataController != null)
            {
                scavengerCell.GetComponent<ScavengerCell>().player = dataController.scavengerRoster[i];

                scavengerCell.transform.GetChild(2).gameObject.
                    GetComponent<TextMeshProUGUI>().text = dataController.scavengerRoster[i].characterName;
                scavengerCell.transform.GetChild(0).GetChild(0).gameObject.
                    GetComponent<Image>().sprite = dataController.scavengerRoster[i].characterHalf;
            }
            else
            {
                scavengerCell.GetComponent<ScavengerCell>().player = tempoScavRoster[i];

                scavengerCell.transform.GetChild(2).gameObject.
                    GetComponent<TextMeshProUGUI>().text = tempoScavRoster[i].characterName;
                scavengerCell.transform.GetChild(0).GetChild(0).gameObject.
                    GetComponent<Image>().sprite = tempoScavRoster[i].characterHalf;
            }
            
        }
    }
}
