using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour {

    [Header("Character Holder")]
    public GameObject scavengerGroup;
    public GameObject[] scavengers;
    public GameObject mutantGroup;
    public GameObject[] mutants;

    

    [Header("Managers")]
    public StatusManager statusManager;
    public CameraManager cameraManager;
    public BattleInfoManager battleInfoManager;

    private GameObject[] scavengerPrefabs;
    private GameObject[] mutantPrefabs;

    #region Properties
    private bool allScavengersAlive;

    public bool AllScavengersAlive
    {
        get { return allScavengersAlive; }
        set { allScavengersAlive = value; }
    }

    private bool allMutantsAlive;

    public bool AllMutantsAlive
    {
        get { return allMutantsAlive; }
        set { allMutantsAlive = value; }
    }
    #endregion

    private CharacterManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    void Start()
    {
        allScavengersAlive = true;
        allMutantsAlive = true;
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
        Player[] initScavengers = new Player[scavengers.Length];

        int i = 0;
        foreach (Player scavenger in scavengers)
        {
            if (scavenger != null)
            {
                scavenger.OnInitialize();
                initScavengers[i] = scavenger;
            }

            i++;
        }
            
        return initScavengers;
    }

    public Enemy[] InitializeMutants(Enemy[] mutants)
    {
        Enemy[] initMutants = new Enemy[mutants.Length];

        int i = 0;
        foreach (Enemy mutant in mutants)
        {
            if (mutant != null)
            {
                mutant.OnInitialize();
                initMutants[i] = mutant;
            }

            i++;
        }
            
        return initMutants;
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
        GameObject mutantPrefab = null;

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

    // <summary>
    // Gets character section, gameObject that holds each character type
    // </summary>
    public GameObject GetCharacterSection(int characterType)
    {
        return (characterType > 0) ? scavengerGroup : mutantGroup;
    }

    public IEnumerator ScavengersEntrance()
    {
        foreach (GameObject scavenger in scavengers)
        {
            scavenger.SetActive(true);
            yield return new WaitForSeconds(.5f);
        }

        yield return null;
    }

    public IEnumerator MutantEntrance()
    {
        foreach (GameObject mutant in mutants)
        {
            mutant.SetActive(true);
            yield return new WaitForSeconds(.5f);
        }

        yield return null;
    }

    IEnumerator CheckIfCharactersAreAlive(int targetCharacter) 
    {
        // Get all prefabs of certain character type
        GameObject[] characterPrefabs = (targetCharacter == 0) ? mutantPrefabs : scavengerPrefabs;

        // Loop throught the prefabs, get character monitor, and check if character is alive
        foreach (GameObject characterObject in characterPrefabs)
        {
            // If a character is alive, break loop
            // This means that battle must go on as both scavengers and mutants
            // still has alive characters
            if (characterObject.GetComponent<CharacterMonitor>().IsAlive)
            {
                if (targetCharacter == 1)
                    allScavengersAlive = true;

                if (targetCharacter == 0)
                    allMutantsAlive = true;

                break;
            }
            // This condition is reached if a character is dead
            else
            {
                // This condition won't be reached if loop haven't gone through
                // all prefabs, which just means all characters in a team is dead
                if (characterObject.GetInstanceID() == 
                    characterPrefabs[characterPrefabs.Length - 1].GetInstanceID())
                {
                    if (targetCharacter == 1)
                        allScavengersAlive = false;

                    if (targetCharacter == 0)
                        allMutantsAlive = false;
                }
            }
        }

        yield return null;
    }

    public void CheckStatusEffects()
    {
        foreach (GameObject scavengerPrefab in scavengerPrefabs)
        {
            scavengerPrefab.GetComponent<CharacterMonitor>().EndOfLoop = true;
        }

        foreach (GameObject mutantPrefab in mutantPrefabs)
        {
            mutantPrefab.GetComponent<CharacterMonitor>().EndOfLoop = true;
        }
    }

    void Update()
    {
        // Constantly check lives of characters per team
        // If all characters in a team, Scavenger or Mutant, are dead, battle should end
        StartCoroutine(CheckIfCharactersAreAlive(0));
        StartCoroutine(CheckIfCharactersAreAlive(1));
    }
}
