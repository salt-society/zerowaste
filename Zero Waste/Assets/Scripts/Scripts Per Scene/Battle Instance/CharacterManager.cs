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

    public Player[] CloneCharacters(Player[] scavengers)
    {
        Player[] clonedScavengers = (Player[])scavengers.Clone();
        return clonedScavengers;
    }

    public Enemy[] CloneCharacters(Enemy[] mutants)
    {
        Enemy[] clonedMutants = (Enemy[])mutants.Clone();
        return clonedMutants;
    }

    public Player[] InstantiateCharacters(Player[] scavengers)
    {
        
        Player[] instantiatedScavengers = new Player[scavengers.Length];      
        for (int i = 0; i < scavengers.Length; i++)
            instantiatedScavengers[i] = Instantiate(scavengers[i]);


        return instantiatedScavengers;
    }

    public Enemy[] InstantiateCharacters(Enemy[] mutants)
    {

        Enemy[] instantiatedMutants = new Enemy[mutants.Length];
        for (int i = 0; i < mutants.Length; i++)
            instantiatedMutants[i] = Instantiate(mutants[i]);

        return instantiatedMutants;
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
