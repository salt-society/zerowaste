using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScavengerRoster : MonoBehaviour
{
    public DataController dataController;
    public TeamSelect teamSelectManager;

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
        GameObject scavengerCell;
        if (dataController != null)
            maxNoOfCells = dataController.scavengerRoster.Count;

        for (int i = 1; i < maxNoOfCells; i++)
        {
            scavengerCell = Instantiate(scavengerCellPrefab, transform);

            if (dataController != null)
            {
                scavengerCell.GetComponent<ScavengerCell>().SetScavengerData(dataController.scavengerRoster[i], teamSelectManager);
                scavengerCell.GetComponent<ScavengerCell>().SetScavengerIndex(i);

                Debug.Log(scavengerCell.GetComponent<ScavengerCell>().GetScavengerData().characterName);

                scavengerCell.transform.GetChild(1).GetChild(1).gameObject.
                    GetComponent<TextMeshProUGUI>().text = dataController.scavengerRoster[i].currentLevel.ToString();

                scavengerCell.transform.GetChild(2).gameObject.
                    GetComponent<TextMeshProUGUI>().text = dataController.scavengerRoster[i].characterName;

                scavengerCell.transform.GetChild(0).GetChild(1).gameObject.
                    GetComponent<Image>().sprite = dataController.scavengerRoster[i].characterHalf;
            }
        }
    }
}
