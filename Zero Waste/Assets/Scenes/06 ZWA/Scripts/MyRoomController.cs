using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRoomController : AreaController
{
    [Header("Unique in My Room")]
    public GameObject bestiaryGrid;
    public GameObject boosterGrid;

    [Space]
    public List<GameObject> bedSubparts;
    public List<GameObject> bedSubpartsSelected;
    public GameObject bedExit;

    [Space]
    public GameObject saveGrid;

    private string currentPartName;

    public override IEnumerator ShowPartIE(PartIdentifier partIdentifier)
    {
        currentPartName = partIdentifier.partName;

        if (partIdentifier.partName.Equals("Shelf"))
        {
            bestiaryGrid.GetComponent<BestiaryGrid>().dataController = dataController;
            bestiaryGrid.GetComponent<BestiaryGrid>().PopulateGrid();
        }
        else if (partIdentifier.partName.Equals("Bag"))
        {
            boosterGrid.GetComponent<BoosterGrid>().dataController = dataController;
            boosterGrid.GetComponent<BoosterGrid>().PopulateGrid();
        }
        else if (partIdentifier.partName.Equals("Bed"))
        {
            
        }
        
        return base.ShowPartIE(partIdentifier);
    }

    public void ClosePart()
    {
        StartCoroutine(ClosePartIE());
    }

    public override IEnumerator ClosePartIE()
    {
        if (currentPartName.Equals("Shelf"))
        {
            bestiaryGrid.GetComponent<BestiaryGrid>().RemoveCells();
        }
        else if (currentPartName.Equals("Bag"))
        {
            boosterGrid.GetComponent<BoosterGrid>().RemoveCells();
            dataController.boosterInfo.ShowBoosterInfo();
        }
        else if (currentPartName.Equals("Bed"))
        {
            
        }
        
        return base.ClosePartIE();
    }

    public override void CloseArea()
    {
        base.CloseArea();
    }

    // Shelf
    public void CloseMutantInfo()
    {
        if (dataController == null)
            return;

        dataController.mutantInfo.CloseMutantInfo();
    }

    // Bed
    public void OpenSubpart(int subpartId)
    {
        bedSubparts[subpartId].SetActive(!bedSubparts[subpartId].activeInHierarchy);
        bedSubpartsSelected[subpartId].SetActive(!bedSubpartsSelected[subpartId].activeInHierarchy);
        bedExit.SetActive(!bedExit.activeInHierarchy);

        if (subpartId == 0)
        {
            saveGrid.GetComponent<SaveGrid>().dataController = dataController;
            saveGrid.GetComponent<SaveGrid>().DisplaySaveDetails();
        }
    }
}
