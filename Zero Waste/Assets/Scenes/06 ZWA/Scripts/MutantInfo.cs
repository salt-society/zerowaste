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
    public GameObject mutantInfoPanel;

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
            GetComponent<TextMeshProUGUI>().text = mutant.plModifier.ToString() + " Per Level Up";

        // MAX PL

        // ATK
        mutantInfoPanel.transform.GetChild(4).GetChild(0).GetChild(5).GetComponent<TextMeshProUGUI>().text = mutant.baseAtk.ToString();
        mutantInfoPanel.transform.GetChild(4).GetChild(0).GetChild(5).GetChild(0).
            GetComponent<TextMeshProUGUI>().text = mutant.atkModifier.ToString() + " Per Level Up";

        // DEF
        mutantInfoPanel.transform.GetChild(4).GetChild(0).GetChild(8).GetComponent<TextMeshProUGUI>().text = mutant.baseDef.ToString();
        mutantInfoPanel.transform.GetChild(4).GetChild(0).GetChild(8).GetChild(0).
            GetComponent<TextMeshProUGUI>().text = mutant.defModifier.ToString() + " Per Level Up";

        // SPD
        mutantInfoPanel.transform.GetChild(4).GetChild(0).GetChild(11).GetComponent<TextMeshProUGUI>().text = mutant.baseSpd.ToString();
        mutantInfoPanel.transform.GetChild(4).GetChild(0).GetChild(11).GetChild(0).gameObject.SetActive(false);

        // Skill 1
        mutantInfoPanel.transform.GetChild(4).GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = mutant.abilities[1].abilityName;
        mutantInfoPanel.transform.GetChild(4).GetChild(1).GetChild(2).GetChild(0).
            GetComponent<TextMeshProUGUI>().text = mutant.abilities[1].abilityName;

        // Skill 2
        mutantInfoPanel.transform.GetChild(4).GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>().text = mutant.abilities[1].abilityName;
        mutantInfoPanel.transform.GetChild(4).GetChild(1).GetChild(3).GetChild(0).
            GetComponent<TextMeshProUGUI>().text = mutant.abilities[1].abilityName;

        // Ultimate
        mutantInfoPanel.transform.GetChild(4).GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>().text = mutant.abilities[1].abilityName;
        mutantInfoPanel.transform.GetChild(4).GetChild(1).GetChild(4).GetChild(0).
            GetComponent<TextMeshProUGUI>().text = mutant.abilities[1].abilityName;

    }

    public void ShowMutantInfo()
    {
        SetMutantInfo();
        mutantInfoPanel.SetActive(!mutantInfoPanel.activeInHierarchy);
    }
}
