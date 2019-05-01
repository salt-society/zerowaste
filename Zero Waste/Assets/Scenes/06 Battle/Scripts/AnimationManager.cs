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

        if (ability.particleIndex != -1)
            StartCoroutine(particleManager.PlayParticles(ability.particleIndex, new Vector3(attackerObj.transform.position.x, ability.spawnY)));
        yield return new WaitForSeconds(1f);

        targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(ability.directIndex, null);

        yield return new WaitForSeconds(1f);
        cameraManager.Shake(false, 2);
    }

    public IEnumerator Charge(Ability ability, GameObject targetObj)
    {
        StartCoroutine(particleManager.PlayParticles(ability.particleIndex, targetObj.transform.position));
        targetObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator ScavengerSkill(Ability ability, GameObject attackerObj, GameObject targetObj)
    {
        if (ability.type.Equals("Offensive"))
        {
            cameraManager.Shake(true, 1);
            attackerObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);
            StartCoroutine(particleManager.PlayParticles(ability.particleIndex, attackerObj.transform.position));

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
            cameraManager.Shake(true, 2);
            attackerObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);
            StartCoroutine(particleManager.PlayParticles(ability.particleIndex, attackerObj.transform.position));

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
            StartCoroutine(particleManager.PlayParticles(ability.particleIndex, attackerObj.transform.position));

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
