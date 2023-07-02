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

    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
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
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
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

        AudioMixerGroup[] groups = mainMixer.FindMatchingGroups("Doors");
        if (groups.Length > 0)
        {
            audioSource.outputAudioMixerGroup = groups[0];
        }
        else
        {
            Debug.LogError("Could not find Doors group in the main mixer");
        }

        if (openAnimation != null)
            openAudio = Resources.Load<AudioClip>("Audio/Door/Open");
        if (closeAnimation != null)
            closeAudio = Resources.Load<AudioClip>("Audio/Door/Close");

        doorAnimation.playAutomatically = false;
    }


    public void SetClosestDoor()
    {

        DoorController closestDoor = LevelManager.Instance?.GetClosestDoor(this);

        if (closestDoor != null)
        {
            UpdateRenderer(closestDoor);
            closestDoor.UpdateRenderer(this);
            closestDoor.AssignDoor(this);
            otherDoor = closestDoor;
        }



    }

    public void UpdateRenderer(DoorController otherController)
    {
        if (otherController == null) return;

        bool isLeft = door.wallType == Wall.WallType.Left;
        bool isRight = door.wallType == Wall.WallType.Right;
        if (isLeft)
            if (openAnimation != null) return;
            else if (otherController.openAnimation)
            {
                if (openAnimation == null) return;
                meshCollider.enabled = false;
                meshRenderer.enabled = false;
                isOpen = true;
            }
            else
            {
                meshCollider.enabled = true;
                meshRenderer.enabled = true;
            }
    }

    public void OpenDoorForced()
    {
        if (openAnimation)
        {
            isOpen = true;
            doorAnimation.clip = openAnimation;
            doorAnimation.Play();
            StartCoroutine(AutoClose(silent: true));
        }

    }

    public void OpenDoor(bool silent = false)
    {
        SetClosestDoor();
        if (isLocked)
            if (otherDoor)
                if (!otherDoor.isLocked && door.wallType == Wall.WallType.Left)
                    isLocked = false;
                else
                    return;
            else
                return;

        if (otherDoor)
            otherDoor.OpenDoorForced();

        if (openAudio != null && !silent)
        {
            audioSource.clip = openAudio;
            audioSource.Play();
        }

        if (openAnimation)
        {
            isOpen = true;
            doorAnimation.clip = openAnimation;
            doorAnimation.Play();
            StartCoroutine(AutoClose());
        }
    }

    public IEnumerator AutoClose(bool silent = false)
    {
        if (autoCloseTime == 0)
            yield break;

        yield return new WaitForSeconds(autoCloseTime);

        CloseDoor(silent: silent);
    }

    public void CloseDoor(bool silent = false)
    {
        isOpen = false;

        if (closeAnimation != null)
        {
            doorAnimation.clip = closeAnimation;
            doorAnimation.Play();
        }

        if (closeAudio != null && !silent)
        {
            audioSource.clip = closeAudio;
            audioSource.Play();
        }
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
