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
    private float maxVolume;
    /// <summary>
    /// Target that triggered the sound
    /// </summary>
    private GameObject target;

    public enum SoundType
    {
        Music,
        Sfx
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
        audioSource.volume = Mathf.Lerp(0, maxVolume, delta) * globalVolume;
    }

    public override void Activate(GameObject trigger)
    {
        audioSource.volume = 0;
        audioSource.Play();
        target = trigger;
    }

    public override void Desactivate()
    {
        target = null;
        audioSource.Pause();
    }
}
