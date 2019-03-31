using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public Transform canvasTransform;
    public List<ParticleSystem> particleList;
    public List<ParticleSystem> particlesOnLoop;

    public IEnumerator PlayParticles(int particleIndex, Vector3 instantiatePos)
    {
        float duration = particleList[particleIndex].main.duration;

        ParticleSystem newParticle = Instantiate(particleList[particleIndex]);
        newParticle.transform.position = instantiatePos;

        yield return new WaitForSeconds(duration);
        Destroy(newParticle);
        Destroy(newParticle.gameObject);
    }

    public void PlayLoopingParticles(int particleIndex, Vector3 instantiatePos)
    {
        float duration = particleList[particleIndex].main.duration;

        ParticleSystem newParticle = Instantiate(particleList[particleIndex], canvasTransform);
        newParticle.transform.position = instantiatePos;
        particlesOnLoop.Add(newParticle);
    }

}
