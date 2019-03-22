using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private CameraManager cameraManager;
    private StatusManager statusManager;
    private ParticleManager particleManager;

    void Start()
    {
        statusManager = FindObjectOfType<StatusManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        particleManager = FindObjectOfType<ParticleManager>();
    }

    // <summary>
    // </summary>
    public IEnumerator AttackMutant(GameObject attacker, GameObject target, int abilityIndex, 
        int damagePoints, string abilityRange)
    {
        // Shake camera to create an illusion of motion
        cameraManager.Shake(true);

        // Decrement PL
        StartCoroutine(statusManager.DecrementPollutionBar(damagePoints));

        // Show scavenger's attack animation base on ability
        attacker.GetComponent<CharacterMonitor>().CharacterAbilityAnimation(abilityIndex, abilityRange);

        // Show mutant's damage animation
        target.GetComponent<CharacterMonitor>().CharacterStatusChangeAnimation(0);

        // Show damage points/damage taken by mutant
        // Takes in the damage points in string format and gameobject of mutant to
        // know where damage points should be instantiated
        StartCoroutine(statusManager.ShowDamagePoints(damagePoints.ToString(), target));
        
        yield return new WaitForSeconds(1f);

        cameraManager.Shake(false);
    }

    public IEnumerator AttackAllMutants(GameObject attacker, GameObject[] targets, int abilityIndex, 
        int damagePoints, string abilityRange)
    {
        // Shake camera to create an illusion of motion
        cameraManager.Shake(true);

        // Decrement PL
        StartCoroutine(statusManager.DecrementPollutionBar(damagePoints));

        // Show scavenger's attack animation base on ability
        attacker.GetComponent<CharacterMonitor>().CharacterAbilityAnimation(abilityIndex, abilityRange);

        // Show each mutant's damage animation
        foreach (GameObject target in targets)
        {
            target.GetComponent<CharacterMonitor>().CharacterStatusChangeAnimation(0);
        }

        // Show damage points/damage taken by mutant
        // Takes in the damage points in string format and gameobject of mutant to
        // know where damage points should be instantiated
        StartCoroutine(statusManager.ShowTotalPoints(damagePoints.ToString(), "Offensive", 0));

        yield return new WaitForSeconds(1f);

        cameraManager.Shake(false);
    }

    public IEnumerator HealScavenger(GameObject target, int healPoints, int abilityIndex, string abilityRange)
    {
        // Increment HP
        StartCoroutine(statusManager.IncrementHealth(target.GetComponent<CharacterMonitor>().CurrentHealth, 
            target.GetComponent<CharacterMonitor>().MaxHealth, target.GetComponent<CharacterMonitor>().Position));

        // Show scavenger's heal animation
        target.GetComponent<CharacterMonitor>().CharacterAbilityAnimation(abilityIndex, abilityRange);

        // Show heal points taken
        StartCoroutine(statusManager.ShowHealPoints(healPoints.ToString(), target));
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator HealAllScavengers(GameObject[] targets, int healPoints, int abilityIndex, string abilityRange)
    {
        foreach (GameObject target in targets)
        {
            // Increment HP
            StartCoroutine(statusManager.IncrementHealth(target.GetComponent<CharacterMonitor>().CurrentHealth,
            target.GetComponent<CharacterMonitor>().MaxHealth, target.GetComponent<CharacterMonitor>().Position));

            // Show scavenger's heal animation
            target.GetComponent<CharacterMonitor>().CharacterAbilityAnimation(abilityIndex, abilityRange);
        }
        
        yield return new WaitForSeconds(1f);

        // Show heal points taken
        StartCoroutine(statusManager.ShowTotalPoints(healPoints.ToString(), "Defensive", 1));
    }

    public IEnumerator ChargeAntidote(GameObject target, int abilityIndex, string abilityRange)
    {
        // Increment HP
        StartCoroutine(statusManager.IncrementAntidote(target.GetComponent<CharacterMonitor>().CurrentAnt,
            target.GetComponent<CharacterMonitor>().MaxAnt, target.GetComponent<CharacterMonitor>().Position));

        // Show scavenger's ant gen animation
        target.GetComponent<CharacterMonitor>().CharacterAbilityAnimation(abilityIndex, abilityRange);
        yield return new WaitForSeconds(1.6f);
        StartCoroutine(particleManager.AntBurst(new Vector3(target.transform.position.x, 0, 0)));
    }

    public IEnumerator ChargeAllAntidote(GameObject[] targets, int abilityIndex, string abilityRange)
    { 
        foreach (GameObject target in targets)
        {
            // Increment HP
            StartCoroutine(statusManager.IncrementAntidote(target.GetComponent<CharacterMonitor>().CurrentAnt,
            target.GetComponent<CharacterMonitor>().MaxAnt, target.GetComponent<CharacterMonitor>().Position));

            // Show scavenger's ant gen animation
            target.GetComponent<CharacterMonitor>().CharacterAbilityAnimation(abilityIndex, abilityRange);
        }

        yield return new WaitForSeconds(1.6f);
        StartCoroutine(particleManager.AntBurst(new Vector3(targets[1].transform.position.x, 0, 0)));
    }
}
