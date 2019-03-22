using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAbilityManager : MonoBehaviour
{
    private EnemyAbility chosenAbility;
    private Enemy mutant;
    private GameObject threatPlayer;

    private CharacterManager characterManager;

    void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();
    }

    public void SetupEnemyAttack(Enemy mutant)
    {
        // Set current enemy to attack
        this.mutant = mutant;

        // Get all available abilities of mutant to attack
        List<EnemyAbility> availableAbilities = new List<EnemyAbility>();
        foreach (Ability ability in mutant.instanceAbilities)
        {
            EnemyAbility mutantAbility = ability as EnemyAbility;

            if (mutantAbility.turnTillActive == 0)
                availableAbilities.Add(mutantAbility);
        }

        // Decide what ability to use
        int chosenAbilityIndex = Random.Range(0, availableAbilities.Count);
        chosenAbility = availableAbilities[chosenAbilityIndex];

        // Cool down chosen ability so mutants wouldn't be able to spam it
        chosenAbility.turnTillActive = chosenAbility.cooldown;

        
    }

    void ExecuteAbility()
    {
        if (chosenAbility.abilityType.Equals("Offensive"))
        {
            
        }
        else
        {

        }
    }

    GameObject SelectTarget()
    {
        List<GameObject> aliveScavengers = new List<GameObject>();
        int scavCount = 0;

        foreach (GameObject scavengerObject in characterManager.GetAllCharacterPrefabs(1))
        {
            if (scavengerObject.GetComponent<CharacterMonitor>().IsAlive)
                aliveScavengers.Add(scavengerObject);
        }

        int totalThreatLevel = 0;
        int randomizedTarget = 0;
        int targetIndex = 0;

        List<GameObject> sortByThreat = new List<GameObject>();
        for (int i = 0; i < aliveScavengers.Count; i++)
        {
            int threatLevel = -1;
            foreach (GameObject scavengerObject in aliveScavengers)
            {
                Player scavengerData = scavengerObject.GetComponent<CharacterMonitor>().Scavenger;
                if (scavengerData.currentThreatLevel > threatLevel)
                {
                    threatPlayer = scavengerObject;
                    threatLevel = scavengerData.currentThreatLevel;
                }
            }

            aliveScavengers.Remove(threatPlayer);
            sortByThreat.Add(threatPlayer);

            totalThreatLevel += threatPlayer.GetComponent<CharacterMonitor>().Scavenger.currentThreatLevel;
        }

        for (int i = 0; i < sortByThreat.Count; i++)
        {
            randomizedTarget = Random.Range(1, totalThreatLevel + 1);

            if (randomizedTarget <= sortByThreat[i].
                GetComponent<CharacterMonitor>().Scavenger.currentThreatLevel)
            {
                targetIndex = i;
                break;
            }
            else
            {
                totalThreatLevel -= sortByThreat[i].
                    GetComponent<CharacterMonitor>().Scavenger.currentThreatLevel;
            }
        }

        return sortByThreat[targetIndex];
    }

    IEnumerator MutantToScavenger()
    {
        if (chosenAbility.abilityRange.Equals("Single"))
        {
            GameObject targetObject = SelectTarget();
            int totalDamage = 0;

            foreach (Effect effect in chosenAbility.effects)
            {
                if (effect.effectType.Equals("Direct"))
                {

                }
                else
                {
                    if (effect.effectApplication.Equals("Variable"))
                    {

                    }
                    else
                    {

                    }
                }
            }
        }
        else if (chosenAbility.abilityRange.Equals("AOE"))
        {

        }

        yield return null;
    }

    void MutantToMutant()
    {

    }
}
