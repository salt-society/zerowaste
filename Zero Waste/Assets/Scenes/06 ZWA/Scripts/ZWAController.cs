using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZWAController : MonoBehaviour
{
    private DataController dataController;

    [Space]
    public GameObject raycastBlock;

    [Space]
    public GameObject zwaSign;
    public GameObject mapPoints;

    [Space]
    public List<AreaController> areaCtrls;

    [Space]
    public List<Button> points;
    public List<GameObject> areasUI;
    public List<GameObject> areas;

    [Space]
    public GameObject fadeObject;

    void Start()
    {
        dataController = FindObjectOfType<DataController>();


    }

    public void OpenArea(int areaId)
    {
        zwaSign.SetActive(!zwaSign.activeInHierarchy);
        mapPoints.SetActive(!mapPoints.activeInHierarchy);

        areasUI[areaId].SetActive(!areasUI[areaId].activeInHierarchy);
        areas[areaId].SetActive(!areas[areaId].activeInHierarchy);

        StartCoroutine(areaCtrls[areaId].ShowInteractableParts());
    }
}
