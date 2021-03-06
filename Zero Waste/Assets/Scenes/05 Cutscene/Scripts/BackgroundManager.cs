﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public GameObject overlayBox;
    public Sprite previousBackground;

    public void ChangeBackground(Sprite sprite)
    {
        Debug.Log("Set first background.");

        GetComponent<Image>().sprite = sprite;
    }
    
    public IEnumerator ChangeBackground(Sprite sprite, Color color)
    {
        Debug.Log("Background Transition: " + (previousBackground == sprite));

        previousBackground = GetComponent<Image>().sprite;

        if (sprite == null)
        {
            GetComponent<Image>().color = color;
            GetComponent<Image>().sprite = sprite;

            yield return new WaitForSeconds(0.5f);

            gameObject.SetActive(true);
            overlayBox.SetActive(true);
        }
        else if(!(previousBackground == sprite))
        {
            Debug.Log("Background State: " + !gameObject.activeInHierarchy);
            Debug.Log("Overlay State: " + !overlayBox.activeInHierarchy);

            if (!gameObject.activeInHierarchy && !overlayBox.activeInHierarchy)
            {
                GetComponent<Image>().color = color;
                GetComponent<Image>().sprite = sprite;

                yield return new WaitForSeconds(0.5f);

                gameObject.SetActive(true);
                overlayBox.SetActive(true);

                Debug.Log("Condition 1");
            }
            else
            {
                GetComponent<Animator>().SetBool("Fade Out", true);
                overlayBox.GetComponent<Animator>().SetBool("Fade Out", true);

                yield return new WaitForSeconds(0.5f);

                GetComponent<Image>().color = color;
                GetComponent<Image>().sprite = sprite;

                yield return new WaitForSeconds(0.5f);

                GetComponent<Animator>().SetBool("Fade Out", false);
                overlayBox.GetComponent<Animator>().SetBool("Fade Out", false);
                Debug.Log("Condition 2");
            }
        }
    }

    public bool CompareBackground(Sprite sprite)
    {
        return (previousBackground == sprite);
    }
}
