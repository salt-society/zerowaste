using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnknownMutant : MonoBehaviour
{
    public void SelectMutant() 
    {
        FindObjectOfType<AudioManager>().PlaySound("Beep Denied");
    }
}
