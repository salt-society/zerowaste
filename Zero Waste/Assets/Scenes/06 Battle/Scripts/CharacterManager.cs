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
    public BattleInfoManager battleInfoManager;

    private GameObject[] scavengerPrefabs;
    private GameObject[] mutantPrefabs;

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
        {
            if(scavengers[i] != null)
                instantiatedScavengers[i] = Instantiate(scavengers[i]);
        }
            
        return instantiatedScavengers;
    }

    public Enemy[] InstantiateCharacterData(Enemy[] mutants)
    {

        Enemy[] instantiatedMutants = new Enemy[mutants.Length];
        for (int i = 0; i < mutants.Length; i++)
        {
            if(mutants[i] != null)
                instantiatedMutants[i] = Instantiate(mutants[i]);
        }

        return instantiatedMutants;
    }

    public Player[] InitializeScavengers(Player[] scavengers)
    {
        foreach (Player scavenger in scavengers)
        {
            if(scavenger != null)
                scavenger.OnInitialize();
        }
            
        return scavengers;
    }

    public Enemy[] InitializeMutants(Enemy[] mutants)
    {
        foreach (Enemy mutant in mutants)
        {
            if(mutant != null)
                mutant.OnInitialize();
        }
            
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
        scavengerPrefabs = new GameObject[scavengers.Length];

        int i = 0;
        foreach (Player scavenger in scavengers)
        {
            if (scavenger != null)
            {
                GameObject scavengerObject = Instantiate(scavenger.prefab, this.scavengers[i].transform);
                scavengerObject.GetComponent<CharacterMonitor>().SetCharacter(scavenger);
                scavengerObject.GetComponent<CharacterMonitor>().SetPosition(i);
                scavengerObject.GetComponent<CharacterMonitor>().SetStatusManager(statusManager);
                scavengerObject.transform.localScale = scavenger.scale;

                scavengerPrefabs[i] = scavengerObject;
                i++;
            }
        }
    }

    public void InstantiateCharacterPrefab(Enemy[] mutants)
    {
        statusManager.SetPollutionLevelBarCount();
        mutantPrefabs = new GameObject[mutants.Length];

        int i = 0;
        foreach (Enemy mutant in mutants)
        {
            GameObject mutantObject = Instantiate(mutant.prefab, this.mutants[i].transform);
            mutantObject.GetComponent<CharacterMonitor>().SetCharacter(mutant);
            mutantObject.GetComponent<CharacterMonitor>().SetPosition(i);
            mutantObject.GetComponent<CharacterMonitor>().SetStatusManager(statusManager);
            mutantObject.transform.localScale = mutant.scale;

            float minX = mutantObject.GetComponent<BoxCollider2D>().bounds.min.x;
            float minY = mutantObject.GetComponent<BoxCollider2D>().bounds.min.y;
            float extentY = mutantObject.GetComponent<BoxCollider2D>().size.y * 3;
            statusManager.AddPollutionLevelBar(minX, 10, i);

            mutantPrefabs[i] = mutantObject;
            i++;
        }
    }

    public GameObject GetScavengerPrefab(Player scavengerData)
    {
        GameObject scavengerPrefab = new GameObject();

        foreach (GameObject scavengerObject in scavengerPrefabs)
        {
            int instanceId = scavengerObject.GetComponent<CharacterMonitor>().GetCharacterInstance(1);

            if (instanceId.Equals(scavengerData.GetInstanceID())) 
            {
                scavengerPrefab = scavengerObject;
                break;
            }
        }

        return scavengerPrefab;
    }

    public int GetScavengerPosition(Player scavengerData)
    {
        int position = 0;

        for (int i = 0; i < scavengerPrefabs.Length; i++)
        {
            int instanceId = scavengerPrefabs[i].GetComponent<CharacterMonitor>().GetCharacterInstance(1);

            if (instanceId.Equals(scavengerData.GetInstanceID()))
            {
                position = i;
                break;
            }
        }

        return position;
    }

    public GameObject GetMutantPrefab(Player mutantData)
    {
        GameObject mutantPrefab = new GameObject();

        foreach (GameObject mutantObject in mutantPrefabs)
        {
            int instanceId = mutantObject.GetComponent<CharacterMonitor>().GetCharacterInstance(0);

            if (instanceId.Equals(mutantData.GetInstanceID()))
            {
                mutantPrefab = mutantObject;
                break;
            }
        }

        return mutantPrefab;
    }

    public int GetMutantPosition(Enemy mutantData)
    {
        int position = 0;

        for (int i = 0; i < mutantPrefabs.Length; i++)
        {
            int instanceId = mutantPrefabs[i].GetComponent<CharacterMonitor>().GetCharacterInstance(0);

            if (instanceId.Equals(mutantData.GetInstanceID()))
            {
                position = i;
                break;
            }
        }

        return position;
    }

    public GameObject[] GetAllCharacterPrefabs(int characterType)
    {
        return (characterType > 0) ? scavengerPrefabs : mutantPrefabs;
    }

    public IEnumerator ScavengersEntrance()
    {
        /*cameraManager.ScavengerFocus(1);
        yield return new WaitForSeconds(1f);
        cameraManager.ScavengerFocus(0);*/

        foreach (GameObject scavenger in scavengers)
        {
            scavenger.SetActive(true);
            yield return new WaitForSeconds(.5f);
        }

        yield return null;
    }

    public IEnumerator MutantEntrance()
    {
        /*cameraManager.MutantFocus(1);
        yield return new WaitForSeconds(.5f);
        cameraManager.MutantFocus(0);*/

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
