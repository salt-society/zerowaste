using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQController : MonoBehaviour
{
    public DataController dataController;

    [Space]
    public List<GameObject> partHighlights;
    public List<GameObject> partNames;

    void Start()
    {
        StartCoroutine(DisplayTooltips());
    }

    IEnumerator DisplayTooltips()
    {
        foreach (GameObject tooltip in partNames)
            tooltip.SetActive(!tooltip.activeInHierarchy);

        yield return new WaitForSeconds(1.0f);

        foreach (GameObject tooltip in partNames)
            tooltip.GetComponent<Animator>().SetBool("Hide", true);

        yield return new WaitForSeconds(1.0f);

        foreach (GameObject tooltip in partNames)
        {
            tooltip.SetActive(!tooltip.activeInHierarchy);
            tooltip.GetComponent<Animator>().SetBool("Hide", true);
        }
    }
}
