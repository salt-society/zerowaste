using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenController : BaseSceneController {

    [Space]
    [Header("Unique Values")]
    [SerializeField]
    private MessageController messageController;

    protected override void Update()
    {
        // functions common to all scenes
        base.Update();

        // functions unique to this scene
        // if shake animation is finished
        if (animationEvents[0].finished)
        {
            // display dev name
            if (!messageController.getStartedValue())
            {
                messageController.DisplayMessage(0);
                messageController.setStartedValue(true);
            }
            
            // load next scene
            if (messageController.getFinishedValue())
                StartCoroutine(base.LoadNextScene());
        }
    }
}
