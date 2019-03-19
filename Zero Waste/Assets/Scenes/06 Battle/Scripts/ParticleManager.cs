using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public Transform canvasTransform;
    public List<ParticleSystem> particles;

    public IEnumerator CircleBurst(Vector3 instantiatePos)
    {
        float duration = particles[0].main.duration;

        ParticleSystem newParticle = Instantiate(particles[0], canvasTransform);
        newParticle.transform.position = instantiatePos;

        yield return new WaitForSeconds(duration);
        Destroy(newParticle);
    }
}
