using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private CameraManager cameraManager;
    private StatusManager statusManager;
    private ParticleManager particleManager;
    private AudioManager audioManager;

    void Start()
    {
        statusManager = FindObjectOfType<StatusManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        particleManager = FindObjectOfType<ParticleManager>();
        audioManager = FindObjectOfType<AudioManager>();
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

        if (ability.castParticle != -1)
        {
            if (ability.customCoords)
            {
                if (ability.cCoordX)
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(attackerObj.transform.position.x, ability.spawnY)));
                }
                else if (ability.cCoordY)
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, ability.spawnY)));
                }
                else
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, attackerObj.transform.position.y)));
                }
            }
            else
            {
                StartCoroutine(particleManager.PlayParticles(ability.castParticle, attackerObj.transform.position));
            }
        }

        audioManager.PlaySound(ability.SFX);
        yield return new WaitForSeconds(1f);

        targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(ability.toSelfDirectEffect, null);

        yield return new WaitForSeconds(1f);
        cameraManager.Shake(false, 2);
    }

    public IEnumerator Charge(Ability ability, GameObject targetObj)
    {
        if (ability.customCoords)
        {
            if (ability.cCoordX)
            {
                StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(targetObj.transform.position.x, ability.spawnY)));
            }
            else if (ability.cCoordY)
            {
                StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, ability.spawnY)));
            }
            else
            {
                StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, targetObj.transform.position.y)));
            }
        }
        else
        {
            StartCoroutine(particleManager.PlayParticles(ability.castParticle, targetObj.transform.position));
        }

        targetObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);
        audioManager.PlaySound(ability.SFX);
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator ScavengerSkill(Ability ability, GameObject attackerObj, GameObject targetObj)
    {
        if (ability.type.Equals("Offensive"))
        {
            cameraManager.Shake(true, 1);
            attackerObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);

            if (ability.customCoords)
            {
                if (ability.cCoordX)
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(attackerObj.transform.position.x, ability.spawnY)));
                }
                else if (ability.cCoordY)
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, ability.spawnY)));
                }
                else
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, attackerObj.transform.position.y)));
                }
            }
            else
            {
                StartCoroutine(particleManager.PlayParticles(ability.castParticle, attackerObj.transform.position));
            }

            audioManager.PlaySound(ability.SFX);

            if (ability.withDirectEffect)
            {
                // StartCoroutine(particleManager.PlayParticles(ability.castParticle, attackerObj.transform.position));
                attackerObj.GetComponent<CharacterMonitor>().EffectsAnimation(ability.toSelfDirectEffect, null);
            }

            yield return new WaitForSeconds(1f);
            cameraManager.Shake(false, 1);
        }
        
        if (ability.type.Equals("Defensive"))
        {
            attackerObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);

            if (ability.customCoords)
            {
                if (ability.cCoordX)
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(attackerObj.transform.position.x, ability.spawnY)));
                }
                else if (ability.cCoordY)
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, ability.spawnY)));
                }
                else
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, attackerObj.transform.position.y)));
                }
            }
            else
            {
                StartCoroutine(particleManager.PlayParticles(ability.castParticle, attackerObj.transform.position));
            }

            audioManager.PlaySound(ability.SFX);
            yield return new WaitForSeconds(1f);

            if (ability.withDirectEffect)
                targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(ability.toSelfDirectEffect, null);
        }
    }

    public IEnumerator MutantSkill(Ability ability, GameObject attackerObj, GameObject targetObj)
    {
        if (ability.type.Equals("Offensive"))
        {
            cameraManager.Shake(true, 2);
            attackerObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);

            if (ability.customCoords)
            {
                if (ability.cCoordX)
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(attackerObj.transform.position.x, ability.spawnY)));
                }
                else if (ability.cCoordY)
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, ability.spawnY)));
                }
                else
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, attackerObj.transform.position.y)));
                }
            }
            else
            {
                StartCoroutine(particleManager.PlayParticles(ability.castParticle, attackerObj.transform.position));
            }

            audioManager.PlaySound(ability.SFX);

            if (ability.withDirectEffect)
            {
                // StartCoroutine(particleManager.PlayParticles(ability.castParticle, attackerObj.transform.position));
                attackerObj.GetComponent<CharacterMonitor>().EffectsAnimation(ability.toSelfDirectEffect, null);
            }

            yield return new WaitForSeconds(1f);
            cameraManager.Shake(false, 2);
        }

        if (ability.type.Equals("Defensive"))
        {
            attackerObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);

            if (ability.customCoords)
            {
                if (ability.cCoordX)
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(attackerObj.transform.position.x, ability.spawnY)));
                }
                else if (ability.cCoordY)
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, ability.spawnY)));
                }
                else
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, attackerObj.transform.position.y)));
                }
            }
            else
            {
                StartCoroutine(particleManager.PlayParticles(ability.castParticle, attackerObj.transform.position));
            }

            audioManager.PlaySound(ability.SFX);
            yield return new WaitForSeconds(1f);

            if (ability.withDirectEffect)
                targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(ability.toSelfDirectEffect, null);
        }
    }

    public IEnumerator Ultimate(Ability ability, GameObject attackerObj, GameObject targetObj)
    {
        if (ability.type.Equals("Offensive"))
        {
            cameraManager.Shake(true, 2);
            attackerObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);

            if (ability.customCoords)
            {
                if (ability.cCoordX)
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(attackerObj.transform.position.x, ability.spawnY)));
                }
                else if (ability.cCoordY)
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, ability.spawnY)));
                }
                else
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, attackerObj.transform.position.y)));
                }
            }
            else
            {
                StartCoroutine(particleManager.PlayParticles(ability.castParticle, attackerObj.transform.position));
            }
            
            audioManager.PlaySound(ability.SFX);

            if (ability.withDirectEffect)
            {
                // StartCoroutine(particleManager.PlayParticles(ability.castParticle, attackerObj.transform.position));
                attackerObj.GetComponent<CharacterMonitor>().EffectsAnimation(ability.toSelfDirectEffect, null);
            }

            yield return new WaitForSeconds(1f);
            cameraManager.Shake(false, 2);
        }

        if (ability.type.Equals("Defensive"))
        {
            attackerObj.GetComponent<CharacterMonitor>().AbilityAnimations(ability);
            if (ability.customCoords)
            {
                if (ability.cCoordX)
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(attackerObj.transform.position.x, ability.spawnY)));
                }
                else if (ability.cCoordY)
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, ability.spawnY)));
                }
                else
                {
                    StartCoroutine(particleManager.PlayParticles(ability.castParticle, new Vector2(ability.spawnX, attackerObj.transform.position.y)));
                }
            }
            else
            {
                StartCoroutine(particleManager.PlayParticles(ability.castParticle, attackerObj.transform.position));
            }

            audioManager.PlaySound(ability.SFX);
            yield return new WaitForSeconds(1f);

            if (ability.withDirectEffect)
                targetObj.GetComponent<CharacterMonitor>().EffectsAnimation(ability.toSelfDirectEffect, null);
        }
    }
}
