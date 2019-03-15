using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    [SerializeField] private Camera particleFxCam;
    [SerializeField] private Camera mainCam;

    [Space]
    public Material[] imageEffectMaterials;

    private Animator mainCamAnimator;
    private Animator particleFxCamAnimator;

    void Start()
    {
        // Get animators of both cam
        mainCamAnimator = mainCam.GetComponent<Animator>();
        // particleFxCamAnimator = particleFxCam.GetComponent<Animator>();
    }

    // <summary>
    // Functions to trigger Camera animation and effects
    // </summary>
    public void Shake(bool shake)
    {
        mainCamAnimator.SetBool("Shake", shake);
    }

    public void FocusOnScavengers(bool focus)
    {
        mainCamAnimator.SetBool("Scavenger Focus", focus);
    }

    public void FocusOnMutants(bool focus)
    {
        mainCamAnimator.SetBool("Mutant Focus", focus);
    }

    
    
}
