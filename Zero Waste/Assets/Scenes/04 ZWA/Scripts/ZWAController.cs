using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZWAController : MonoBehaviour
{
    [Header("Controllers")]
    private DataController dataController;

    [Header("ZWA")]
    public GameObject zwaSign;
    public GameObject mapPoints;

    [Space]
    public List<AreaController> areaCtrls;

    [Space]
    public List<Button> points;
    public List<GameObject> areasUI;
    public List<GameObject> areas;


    void Start()
    {
        
    }

    public void OpenArea(int areaId)
    {
        // Hide ZWA Sign and Map Points
        zwaSign.SetActive(!zwaSign.activeInHierarchy);
        mapPoints.SetActive(!mapPoints.activeInHierarchy);

        // Show Area and UI
        areasUI[areaId].SetActive(!areasUI[areaId].activeInHierarchy);
        areas[areaId].SetActive(!areas[areaId].activeInHierarchy);

        // Show parts of Area
        StartCoroutine(areaCtrls[areaId].ShowInteractableParts());
    }
}
