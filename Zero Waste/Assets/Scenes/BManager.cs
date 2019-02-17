using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BManager : MonoBehaviour
{
    public GameObject overlayBox;
    public Sprite previousBackground;

    public IEnumerator ChangeBackground(Sprite sprite, Color color)
    {
        Debug.Log((previousBackground == sprite));
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
            if (!gameObject.activeInHierarchy && !overlayBox.activeInHierarchy)
            {
                gameObject.SetActive(true);
                overlayBox.SetActive(true);

                yield return new WaitForSeconds(0.5f);
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
            }
        }
    }
}
