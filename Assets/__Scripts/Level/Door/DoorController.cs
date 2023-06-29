using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DoorController : MonoBehaviour
{
    public Door door;
    public bool isOpen = false;
    public bool isLocked = false;
    public float autoCloseTime;
    public AudioClip openAudio;
    public AudioClip closeAudio;

    public string audioMixerGroup = "FX";

    private AudioSource audioSource;

    [SerializeField]
    private DoorController otherDoor;

    private void Awake()
    {
        // Get closest other door and set the other door
        otherDoor = FindClosestDoor();

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
    }


    public DoorController FindClosestDoor()
    {
        // Find closest door by tag "door"
        GameObject[] doors = GameObject.FindGameObjectsWithTag("door");
        DoorController closestDoor = null;
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < doors.Length; i++)
        {
            DoorController otherDoor = doors[i].GetComponent<DoorController>();
            
            float distance = Vector3.Distance(transform.position, otherDoor.transform.position);
            if (distance < closestDistance)
            {
                closestDoor = otherDoor;
                closestDistance = distance;
            }
        }
        return closestDoor;
    }

    public void AssignDoor(Door doorObj)
    {
        otherDoor = FindClosestDoor();
        this.door = doorObj;
    }

    public bool OpenDoor()
    {
        if (otherDoor)
        {
            bool isOtherLocked = otherDoor.OpenDoor();
            if (isLocked)
            {
                Debug.Log("Other door is locked");
                return false;
            }
        }

        if (isLocked)
        {
            return false;
        }
        isOpen = true;

        // Play open audio
        audioSource.clip = openAudio;
        audioSource.Play();

        StartCoroutine(AutoClose());
        return true;
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

        // Play close audio
        audioSource.clip = closeAudio;
        audioSource.Play();
    }

    public void ToggleDoorLockState(bool locked)
    {
        isLocked = locked;
        if (isLocked && otherDoor != null)
        {
            otherDoor.ToggleDoorLockState(true);
        }
    }
}
