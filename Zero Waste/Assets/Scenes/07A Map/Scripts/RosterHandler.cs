using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RosterHandler : MonoBehaviour
{

    #region Editor Variables

    [Header("UI Components")]
    public GameObject scrollview;

    #endregion

    #region Private Variables

    private int chosenSlot;

    #endregion

    // Show Roster Screen and populate
    public void ShowRoster(int index)
    {
        chosenSlot = index;
    }
}
