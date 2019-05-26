using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengerList : MonoBehaviour
{
    [Header("Editor Components")]
    public GameObject scrollview;
    public GameObject scavengerPrefab;

    #region Private Variables

    private DataController dataController;

    #endregion

    // Show Roster Screen and populate
    public void ShowRoster(int index)
    { 
        List<Player> roster = dataController.scavengerRoster;

        ClearRoster();

        for (int CTR = 0; CTR < roster.Count; CTR++)
        {
            GameObject instancePrefab = Instantiate(scavengerPrefab, scrollview.transform);
            instancePrefab.GetComponent<InstanceHandler>().SetPlayer(roster[CTR]);
        }
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
}
