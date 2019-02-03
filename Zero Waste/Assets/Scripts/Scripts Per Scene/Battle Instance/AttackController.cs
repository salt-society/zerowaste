using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackController : MonoBehaviour
{
    [Header("Attack Panel")]
    public GameObject attackButtons;

    public void DisplayAttackButtons(int visibility)
    {
        bool showComponent = (visibility > 0) ? true : false;
        attackButtons.SetActive(showComponent);
    }
}
