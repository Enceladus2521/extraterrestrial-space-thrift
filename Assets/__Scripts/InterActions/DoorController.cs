using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;

[RequireComponent(typeof(Animation))]
public class DoorController : MonoBehaviour
{
    public bool isOpen = false;
    public bool isLocked = false;
    public float autoCloseTime;
    public AnimationClip openAnimation;
    public AnimationClip closeAnimation;
    public AudioClip openAudio;
    public AudioClip closeAudio;

    public string audioMixerGroup = "FX";

    private Animation doorAnimation;
    private AudioSource audioSource;
    
    // public AudioMixerGroup mixerGroup;

    private void Awake()
    {
        doorAnimation = GetComponent<Animation>();

        // Check if an AudioSource component already exists
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Create a new AudioSource component
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set the audio output to the "FX" audio mixer 
        AudioMixer mainMixer = Resources.Load<AudioMixer>("Audio/Mixer/Main");
        if (mainMixer == null)
        {
            Debug.LogError("Could not load main mixer");
            return;
        }

        AudioMixerGroup[] groups = mainMixer.FindMatchingGroups("FX");
        if (groups.Length > 0)
        {
            audioSource.outputAudioMixerGroup = groups[0];
        }
        else
        {
            Debug.LogError("Could not find FX group in the main mixer");
        }



        // Load audio clips from resources/audio folder
        openAudio = Resources.Load<AudioClip>("Audio/Door/Open");
        closeAudio = Resources.Load<AudioClip>("Audio/Door/Close");

        doorAnimation.playAutomatically = false;
    }

    public void OpenDoor()
    {
        if (isLocked)
        {
            Debug.Log("Door is locked");
            return;
        }

        isOpen = true;

        // Play open animation
        doorAnimation.clip = openAnimation;
        doorAnimation.Play();

        // Play open audio
        audioSource.clip = openAudio;
        audioSource.Play();

        StartCoroutine(AutoClose());
    }

    public IEnumerator AutoClose()
    {
        if (autoCloseTime == 0)
            yield break;

        yield return new WaitForSeconds(autoCloseTime);

        CloseDoor();
    }

    public void CloseDoor()
    {
        isOpen = false;

        // Play close animation
        doorAnimation.clip = closeAnimation;
        doorAnimation.Play();

        // Play close audio
        audioSource.clip = closeAudio;
        audioSource.Play();
    }

    public void ToggleDoorLockState(bool locked)
    {
        isLocked = locked;
    }
}
