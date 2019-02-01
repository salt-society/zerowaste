using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    [Header("Cameras")]
    public Camera mainCamera;
    public Camera uiCamera;
    public Camera fxCamera;

    public void ScavengerFocus(bool isFocus)
    {
        mainCamera.GetComponent<Animator>().SetBool("Scavenger Focus", isFocus);
    }

    public void MutantFocus(bool isFocus)
    {
        mainCamera.GetComponent<Animator>().SetBool("Mutant Focus", isFocus);
    }

    public void BackToNormalCamera(bool isFocus)
    {
        mainCamera.GetComponent<Animator>().SetBool("Normal Camera", isFocus);
    }
    
}
