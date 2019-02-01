using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour {

    [Header("Sprites")]
    public GameObject[] scavengers;
    public GameObject[] mutants;

    [Header("Managers")]
    public CameraManager cameraManager;

    public void ChangeSprite(Player[] scavengers, Enemy[] mutants)
    {
        // TEMPORARY! DOESNT CHECK IF NULL
        for (int i = 0; i < scavengers.Length; i++)
        {
            this.scavengers[i].GetComponent<SpriteRenderer>().sprite = scavengers[i].characterImage;
            this.mutants[i].GetComponent<SpriteRenderer>().sprite = mutants[i].characterImage;
        }
    }

    public IEnumerator ScavengersEntrance()
    {
        cameraManager.ScavengerFocus(true);
        yield return new WaitForSeconds(1f);
        cameraManager.ScavengerFocus(false);

        foreach (GameObject scavenger in scavengers)
        {
            scavenger.SetActive(true);
            yield return new WaitForSeconds(.5f);
        }

        yield return null;
    }

    public IEnumerator MutantEntrance()
    {
        cameraManager.MutantFocus(true);
        yield return new WaitForSeconds(.5f);
        cameraManager.MutantFocus(false);

        foreach (GameObject mutant in mutants)
        {
            mutant.SetActive(true);
            yield return new WaitForSeconds(.5f);
        }

        cameraManager.BackToNormalCamera(true);
        yield return new WaitForSeconds(1f);
        cameraManager.BackToNormalCamera(false);

        yield return null;
    }
}
