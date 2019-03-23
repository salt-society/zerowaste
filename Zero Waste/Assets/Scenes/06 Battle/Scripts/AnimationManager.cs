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

    #region Shitty Code
    // <summary>
    // </summary>
    /*public IEnumerator AttackMutant(GameObject attacker, GameObject target, int abilityIndex, 
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
    }*/
    #endregion

    public void PlayAnimation(Ability ability, GameObject attackerObj, GameObject targetObj, string attackerType)
    {
        if (attackerType.Equals("Scavenger"))
        {
            switch (ability.index)
            {
                case 0:
                    {
                        StartCoroutine(AttackTarget(ability, attackerObj, targetObj));
                        break;
                    }
                case 1:
                    {
                        StartCoroutine(Charge(ability, targetObj));
                        break;
                    }
                case 2:
                    {
                        StartCoroutine(ScavengerSkill(ability, attackerObj, targetObj));
                        break;
                    }
                case 3:
                    {
                        StartCoroutine(Ultimate(ability, attackerObj, targetObj));
                        break;
                    }
            }
        }

        if (attackerType.Equals("Mutant"))
        {
            switch (ability.index)
            {
                case 0:
                    {
                        StartCoroutine(AttackTarget(ability, attackerObj, targetObj));
                        break;
                    }
                case 1:
                    {
                        StartCoroutine(MutantSkill(ability, attackerObj, targetObj));
                        break;
                    }
                case 2:
                    {
                        StartCoroutine(MutantSkill(ability, attackerObj, targetObj));
                        break;
                    }
                case 3:
                    {
                        StartCoroutine(Ultimate(ability, attackerObj, targetObj));
                        break;
                    }
            }
        }
    }

    public IEnumerator AttackTarget(Ability ability, GameObject attackerObj, GameObject targetObj)
    {
        cameraManager.Shake(true, 2);
        attackerObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);
        StartCoroutine(particleManager.PlayParticles(ability.particleIndex, new Vector3(attackerObj.transform.position.x, ability.spawnY)));
        yield return new WaitForSeconds(1f);

        targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(ability.directIndex, null);

        yield return new WaitForSeconds(1f);
        cameraManager.Shake(false, 2);
    }

    public IEnumerator Charge(Ability ability, GameObject targetObj)
    {
        targetObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);
        yield return new WaitForSeconds(1f);
        StartCoroutine(particleManager.PlayParticles(ability.particleIndex, targetObj.transform.position));
    }

    public IEnumerator ScavengerSkill(Ability ability, GameObject attackerObj, GameObject targetObj)
    {
        if (ability.type.Equals("Offensive"))
        {
            cameraManager.Shake(true, 1);
            attackerObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);

            if (ability.withDirect)
            {
                StartCoroutine(particleManager.PlayParticles(ability.particleIndex, targetObj.transform.position));
                targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(ability.directIndex, null);
            }

            yield return new WaitForSeconds(1f);
            cameraManager.Shake(false, 1);
        }
        
        if (ability.type.Equals("Defensive"))
        {
            attackerObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);
            StartCoroutine(particleManager.PlayParticles(ability.particleIndex, attackerObj.transform.position));
            yield return new WaitForSeconds(1f);

            if (ability.withDirect)
                targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(ability.directIndex, null);
        }
    }

    public IEnumerator MutantSkill(Ability ability, GameObject attackerObj, GameObject targetObj)
    {
        if (ability.type.Equals("Offensive"))
        {
            cameraManager.Shake(true, 1);
            attackerObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);

            if (ability.withDirect)
            {
                StartCoroutine(particleManager.PlayParticles(ability.particleIndex, targetObj.transform.position));
                targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(ability.directIndex, null);
            }

            yield return new WaitForSeconds(1f);
            cameraManager.Shake(false, 1);
        }

        if (ability.type.Equals("Defensive"))
        {
            attackerObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);
            StartCoroutine(particleManager.PlayParticles(ability.particleIndex, attackerObj.transform.position));
            yield return new WaitForSeconds(1f);

            if (ability.withDirect)
                targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(ability.directIndex, null);
        }
    }

    public IEnumerator Ultimate(Ability ability, GameObject attackerObj, GameObject targetObj)
    {
        if (ability.type.Equals("Offensive"))
        {
            cameraManager.Shake(true, 2);
            attackerObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);

            if (ability.withDirect)
            {
                StartCoroutine(particleManager.PlayParticles(ability.particleIndex, targetObj.transform.position));
                targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(ability.directIndex, null);
            }

            yield return new WaitForSeconds(1f);
            cameraManager.Shake(false, 2);
        }

        if (ability.type.Equals("Defensive"))
        {
            attackerObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);
            StartCoroutine(particleManager.PlayParticles(ability.particleIndex, attackerObj.transform.position));
            yield return new WaitForSeconds(1f);

            if (ability.withDirect)
                targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(ability.directIndex, null);
        }
    }
}
