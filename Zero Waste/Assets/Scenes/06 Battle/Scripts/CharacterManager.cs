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

    private CharacterManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

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

    // <summary>
    // Instantiate each scavenger's prefab on respective positions.
    // </summary>
    public void InstantiateCharacterPrefab(Player[] scavengers) 
    {
        scavengerPrefabs = new GameObject[scavengers.Length];
        GameObject scavengerObject;

        int i = 0;
        foreach (Player scavenger in scavengers)
        {
            // For extra caution, check if scavenger is not null before performing function
            if (scavenger != null)
            {
                // Instantiate
                scavengerObject = Instantiate(scavenger.prefab, this.scavengers[i].transform);
                scavengerObject.GetComponent<CharacterMonitor>().InitializeMonitor();
                scavengerObject.GetComponent<CharacterMonitor>().Scavenger = scavenger;
                scavengerObject.GetComponent<CharacterMonitor>().Position = i;
                scavengerObject.GetComponent<CharacterMonitor>().SetCharacter();
                scavengerObject.transform.localScale = scavenger.scale;

                // Store scavenger prefabs for later access
                scavengerPrefabs[i] = scavengerObject;
                i++;
            }
        }
    }

    public void InstantiateCharacterPrefab(Enemy[] mutants)
    {
        mutantPrefabs = new GameObject[mutants.Length];
        GameObject mutantObject;

        int i = 0;
        foreach (Enemy mutant in mutants)
        {
            // Instantiate
            mutantObject = Instantiate(mutant.prefab, this.mutants[i].transform);
            mutantObject.GetComponent<CharacterMonitor>().InitializeMonitor();
            mutantObject.GetComponent<CharacterMonitor>().Mutant = mutant;
            mutantObject.GetComponent<CharacterMonitor>().Position = i;
            mutantObject.GetComponent<CharacterMonitor>().SetCharacter();
            mutantObject.transform.localScale = mutant.scale;

            // Store mutant prefab for later access
            mutantPrefabs[i] = mutantObject;
            i++;
        }
    }

    // <summary>
    // Gets scavenger prefab, existing on battle, through scavenger data
    // </summary>
    public GameObject GetScavengerPrefab(Player scavengerData)
    {
        GameObject scavengerPrefab = null;

        // Loop through the prefabs list of scavengers
        foreach (GameObject scavengerObject in scavengerPrefabs)
        {
            // Get scavenger data's instance id
            int instanceId = scavengerObject.GetComponent<CharacterMonitor>().InstanceId;

            // Check if it matches
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
            int instanceId = scavengerPrefabs[i].GetComponent<CharacterMonitor>().InstanceId;

            if (instanceId.Equals(scavengerData.GetInstanceID()))
            {
                position = scavengerPrefabs[i].GetComponent<CharacterMonitor>().Position;
                break;
            }
        }

        return position;
    }

    public GameObject GetMutantPrefab(Enemy mutantData)
    {
        GameObject mutantPrefab = new GameObject();

        foreach (GameObject mutantObject in mutantPrefabs)
        {
            int instanceId = mutantObject.GetComponent<CharacterMonitor>().InstanceId;

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
            int instanceId = mutantPrefabs[i].GetComponent<CharacterMonitor>().InstanceId;

            if (instanceId.Equals(mutantData.GetInstanceID()))
            {
                position = mutantPrefabs[i].GetComponent<CharacterMonitor>().Position;
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
