using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour {

    [Header("Sprites")]
    public GameObject[] scavengers;
    public GameObject[] mutants;

    [Header("Managers")]
    public StatusManager statusManager;
    public CameraManager cameraManager;

    public Dictionary<string, Character[]> CloneCharacters(Player[] scavengers, Enemy[] mutants)
    {
        Dictionary<string, Character[]> characters = 
            new Dictionary<string, Character[]>();

        characters.Add("Scavenger", (Player[])scavengers.Clone());
        characters.Add("Mutants", (Enemy[])mutants.Clone());

        return characters;
    }

    public Player[] InitializeScavengers(Player[] scavengers)
    {
        foreach (Player scavenger in scavengers)
            scavenger.OnInitialize();

        return scavengers;
    }

    public Enemy[] InitializeMutants(Enemy[] mutants)
    {
        foreach (Enemy mutant in mutants)
            mutant.OnInitialize();

        return mutants;
    }

    public void ChangeSprite(Player[] scavengers, Enemy[] mutants)
    {
        int i = 0;
        foreach (Player scavenger in scavengers)
        {
            this.scavengers[i].GetComponent<SpriteRenderer>().sprite = scavengers[i].characterFull;
            i++;
        }

        int j = 0;
        foreach (Enemy mutant in mutants)
        {
            this.mutants[j].GetComponent<SpriteRenderer>().sprite = mutants[j].characterFull;
            j++;
        }
    }


    public IEnumerator ScavengersEntrance()
    {
        cameraManager.ScavengerFocus(1);
        yield return new WaitForSeconds(1f);
        cameraManager.ScavengerFocus(0);

        foreach (GameObject scavenger in scavengers)
        {
            scavenger.SetActive(true);
            yield return new WaitForSeconds(.5f);
        }

        yield return null;
    }

    public IEnumerator MutantEntrance()
    {
        cameraManager.MutantFocus(1);
        yield return new WaitForSeconds(.5f);
        cameraManager.MutantFocus(0);

        foreach (GameObject mutant in mutants)
        {
            mutant.SetActive(true);
            yield return new WaitForSeconds(.5f);
        }

        cameraManager.BackToNormalCamera(1);
        yield return new WaitForSeconds(1f);
        cameraManager.BackToNormalCamera(0);

        yield return null;
    }
}
