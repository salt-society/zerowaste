using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    [Header("Cameras")]
    public Camera mainCamera;
    public Camera uiCamera;
    public Camera fxCamera;

    public void ScavengerFocus(int focus)
    {
        bool focusCam = (focus > 0) ? true : false;
        mainCamera.GetComponent<Animator>().SetBool("Scavenger Focus", focusCam);
    }

    public void MutantFocus(int focus)
    {
        bool focusCam = (focus > 0) ? true : false;
        mainCamera.GetComponent<Animator>().SetBool("Mutant Focus", focusCam);
    }

    public void BackToNormalCamera(int focus)
    {
        bool focusCam = (focus > 0) ? true : false;
        mainCamera.GetComponent<Animator>().SetBool("Normal Camera", focusCam);
    }
    
}
