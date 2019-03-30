using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAbilityManager : MonoBehaviour
{
    private EnemyAbility ability;
    private Enemy mutant;
    private GameObject mutantObj;
    private int changeApplied;

    private GameObject threatPlayer;

    private BattleController battleController;
    private CharacterManager characterManager;
    private AnimationManager animationManager;
    private StatusManager statusManager;
    private ParticleManager particleManager;
    private TurnQueueManager turnQueueManager;

    void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();
        animationManager = FindObjectOfType<AnimationManager>();
        statusManager = FindObjectOfType<StatusManager>();
        particleManager = FindObjectOfType<ParticleManager>();
        battleController = FindObjectOfType<BattleController>();
        turnQueueManager = FindObjectOfType<TurnQueueManager>();
    }

    public void SetupEnemyAttack(Enemy mutant, GameObject mutantObj)
    {
        // Set current enemy to attack
        this.mutant = mutant;
        this.mutantObj = mutantObj;

        // Get all available abilities of mutant to attack
        List<EnemyAbility> availableAbilities = new List<EnemyAbility>();
        foreach (Ability ability in mutant.instanceAbilities)
        {
            EnemyAbility mutantAbility = ability as EnemyAbility;

            if (mutantAbility.turnTillActive == 0)
                availableAbilities.Add(mutantAbility);
        }

        // Decide what ability to use
        int chosenAbilityIndex = 0;

        // Decide which ability the enemy will use by taking into account its chance to proc,
        // and the enemy's current state

        // To make things fast, check first if there's only one in the list. If it is, just send that ability
        if (availableAbilities.Count == 1)
            chosenAbilityIndex = 0;

        // For multiple abilities
        else
        {
            // Holder for greatest chance
            int probableAbility = 0;

            foreach (EnemyAbility ability in availableAbilities)
            {
                // First add and subtract the chance to proc with 25% of it
                float minChance = (float)(ability.chanceToProc - (ability.chanceToProc * .25));
                float maxChance = (float)(ability.chanceToProc + (ability.chanceToProc * .25));

                // Randomize to check if the monster will do this ability
                float chance = Random.Range(minChance, maxChance + 1);

                // Add if the ability's state aligns with the enemy's current state
                if (ability.abilityState == mutant.currentState)
                    chance += 30;

                // If it's the most probable ability, pick that
                if (chance > probableAbility)
                    chosenAbilityIndex = availableAbilities.IndexOf(ability);
            }
        }

        this.ability = availableAbilities[chosenAbilityIndex];

        // Cool down chosen ability so mutants wouldn't be able to spam it
        /// this.ability.turnTillActive = this.ability.cooldown;

        ExecuteAbility();
    }

    void ExecuteAbility()
    {
        if (ability.type.Equals("Offensive"))
        {
            StartCoroutine(MutantToScavenger());
        }
        else
        {
            StartCoroutine(MutantToMutant());
        }
    }

    GameObject SelectTargetScavenger()
    {
        List<GameObject> aliveScavengers = new List<GameObject>();

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

    GameObject SelectTargetMutant()
    {
        List<GameObject> aliveMutants = new List<GameObject>();

        foreach (GameObject mutantObject in characterManager.GetAllCharacterPrefabs(0))
        {
            if (mutantObject.GetComponent<CharacterMonitor>().IsAlive)
                aliveMutants.Add(mutantObject);
        }

        int mutantIndex = Random.Range(0, aliveMutants.Count);

        return aliveMutants[mutantIndex];
    }

    IEnumerator MutantToScavenger()
    {
        yield return new WaitForSeconds(0.5f);

        changeApplied = 0;

        if (ability.range.Equals("Single"))
        {
            GameObject targetObj = SelectTargetScavenger();
            StartCoroutine(ToScavenger(targetObj));
            yield return new WaitForSeconds(4f);

            if (!targetObj.GetComponent<CharacterMonitor>().IsAlive)
                yield return new WaitForSeconds(1f);
        }
        else if (ability.range.Equals("AOE"))
        {
            List<GameObject> aliveScavengers = new List<GameObject>();

            foreach (GameObject scavengerObject in characterManager.GetAllCharacterPrefabs(1))
            {
                if (scavengerObject.GetComponent<CharacterMonitor>().IsAlive)
                    aliveScavengers.Add(scavengerObject);
            }

            foreach (GameObject targetObj in aliveScavengers)
            {
                StartCoroutine(ToScavenger(targetObj));

                if (ability.repeatAnimation)
                    yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(3f);
        }

        yield return null;
        turnQueueManager.FinishedTurn(mutant);
        StartCoroutine(EndTurn());
    }

    IEnumerator ToScavenger(GameObject targetObj)
    {
        animationManager.PlayAnimation(ability, mutantObj, targetObj, mutant.characterType);

        yield return new WaitForSeconds(ability.length);

        foreach (Effect effect in ability.effects)
        {
            if (effect.type.Equals("Direct"))
            {
                int valueChanged = targetObj.GetComponent<CharacterMonitor>().ScavengerDamaged(effect.target, effect.strength, mutant);
                changeApplied += valueChanged;

                if (effect.target.Equals("HP"))
                {
                    CharacterMonitor scavMonitor = targetObj.GetComponent<CharacterMonitor>();
                    StartCoroutine(statusManager.DecrementHealthBar(scavMonitor.CurrentHealth, scavMonitor.MaxHealth, scavMonitor.Position));
                    StartCoroutine(statusManager.ShowValues(valueChanged.ToString(), ability.type, targetObj, ability.particleIndex));
                    yield return new WaitForSeconds(1f);
                }

                if (effect.target.Equals("ANT"))
                {
                    CharacterMonitor scavMonitor = targetObj.GetComponent<CharacterMonitor>();
                    StartCoroutine(statusManager.DecrementAntidoteBar(scavMonitor.CurrentAnt, scavMonitor.MaxAnt, scavMonitor.Position));
                    StartCoroutine(particleManager.PlayParticles(effect.particleIndex, targetObj.transform.position));
                    StartCoroutine(statusManager.ShowValues(valueChanged.ToString(), ability.type, targetObj, ability.particleIndex));

                    yield return new WaitForSeconds(1f);
                }
            }

            if (effect.type.Equals("Status"))
            {
                targetObj.GetComponent<CharacterMonitor>().ScavengerDebuffed(effect);

                if (effect.application.Equals("CharStats"))
                {
                    targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(4, effect);
                    StartCoroutine(statusManager.ShowBuff(targetObj, effect.state));
                    yield return new WaitForSeconds(effect.animationLength);
                }

                if (effect.application.Equals("Condition"))
                {
                    targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(4, effect);
                    StartCoroutine(particleManager.PlayParticles(effect.particleIndex, targetObj.transform.position));
                    yield return new WaitForSeconds(effect.animationLength);
                }

                statusManager.AddEffectToStatusPanel("Scavenger", targetObj.GetComponent<CharacterMonitor>().Position, effect);
                statusManager.AddEffectsToStatusList("Scavenger", targetObj.GetComponent<CharacterMonitor>().Position, effect, ability.icon);
                yield return new WaitForSeconds(effect.animationLength);

            }
        }
    }

    IEnumerator MutantToMutant()
    {
        yield return new WaitForSeconds(0.5f);

        changeApplied = 0;

        if (ability.range.Equals("Self") || ability.range.Equals("Single"))
        {
            GameObject targetObj = (ability.range.Equals("Self")) ? mutantObj : SelectTargetMutant();
            StartCoroutine(ToMutant(targetObj));
            yield return new WaitForSeconds(3f);
        }
        else if (ability.range.Equals("AOE"))
        {
            List<GameObject> aliveMutants = new List<GameObject>();

            foreach (GameObject mutantObject in characterManager.GetAllCharacterPrefabs(0))
            {
                if (mutantObject.GetComponent<CharacterMonitor>().IsAlive)
                    aliveMutants.Add(mutantObject);
            }

            foreach (GameObject targetObj in aliveMutants)
            {
                StartCoroutine(ToMutant(targetObj));

                if (ability.repeatAnimation)
                    yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(3f);
        }

        yield return null;
        turnQueueManager.FinishedTurn(mutant);
        StartCoroutine(EndTurn());
    }

    IEnumerator ToMutant(GameObject targetObj)
    {
        animationManager.PlayAnimation(ability, mutantObj, targetObj, mutant.characterType);

        yield return new WaitForSeconds(ability.length);

        foreach (Effect effect in ability.effects)
        {
            if (effect.type.Equals("Direct"))
            {
                int valueChanged = targetObj.GetComponent<CharacterMonitor>().MutantHealed(effect.strength);
                changeApplied += valueChanged;

                StartCoroutine(statusManager.IncrementPollutionBar(valueChanged));
                StartCoroutine(statusManager.ShowValues(valueChanged.ToString(), ability.type, targetObj, ability.particleIndex));
                yield return new WaitForSeconds(1f);
            }

            if (effect.type.Equals("Status"))
            {
                targetObj.GetComponent<CharacterMonitor>().MutantBuffed(effect);

                if (effect.application.Equals("CharStats"))
                {
                    targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(4, effect);
                    yield return new WaitForSeconds(effect.animationLength);
                    StartCoroutine(statusManager.ShowBuff(targetObj, effect.state));
                    yield return new WaitForSeconds(1f);
                }

                if (effect.application.Equals("Condition"))
                {
                    targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(4, effect);
                    yield return new WaitForSeconds(effect.animationLength);
                    StartCoroutine(particleManager.PlayParticles(effect.particleIndex, targetObj.transform.position));
                    yield return new WaitForSeconds(1f);
                }
            }
        }
    }

    IEnumerator EndTurn()
    {
        yield return new WaitForSeconds(1f);
        battleController.NextTurn();
    }
}
