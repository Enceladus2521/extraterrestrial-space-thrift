using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip[] musicTracks;
    private int currentTrackIndex = 0;
    private AudioMixer mainMixer;

    void Awake()
    {
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // If AudioSource does not exist, add one
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Load music tracks from resources
        musicTracks = Resources.LoadAll<AudioClip>("Audio/Music");

        // Load the AudioMixer
        mainMixer = Resources.Load<AudioMixer>("Audio/Mixer/Main");
        if (mainMixer == null)
        {
            Debug.LogError("Could not load main mixer");
            return;
        }

        // Get the "Music" group from the AudioMixer
        AudioMixerGroup[] groups = mainMixer.FindMatchingGroups("Music");
        if (groups.Length > 0)
        {
            audioSource.outputAudioMixerGroup = groups[0];
        }
        else
        {
            Debug.LogError("Could not find Music group in the main mixer");
        }
    }

    void Start()
    {
        // Start playing music
        StartCoroutine(PlayMusicTracks());
    }

    IEnumerator PlayMusicTracks()
    {
        while (true)
        {
            // Play current track
            audioSource.clip = musicTracks[currentTrackIndex];
            audioSource.Play();

            // Wait for the current track to finish playing
            yield return new WaitForSeconds(audioSource.clip.length);

            // Move to the next track, looping back to the first track if necessary
            currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length;
        }
    }
}
