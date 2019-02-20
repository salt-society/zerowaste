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
    public int maxNoOfCells;

    void Start()
    {
        dataController = GameObject.FindObjectOfType<DataController>();
        PopulateGrid();
    }

    public void PopulateGrid()
    {
        if (dataController != null)
            maxNoOfCells = dataController.scavengerRoster.Count;

        for (int i = 0; i < maxNoOfCells; i++)
        {
            GameObject scavengerCell;
            scavengerCell = Instantiate(scavengerCellPrefab, transform);

            if (dataController != null)
            {
                scavengerCell.GetComponent<ScavengerCell>().SetScavengerData(dataController.scavengerRoster[i]);
                scavengerCell.GetComponent<ScavengerCell>().SetScavengerIndex(i);

                Debug.Log(scavengerCell.GetComponent<ScavengerCell>().GetScavengerData().characterName);

                scavengerCell.transform.GetChild(2).gameObject.
                    GetComponent<TextMeshProUGUI>().text = dataController.scavengerRoster[i].characterName;
                scavengerCell.transform.GetChild(0).GetChild(0).gameObject.
                    GetComponent<Image>().sprite = dataController.scavengerRoster[i].characterHalf;
            }
        }
    }
}
