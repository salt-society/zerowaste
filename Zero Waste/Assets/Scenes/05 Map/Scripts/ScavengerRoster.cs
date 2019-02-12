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
        maxNoOfCells = tempoScavRoster.Count;
        PopulateGrid();
    }

    void PopulateGrid()
    {
        GameObject scavengerCell;

        for (int i = 0; i < maxNoOfCells; i++)
        {
            scavengerCell = Instantiate(scavengerCellPrefab, transform);
            scavengerCell.GetComponent<ScavengerCell>().player = tempoScavRoster[i];

            scavengerCell.transform.GetChild(0).gameObject.
                GetComponent<TextMeshProUGUI>().text = tempoScavRoster[i].characterName;
            scavengerCell.transform.GetChild(1).gameObject.
                GetComponent<Image>().sprite = tempoScavRoster[i].characterThumb;
        }
    }
}
