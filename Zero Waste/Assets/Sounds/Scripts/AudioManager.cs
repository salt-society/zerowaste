using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Sound List")]
    public Sound[] sounds;

    [Space]
    public static AudioManager instance;

    void Awake()
    {
        // Keep audio manager alive through out the game
        if (instance == null)
            instance = this;
        else if(instance != null)
            Destroy(this);

        DontDestroyOnLoad(gameObject);

        // Then prepare sounds
        PrepareSounds();
    }

    void PrepareSounds()
    {
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();

            sound.source.clip = sound.audioClip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;

            sound.source.playOnAwake = sound.playOnAwake;
            sound.source.loop = sound.loopClip;
        }
    }


    public void PlaySound(string soundName)
    {
        Sound soundToPlay = Array.Find(sounds, sound => sound.soundName == soundName);

        if (soundToPlay == null)
        {
            Debug.LogWarning("No sounds found.");
            return;
        }

        soundToPlay.source.Play();
    }

    public IEnumerator StopSound(string soundName, float fadeTime)
    {
        Sound soundToStop = Array.Find(sounds, sound => sound.soundName == soundName);

        if (soundToStop == null)
        {
            Debug.LogWarning("No sounds found.");
        }
        else
        {
            float startVolume = soundToStop.source.volume;
            while (soundToStop.source.volume > 0)
            {
                soundToStop.source.volume -= startVolume * Time.deltaTime / fadeTime;
                yield return null;
            }

            soundToStop.source.volume = soundToStop.volume;
            soundToStop.source.Stop();
            Debug.Log(soundToStop.source.isPlaying);
        }
    }
}
