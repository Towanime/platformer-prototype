using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager Instance { get { return instance; } }

    public AudioClip[] jumpSounds;
    public AudioClip[] avatarPainSounds;
    public AudioClip overheatSound;
    public AudioClip coinPickupSound;
    public AudioClip bulletSpawnSound;

    private Dictionary<GameObject, AudioSource> soundsBeingPlayed = new Dictionary<GameObject, AudioSource>();
    private List<GameObject> pooledObjectsToRelease = new List<GameObject>();
    private static SoundManager instance;

    protected SoundManager()
    {

    }

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // Check which sources have stopped playing and store the reference
        foreach (KeyValuePair<GameObject, AudioSource> entry in soundsBeingPlayed)
        {
            AudioSource audioSource = entry.Value;
            if (!audioSource.isPlaying)
            {
                GameObject pooledObject = entry.Key;
                pooledObjectsToRelease.Add(pooledObject);
            }
        }
        // Return those objects to the pool
        foreach (GameObject pooledObject in pooledObjectsToRelease)
        {
            AudioSourceContainerPool.instance.ReleaseObject(pooledObject);
            soundsBeingPlayed.Remove(pooledObject);
        }
        pooledObjectsToRelease.Clear();
    }

    /// <summary>
    /// Randomly choses one of the given audio clips and plays it.
    /// </summary>
    /// <param name="audioClips"></param>
    public void PlayRandom(AudioClip[] audioClips)
    {
        int index = Random.Range(0, audioClips.Length);
        Play(audioClips[index]);
    }

    /// <summary>
    /// Play an aduio clip.
    /// </summary>
    /// <param name="audioClip"></param>
    public void Play(AudioClip audioClip)
    {
        // Get the audio source container from the pool
        GameObject poolObject = AudioSourceContainerPool.instance.GetObject();
        poolObject.SetActive(true);
        // Get the audio source component, set the clip to play and play it
        AudioSource audioSource = poolObject.GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
        // Keep reference to the objects to return them later to the pool
        soundsBeingPlayed.Add(poolObject, audioSource);
    }

    /// <summary>
    /// Stops every audio source currently playing the given clip.
    /// </summary>
    /// <param name="audioClip"></param>
    public void Stop(AudioClip audioClip)
    {
        foreach (KeyValuePair<GameObject, AudioSource> entry in soundsBeingPlayed)
        {
            AudioSource audioSource = entry.Value;
            if (audioSource.clip.Equals(audioClip))
            {
                audioSource.Stop();
            }
        }
    }

    /// <summary>
    /// Stops every audio source currently playing the given clip and then plays it from the begining.
    /// </summary>
    /// <param name="audioClip"></param>
    public void StopAndPlay(AudioClip audioClip)
    {
        Stop(audioClip);
        Play(audioClip);
    }
}
