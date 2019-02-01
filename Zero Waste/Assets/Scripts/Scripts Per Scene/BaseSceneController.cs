using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class BaseSceneController : MonoBehaviour {

    // common values to all scene controllers
    [Header("Base Values")]
    public int delay;
    public int sceneToLoad;
    [Space]
    [Header("Scene Transition")]
    public Animator fadeAnimator;
    public string fadeOutTrigger;
    public bool isFadeEnabled;
    [Space]
    [Header("Animations")]
    public AnimationEvent[] animationEvents;
    public bool[] enableAnimation;

    [HideInInspector] // hidden fields
    public int tempdelay;
    [HideInInspector]
    public bool isSceneSkipped;

    protected virtual void Start()
    {
        tempdelay = delay;
    }

    protected virtual void Update()
    {
        // does nothing if scene is skipped
        if (isSceneSkipped)
            return;
    }

    // for loading next scene
    protected virtual IEnumerator LoadNextScene()
    {
        // delay before loading next scene
        yield return new WaitForSeconds(delay);

        // check if there's a scene to load
        if (sceneToLoad.Equals(0))
            yield return null;
        else
        {
            // fade out animation
            if (isFadeEnabled)
            {
                fadeAnimator.SetTrigger(fadeOutTrigger);
            }
            
            // load next scene
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
