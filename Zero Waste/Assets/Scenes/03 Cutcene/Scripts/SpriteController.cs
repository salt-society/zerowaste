using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteController : MonoBehaviour
{
    public GameObject[] characterSprites;
    private List<Sprite> previousSprites;

    public void ShowSprites(Sprite[] sprites)
    {
        if (sprites.Length == 1)
        {
            if (previousSprites[0] == null)
            {
                characterSprites[0].GetComponent<Image>().sprite = sprites[0];

                if (!characterSprites[0].activeInHierarchy)
                {
                    characterSprites[2].SetActive(false);
                    characterSprites[1].SetActive(false);

                    characterSprites[0].SetActive(true);
                }  
            } 
        }

        if (sprites.Length == 2)
        {

        }

        if (sprites.Length == 3)
        {
            int i = 0;
            foreach (GameObject spriteHolder in characterSprites)
            {
                spriteHolder.GetComponent<Image>().sprite = sprites[i];
                spriteHolder.SetActive(true);
                i++;
            }
        }
    }
}
