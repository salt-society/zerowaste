using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScavengerList : MonoBehaviour
{
    [Header("Editor Components")]
    public GameObject scrollview;
    public GameObject scavengerPrefab;
    public GameObject detailedView;

    #region Private Variables

    private DataController dataController;

    #endregion

    // Show Roster Screen and populate
    public void ShowRoster()
    { 
        List<Player> roster = dataController.scavengerRoster;

        ClearRoster();

        for (int CTR = 0; CTR < roster.Count; CTR++)
        {
            GameObject instancePrefab = Instantiate(scavengerPrefab, scrollview.transform);
            instancePrefab.GetComponent<ListInstanceHandler>().SetPlayer(roster[CTR]);
        }
    }

    public void ShowDetailedScreen(Player heldPlayer)
    {
        detailedView.SetActive(true);

        detailedView.transform.GetChild(0).GetComponent<Image>().sprite = heldPlayer.characterFull;
        detailedView.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = heldPlayer.characterName;
        detailedView.transform.GetChild(2).gameObject.SetActive(false);

        // Stats
        detailedView.transform.GetChild(3).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = heldPlayer.baseHP.ToString();
        detailedView.transform.GetChild(3).GetChild(0).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "+ " + heldPlayer.hpModifier + " per level";

        detailedView.transform.GetChild(3).GetChild(0).GetChild(5).GetComponent<TextMeshProUGUI>().text = heldPlayer.baseAtk.ToString();
        detailedView.transform.GetChild(3).GetChild(0).GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = "+ " + heldPlayer.atkModifier + " per level";

        detailedView.transform.GetChild(3).GetChild(0).GetChild(8).GetComponent<TextMeshProUGUI>().text = heldPlayer.baseDef.ToString();
        detailedView.transform.GetChild(3).GetChild(0).GetChild(8).GetChild(0).GetComponent<TextMeshProUGUI>().text = "+ " + heldPlayer.defModifier + " per level";

        detailedView.transform.GetChild(3).GetChild(0).GetChild(11).GetComponent<TextMeshProUGUI>().text = heldPlayer.baseSpd.ToString();
        detailedView.transform.GetChild(3).GetChild(0).GetChild(11).GetChild(0).gameObject.SetActive(false);

        detailedView.transform.GetChild(3).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = heldPlayer.abilities[0].abilityName;
        detailedView.transform.GetChild(3).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = heldPlayer.abilities[0].abilityDescription;

        detailedView.transform.GetChild(3).GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = heldPlayer.abilities[1].abilityName;
        detailedView.transform.GetChild(3).GetChild(1).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = heldPlayer.abilities[1].abilityDescription;

        detailedView.transform.GetChild(3).GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>().text = heldPlayer.abilities[2].abilityName;
        detailedView.transform.GetChild(3).GetChild(1).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = heldPlayer.abilities[2].abilityDescription;

        detailedView.transform.GetChild(3).GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>().text = heldPlayer.abilities[3].abilityName;
        detailedView.transform.GetChild(3).GetChild(1).GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = heldPlayer.abilities[3].abilityDescription;
    }

    public void HideDetailedScreen()
    {
        detailedView.SetActive(false);
    }

    // Clear contents
    private void ClearRoster()
    {
        if (scrollview.transform.childCount > 0)
        {
            foreach (Transform child in scrollview.transform)
                Destroy(child.gameObject);
        }
    }

    private void Awake()
    {
        dataController = FindObjectOfType<DataController>();
    }
}
