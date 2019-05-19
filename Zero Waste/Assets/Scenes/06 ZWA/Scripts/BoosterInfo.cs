using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoosterInfo : MonoBehaviour
{
    public DataController dataController;

    [Space]
    public Booster booster;
    public int quantity;

    [Space]
    public GameObject instruction;
    public GameObject boosterInfo;
    public GameObject boosterSelected;

    public void SetBoosterInfo()
    {
        boosterInfo.transform.GetChild(2).GetComponent<Image>().sprite = booster.typeIcon;
        boosterInfo.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = booster.typeIcon;
        boosterInfo.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = booster.type;
        boosterInfo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = quantity.ToString();

        foreach (Effect effect in booster.effects)
            boosterInfo.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = effect.description + " ";
    }

    public void ShowBoosterInfo()
    {
        instruction.SetActive(!instruction.activeInHierarchy);
        boosterInfo.SetActive(!boosterInfo.activeInHierarchy);
    }

    public void SelectBooster()
    {
        if (dataController == null)
            return;

        if (dataController.boosterInfo == null)
        {
            dataController.boosterInfo = this;

            HightlightBooster();
            SetBoosterInfo();
            ShowBoosterInfo();
        }
        else if (dataController.boosterInfo == this)
        {
            dataController.boosterInfo = null;
            HightlightBooster();
            ShowBoosterInfo();
        }
        else
        {
            dataController.boosterInfo.GetComponent<BoosterInfo>().HightlightBooster();
            dataController.boosterInfo.GetComponent<BoosterInfo>().ShowBoosterInfo();

            SetBoosterInfo();
            HightlightBooster();
            ShowBoosterInfo();

            dataController.boosterInfo = this;
        }
    }

    public void HightlightBooster()
    {
        boosterSelected.SetActive(!boosterSelected.activeInHierarchy);
    }
}
