using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MutantInfo : MonoBehaviour
{
    public DataController dataController;
    public Enemy mutant;

    [Space]
    public GameObject mutantSelected;
    public GameObject mutantInfoPanel;
    public GameObject parentExit;

    [Space]
    public Sprite areaEcountered;
    public string areaName;
    
    public void SetMutantInfo()
    {
        mutantInfoPanel.transform.GetChild(0).GetComponent<Image>().sprite = areaEcountered;
        mutantInfoPanel.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Unlocked In " + areaName;

        mutantInfoPanel.transform.GetChild(1).GetComponent<Image>().sprite = mutant.characterFull;
        mutantInfoPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = mutant.characterName;
        mutantInfoPanel.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = mutant.description;

        // PL
        mutantInfoPanel.transform.GetChild(4).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = mutant.basePollutionLevel.ToString();
        mutantInfoPanel.transform.GetChild(4).GetChild(0).GetChild(2).GetChild(0).
            GetComponent<TextMeshProUGUI>().text = "+ " + mutant.plModifier.ToString() + " Per Level Up";

        // ATK
        mutantInfoPanel.transform.GetChild(4).GetChild(0).GetChild(5).GetComponent<TextMeshProUGUI>().text = mutant.baseAtk.ToString();
        mutantInfoPanel.transform.GetChild(4).GetChild(0).GetChild(5).GetChild(0).
            GetComponent<TextMeshProUGUI>().text = "+ " + mutant.atkModifier.ToString() + " Per Level Up";

        // DEF
        mutantInfoPanel.transform.GetChild(4).GetChild(0).GetChild(8).GetComponent<TextMeshProUGUI>().text = mutant.baseDef.ToString();
        mutantInfoPanel.transform.GetChild(4).GetChild(0).GetChild(8).GetChild(0).
            GetComponent<TextMeshProUGUI>().text = "+ " + mutant.defModifier.ToString() + " Per Level Up";

        // SPD
        mutantInfoPanel.transform.GetChild(4).GetChild(0).GetChild(11).GetComponent<TextMeshProUGUI>().text = mutant.baseSpd.ToString();
        mutantInfoPanel.transform.GetChild(4).GetChild(0).GetChild(11).GetChild(0).
            GetComponent<TextMeshProUGUI>().text = "No Modifier";

        // Skill 1
        mutantInfoPanel.transform.GetChild(4).GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = mutant.abilities[1].abilityName;
        mutantInfoPanel.transform.GetChild(4).GetChild(1).GetChild(2).GetChild(0).
            GetComponent<TextMeshProUGUI>().text = mutant.abilities[1].abilityDescription;

        // Skill 2
        mutantInfoPanel.transform.GetChild(4).GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>().text = mutant.abilities[2].abilityName;
        mutantInfoPanel.transform.GetChild(4).GetChild(1).GetChild(3).GetChild(0).
            GetComponent<TextMeshProUGUI>().text = mutant.abilities[2].abilityDescription;

        // Ultimate
        mutantInfoPanel.transform.GetChild(4).GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>().text = mutant.abilities[3].abilityName;
        mutantInfoPanel.transform.GetChild(4).GetChild(1).GetChild(4).GetChild(0).
            GetComponent<TextMeshProUGUI>().text = mutant.abilities[3].abilityDescription;

    }

    public void ShowMutantInfo()
    {
        StartCoroutine(ShowMutantInfoIE());
    }

    IEnumerator ShowMutantInfoIE()
    {
        HighlightMutant();
        yield return new WaitForSeconds(1f);

        SetMutantInfo();
        mutantInfoPanel.SetActive(!mutantInfoPanel.activeInHierarchy);
    }

    public void HighlightMutant()
    {
        mutantSelected.SetActive(!mutantSelected.activeInHierarchy);
    }

    public void SelectMutant()
    {
        if (dataController == null)
            return;

        ShowMutantInfo();
        ShowHidePExit();
        dataController.mutantInfo = this;
    }

    public void CloseMutantInfo()
    {
        StartCoroutine(CloseMutantInfoIE());
    }

    IEnumerator CloseMutantInfoIE()
    {
        mutantInfoPanel.SetActive(!mutantInfoPanel.activeInHierarchy);
        yield return new WaitForSeconds(1f);

        HighlightMutant();
        ShowHidePExit();
        dataController.mutantInfo = null;
    }

    public void ShowHidePExit()
    {
        parentExit.SetActive(!parentExit.activeInHierarchy);
    }
}
