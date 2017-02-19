using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSoundAreaActivator : BaseActivator {

    [Tooltip("Amount of units since that the target has to move towards the center of the trigger for the sound to play at max volume.")]
    public float fadeOffsetPosition;
    [Tooltip("Side to which the offset will be aplied at. For example if it's \"Left\" but the target enters from the right, the sound will play at max volume instantly.")]
    public FadeOffsetSide fadeOffsetSide = FadeOffsetSide.Both;
    [Tooltip("Sound to play when te target enters the trigger.")]
    public AudioSource audioSource;
    [Tooltip("Collider of the trigger.")]
    public Collider collider;
    public SoundType soundType;
    [Tooltip("If the sound will be played once or multiple times depending on a time interval.")]
    public PlaybackType playbackType = PlaybackType.Once;
    public float playbackInterval = 10f;
    private float maxVolume;
    /// <summary>
    /// Target that triggered the sound
    /// </summary>
    private GameObject target;
    private float timeWhenLastPlayed;

    public enum SoundType
    {
        Music,
        Sfx
    }

    public enum PlaybackType
    {
        Once,
        TimeInterval
    }

    public enum FadeOffsetSide
    {
        Left,
        Right,
        Both
    }

    void Start()
    {
        maxVolume = audioSource.volume;
    }

    void Update()
    {
        if (target == null) return;

        // 1 by default, which means max volume
        float delta = 1;
        Vector3 targetPosition = target.transform.position;
        if (fadeOffsetPosition > 0)
        {
            // Check the side the target is entering the trigger area to calculate the correct delta
            if (targetPosition.x < transform.position.x && (fadeOffsetSide == FadeOffsetSide.Both || fadeOffsetSide == FadeOffsetSide.Left))
            {
                delta = (targetPosition.x - collider.bounds.min.x) / fadeOffsetPosition;
            }
            else if (targetPosition.x > transform.position.x && (fadeOffsetSide == FadeOffsetSide.Both || fadeOffsetSide == FadeOffsetSide.Right))
            {
                delta = (collider.bounds.max.x - targetPosition.x) / fadeOffsetPosition;
            }
        }
        float globalVolume = SoundManager.Instance.globalMusicVolume;
        if (soundType == SoundType.Sfx)
        {
            globalVolume = SoundManager.Instance.globalSfxVolume;
        }
        float volume = Mathf.Lerp(0, maxVolume, delta) * globalVolume;
        audioSource.volume = volume;

        if (volume > 0 && playbackType == PlaybackType.TimeInterval && Time.time - timeWhenLastPlayed >= playbackInterval)
        {
            audioSource.Play();
            timeWhenLastPlayed = Time.time;
        }
    }

    public override void Activate(GameObject trigger)
    {
        if (playbackType == PlaybackType.Once)
        {
            audioSource.volume = 0;
            audioSource.Play();
        } else if (playbackType == PlaybackType.TimeInterval)
        {
            timeWhenLastPlayed = Time.time;
        }
        target = trigger;
    }

    public override void Desactivate()
    {
        if (playbackType == PlaybackType.Once && audioSource.loop)
        {
            audioSource.Pause();
        }
        target = null;
    }
}
