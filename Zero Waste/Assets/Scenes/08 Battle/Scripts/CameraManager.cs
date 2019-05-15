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

    void Awake()
    {
        // Get animators of both cam
        mainCamAnimator = mainCam.GetComponent<Animator>();
        // particleFxCamAnimator = particleFxCam.GetComponent<Animator>();
    }

    // <summary>
    // Functions to trigger Camera animation and effects
    // </summary>
    public void Shake(bool shake, int intensity)
    {
        switch (intensity)
        {
            case 1:
                {
                    mainCamAnimator.SetBool("Light Shake", shake);
                    break;
                }
            case 2:
                {
                    mainCamAnimator.SetBool("Shake", shake);
                    break;
                }
        }
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
