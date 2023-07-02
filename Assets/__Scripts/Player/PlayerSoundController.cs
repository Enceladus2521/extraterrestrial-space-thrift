using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSoundController : MonoBehaviour
{
    AudioClip[] stepSounds;
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        // Set the audio output to the "FX" audio mixer 
        AudioMixer mainMixer = Resources.Load<AudioMixer>("Audio/Mixer/Main");
        if (mainMixer == null)
        {
            Debug.LogError("Could not load main mixer");
            return;
        }

        AudioMixerGroup[] groups = mainMixer.FindMatchingGroups("Player");
        if (groups.Length > 0)
        {
            audioSource.outputAudioMixerGroup = groups[0];
        }
        else
        {
            Debug.LogError("Could not find Player group in the main mixer");
        }


        stepSounds = Resources.LoadAll<AudioClip>("Audio/Player/Steps");
    }

    public void PlayStepSound()
    {
        int index = Random.Range(0, stepSounds.Length);
        audioSource.PlayOneShot(stepSounds[index]);
    }

}
