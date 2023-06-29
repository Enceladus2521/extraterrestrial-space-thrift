using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;

[RequireComponent(typeof(Animation))]
public class DoorController : MonoBehaviour
{
    public Door door;
    public bool isOpen = false;
    public bool isLocked = true;
    public float autoCloseTime;
    public AnimationClip openAnimation;
    public AnimationClip closeAnimation;
    public AudioClip openAudio;
    public AudioClip closeAudio;

    public string audioMixerGroup = "FX";

    private Animation doorAnimation;
    private AudioSource audioSource;

    [SerializeField]
    private DoorController otherDoor;

    public void AssignDoor(DoorController otherOtherDoor)
    {
        otherDoor = otherOtherDoor;

    }

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


    public void SetClosestDoor()
    {
        // only if instansiated
        // if(!door.isConnected)
        //  {
        //     Debug.Log("door is not connected");
        //     return;
        //  }

        // Find closest door by tag "door"
        GameObject[] doors = GameObject.FindGameObjectsWithTag("wall");
        if (doors.Length == 0)
        {
            Debug.Log("no doors found in game");
            return;
        }
        DoorController closestDoor = null;
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < doors.Length; i++)
        {
            DoorController otherDoor = doors[i].GetComponent<DoorController>();
            // check if its not the same door, other door will have the opposite wall type
            if (otherDoor != null)
            {
                if (otherDoor.gameObject == gameObject)
                {
                    continue;
                }


                float distance = Vector3.Distance(transform.position, otherDoor.transform.position);
                if (distance < closestDistance)
                {
                    closestDoor = otherDoor;
                    closestDistance = distance;
                }
            }
            else
            {
                Debug.Log("other door is null");
            }

        }

        if (closestDoor == null)
        {
            Debug.Log("no doors found");
        }

        closestDoor.AssignDoor(this);

        otherDoor = closestDoor;
    }

    public void OpenDoorForced()
    {
        isOpen = true;

        // Play open animation
        doorAnimation.clip = openAnimation;
        doorAnimation.Play();

        // Play open audio
        audioSource.clip = openAudio;
        audioSource.Play();

        StartCoroutine(AutoClose());
    }

    public void OpenDoor()
    {
        SetClosestDoor();
        if (isLocked)
        {
            Debug.Log("is locked");
            return;
        }

        if (otherDoor)
            otherDoor.OpenDoorForced();
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
        SetClosestDoor();
        if (otherDoor)
            otherDoor.SetClosestDoor();
        isLocked = locked;
        if (door.wallType == Wall.WallType.Left)
            otherDoor.ToggleDoorLockState(locked);
    }

    void OnDrawGizmos()
    {
        if (isLocked)
            Gizmos.color = Color.blue;
        else
            Gizmos.color = Color.red;
        // draw door
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
        Gizmos.DrawWireSphere(transform.position, 0.1f);

    }
}
