using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartIdentifier : MonoBehaviour
{
    public string partName;
    public int partId;
    public GameObject partSelected;
    public GameObject tooltip;

    public void Highlight()
    {
        tooltip.SetActive(!tooltip.activeInHierarchy);
        partSelected.SetActive(!partSelected.activeInHierarchy);
    }
}
