using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private CameraManager cameraManager;
    private StatusManager statusManager;

    void Start()
    {
        statusManager = FindObjectOfType<StatusManager>();
        cameraManager = FindObjectOfType<CameraManager>();
    }

    // <summary>
    // </summary>
    public IEnumerator AttackMutant(GameObject attacker, GameObject target, int abilityIndex, int damagePoints)
    {
        // Shake camera to create an illusion of motion
        cameraManager.Shake(true);

        // Decrement PL
        StartCoroutine(statusManager.DecrementPollutionBar(damagePoints));

        // Show scavenger's attack animation base on ability
        attacker.GetComponent<CharacterMonitor>().CharacterAbilityAnimation(abilityIndex);

        // Show mutant's damage animation
        target.GetComponent<CharacterMonitor>().CharacterStatusChangeAnimation(0);

        // Show damage points/damage taken by mutant
        // Takes in the damage points in string format and gameobject of mutant to
        // know where damage points should be instantiated
        StartCoroutine(statusManager.ShowDamagePoints(damagePoints.ToString(), target));
        
        yield return new WaitForSeconds(1f);

        cameraManager.Shake(false);
    }

    public IEnumerator HealScavenger(GameObject target, int healPoints, int abilityIndex)
    {
        // Increment HP
        StartCoroutine(statusManager.IncrementHealth(target.GetComponent<CharacterMonitor>().CurrentHealth, 
            target.GetComponent<CharacterMonitor>().MaxHealth, target.GetComponent<CharacterMonitor>().Position));

        // Show scavenger's heal animation
        target.GetComponent<CharacterMonitor>().CharacterAbilityAnimation(abilityIndex);

        // Show heal points taken
        StartCoroutine(statusManager.ShowHealPoints(healPoints.ToString(), target));
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator GenerateAntidote(GameObject target, int antGen, int abilityIndex)
    {
        // Increment HP
        StartCoroutine(statusManager.IncrementAntidote(target.GetComponent<CharacterMonitor>().CurrentAnt,
            target.GetComponent<CharacterMonitor>().MaxAnt, target.GetComponent<CharacterMonitor>().Position));

        // Show scavenger's heal animation
        target.GetComponent<CharacterMonitor>().CharacterAbilityAnimation(abilityIndex);

        // Show ant points taken
        StartCoroutine(statusManager.ShowHealPoints(antGen.ToString(), target));
        yield return new WaitForSeconds(1f);
    }
}
