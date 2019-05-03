using System;
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

    [Header("Battle Controller")]
    public GameObject battleController;

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

    public Enemy[] InitializeBoss(Enemy bossMutant)
    {
        Enemy[] initMutants = new Enemy[2];
        initMutants[0] = null;

        Boss bossMutantTemp = bossMutant as Boss;
        bossMutantTemp.OnInitialize();
        initMutants[1] = bossMutantTemp as Enemy;

        return initMutants;
    }

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
            if (mutant != null)
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
            }
            i++;
        }
    }

    public void ClearMutantPrefab(int mutantTeamLength)
    {
        // Basic safety check
        if(mutantPrefabs != null)
        {
            // Destroy all created prefabs in mutant prefabs
            foreach (GameObject prefab in mutantPrefabs)
                Destroy(prefab);
        }
    }

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
            if (mutantObject != null)
            {
                int instanceId = mutantObject.GetComponent<CharacterMonitor>().InstanceId;

                if (instanceId.Equals(mutantData.GetInstanceID()))
                {
                    mutantPrefab = mutantObject;
                    break;
                }
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
            // Make sure character object is not null before accessing
            // its components like character monitor
            if (characterObject != null)
            {
                // If a character is alive, break loop
                // This means that battle must go on as both scavengers and mutants
                // still has alive characters
                if (characterObject.GetComponent<CharacterMonitor>().IsAlive)
                {
                    if (targetCharacter == 1)
                        allScavengersAlive = true;

                    else if (targetCharacter == 0)
                        allMutantsAlive = true;

                    break;
                }
                // This condition is reached if a character is dead
                else
                {
                    // This condition won't be reached if loop haven't gone through
                    // all prefabs, which just means all characters in a team is dead
                    if(characterObject.GetInstanceID() == (characterPrefabs[characterPrefabs.Length - 1]).GetInstanceID())
                    {
                        Debug.Log("Got Here");
                        if (targetCharacter == 1)
                        {
                            battleController.GetComponent<BattleController>().CheckBattleEnd(targetCharacter);
                            allScavengersAlive = false;
                        }

                        else if (targetCharacter == 0)
                        {
                            battleController.GetComponent<BattleController>().CheckBattleEnd(targetCharacter);
                            allMutantsAlive = false;
                            Debug.Log("Hello");
                        }
                    }
                }
            }
        }

        yield return null;
    }

    void Update()
    {
        // Constantly check lives of characters per team
        // If all characters in a team, Scavenger or Mutant, are dead, battle should end
        if (allMutantsAlive && allScavengersAlive)
        {
            StartCoroutine(CheckIfCharactersAreAlive(0));
            StartCoroutine(CheckIfCharactersAreAlive(1));
        }
    }

    public void SuddenInvasion()
    {
        allMutantsAlive = true;
        Debug.Log("Got Here!");
    }
}
