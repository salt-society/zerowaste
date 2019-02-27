using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAutoResize : MonoBehaviour
{
    public bool runInEditor;

    [Space]
    public Vector2 baseScreenResolution;
    public float baseSpriteLocalScale;

    [Space]
    public float scaleMultiplier;
    public float baseScreenRatio;

    [Space]
    public bool hasGround;
    public float spriteBasePositionY;
    public float yPositionMultiplier;

    void Awake()
    {
        baseScreenRatio = baseScreenResolution.x / baseScreenResolution.y;
        scaleMultiplier = baseSpriteLocalScale / baseScreenRatio;

        yPositionMultiplier = baseSpriteLocalScale / spriteBasePositionY;
    }

    void Start()
    {
        SpriteResize();
    }

    void Update()
    {
        if (runInEditor)
        {
            SpriteResize();
        }
    }

    void SpriteResize()
    {
        float screenRatio = (float)Screen.width / (float)Screen.height;

        if (screenRatio > baseScreenRatio)
        {
            float diffInRatio = screenRatio - baseScreenRatio;
            float spriteSizePlus = (diffInRatio * scaleMultiplier);

            Vector3 spriteNewScale = new Vector3(baseSpriteLocalScale + spriteSizePlus, baseSpriteLocalScale + spriteSizePlus);

            transform.localScale = spriteNewScale;

            if (hasGround)
            {
                float diffInScale = spriteNewScale.x - baseSpriteLocalScale;
                float changeInYPosition = diffInScale * yPositionMultiplier;

                transform.localPosition = new Vector3(transform.position.x, spriteBasePositionY + changeInYPosition);
            }
        }
        else
        {
            Vector3 spriteNewScale = new Vector3(baseSpriteLocalScale, baseSpriteLocalScale);
            transform.localScale = spriteNewScale;
            Debug.Log("Sprite Scale changed to: " + spriteNewScale);

            if (hasGround)
            {
                transform.localPosition = new Vector3(transform.position.x, spriteBasePositionY);
            }
        }
    }
}
