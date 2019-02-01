using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour {

    [Header("Common Values")]
    [Space]
    [Header("Particles")]
    [SerializeField]
    private ParticleSystem effectsOnTouch; // burst particles
    [SerializeField]
    [Space]
    [Header("Particle Camera")]
    private Camera effectsCam; // camera that renders particles
    [Space]
    [SerializeField]
    private BaseSceneController controller; // have control over scene
                                        // by touch inputs
    // hidden values
    private Touch touch; // holds touch values
   
    // called every frame
	void Update () {
        touch = new Touch(); // clear touch value
        StopParticles(); // stop particles

        // check if user is touching screen
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0); // get first touch

            // check touch phase
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        TouchEffect(); // call function to play particles
                        
                        Debug.Log("Phase: " + touch.phase); // debugging
                        break;
                    }
                case TouchPhase.Stationary:
                    {
                        TouchEffect(); // call function to play particles
                        Debug.Log("Phase: " + touch.phase); // debugging
                        break;
                    }
            }
        }
	}

    // particle effects for touch input
    private void TouchEffect()
    {
        Vector3 point = new Vector3(); // point where particles emit

        // to keep particles inside camera bounds
        point = effectsCam.ScreenToWorldPoint(new Vector3(touch.position.x, 
            touch.position.y, effectsCam.nearClipPlane));

        // place particle on touch position
        effectsOnTouch.transform.position = point;
        effectsOnTouch.Play(); // particle is burst

        Debug.Log("Touch Coordinates: " + touch.position); // debugging
    }

    // stops all particles emitted on touch
    private void StopParticles()
    {
        // stop touch effect
        if (effectsOnTouch.isEmitting || effectsOnTouch.isPlaying)
            effectsOnTouch.Stop();
    }
}
