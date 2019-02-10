using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public AudioClip audioClip;
    public string soundName;
    [HideInInspector]
    public AudioSource source;

    [Space]
    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;
    [Range(0f, 1f)]
    public float spatialBlend;

    [Space]
    public bool playOnAwake;
    public bool loopClip;
}
