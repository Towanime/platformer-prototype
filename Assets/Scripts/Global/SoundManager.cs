using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager Instance { get { return instance; } }

    [Range(0f, 1f)]
    public float globalSfxVolume = 1f;
    [Range(0f, 1f)]
    public float globalMusicVolume = 1f;
    public AudioClipInfo[] jumpSounds;
    public AudioClipInfo[] avatarPainSounds;
    public AudioClipInfo[] catPainSounds;
    public AudioClipInfo[] fireWhenOverheatSounds;
    public AudioClipInfo overheatCoolDownSound;
    public AudioClipInfo overheatLaughSound;
    public AudioClipInfo overheatKettleSound;
    public AudioClipInfo reachedOverheatSound;
    public AudioClipInfo loadingGunSound;
    public AudioClipInfo coinPickupSound;
    public AudioClipInfo bulletSpawnSound;
    public AudioClipInfo soulPickupSound;
    public AudioClipInfo shadowWalkSound;
    public AudioClipInfo catDeathSound;

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
    public AudioSource PlayRandom(AudioClipInfo[] audioClips)
    {
        int index = Random.Range(0, audioClips.Length);
        return Play(audioClips[index]);
    }

    /// <summary>
    /// Play an aduio clip.
    /// </summary>
    /// <param name="audioClip"></param>
    public AudioSource Play(AudioClipInfo audioClipInfo)
    {
        // Get the audio source container from the pool
        GameObject poolObject = AudioSourceContainerPool.instance.GetObject();
        poolObject.SetActive(true);
        // Get the audio source component, set the clip to play and play it
        AudioSource audioSource = poolObject.GetComponent<AudioSource>();
        SetInitialValues(audioSource, audioClipInfo);
        audioSource.Play();
        // Keep reference to the objects to return them later to the pool
        soundsBeingPlayed.Add(poolObject, audioSource);
        return audioSource;
    }

    /// <summary>
    /// Stops every audio source currently playing the given clip.
    /// </summary>
    /// <param name="audioClip"></param>
    public void Stop(AudioClipInfo audioClipInfo)
    {
        AudioSource audioSource = GetAudioSource(audioClipInfo.audioClip);
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Stops every audio source currently playing the given clip and then plays it from the begining.
    /// </summary>
    /// <param name="audioClip"></param>
    public AudioSource StopAndPlay(AudioClipInfo audioClipInfo)
    {
        AudioSource audioSource = GetAudioSource(audioClipInfo.audioClip);
        if (audioSource != null)
        {
            SetInitialValues(audioSource, audioClipInfo);
            audioSource.time = 0;
        } else
        {
            audioSource = Play(audioClipInfo);
        }
        return audioSource;
    }

    private AudioSource GetAudioSource(AudioClip audioClip)
    {
        foreach (KeyValuePair<GameObject, AudioSource> entry in soundsBeingPlayed)
        {
            AudioSource audioSource = entry.Value;
            if (audioSource.clip.Equals(audioClip))
            {
                return audioSource;
            }
        }
        return null;
    }

    private void SetInitialValues(AudioSource audioSource, AudioClipInfo audioClipInfo)
    {
        audioSource.clip = audioClipInfo.audioClip;
        audioSource.volume = globalSfxVolume * audioClipInfo.volume;
        audioSource.loop = false;
    }
}
