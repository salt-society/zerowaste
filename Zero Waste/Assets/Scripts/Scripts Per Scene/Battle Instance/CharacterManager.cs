using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour {

    [Header("Character Holder")]
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

    public Player[] InstantiateCharacterData(Player[] scavengers)
    {
        
        Player[] instantiatedScavengers = new Player[scavengers.Length];      
        for (int i = 0; i < scavengers.Length; i++)
            instantiatedScavengers[i] = Instantiate(scavengers[i]);


        return instantiatedScavengers;
    }

    public Enemy[] InstantiateCharacterData(Enemy[] mutants)
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

    public void ChangeSprite(Player[] scavengers)
    {
        int i = 0;
        foreach (Player scavenger in scavengers)
        {
            this.scavengers[i].GetComponent<SpriteRenderer>().sprite = scavenger.characterFull;
            i++;
        }
    }

    public void ChangeSprite(Enemy[] mutants)
    {
        int i = 0;
        foreach (Enemy mutant in mutants)
        {
            this.mutants[i].GetComponent<SpriteRenderer>().sprite = mutant.characterFull;
            i++;
        }
    }

    public void InstantiateCharacterPrefab(Player[] scavengers) 
    {
        int i = 0;
        foreach (Player scavenger in scavengers)
        {
            GameObject mutantObject = Instantiate(scavenger.prefab, this.scavengers[i].transform);
            mutantObject.transform.localScale = scavenger.scale;
            i++;
        }
    }

    public void InstantiateCharacterPrefab(Enemy[] mutants)
    {
        statusManager.SetPollutionLevelBarCount();

        int i = 0;
        foreach (Enemy mutant in mutants)
        {
            GameObject mutantObject = Instantiate(mutant.prefab, this.mutants[i].transform);
            mutantObject.transform.localScale = mutant.scale;

            float minX = mutantObject.GetComponent<BoxCollider2D>().bounds.min.x;
            float minY = mutantObject.GetComponent<BoxCollider2D>().bounds.min.y;
            float extentY = mutantObject.GetComponent<BoxCollider2D>().size.y * 3;
            statusManager.AddPollutionLevelBar(minX, minY, extentY, i);

            i++;
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
