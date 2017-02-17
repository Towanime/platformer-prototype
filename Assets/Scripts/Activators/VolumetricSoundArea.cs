using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumetricSoundArea : BaseActivator {

    public float fadeOffsetPosition;
    public AudioSource audioSource;
    public Collider collider;
    private GameObject target;

    void Update()
    {
        if (target == null) return;

        float currentVolume = 0;
        float delta = 1;
        Vector3 targetPosition = target.transform.position;
        if (targetPosition.x < transform.position.x)
        {
            if (fadeOffsetPosition > 0)
            {
                delta = (targetPosition.x - collider.bounds.min.x) / fadeOffsetPosition;
            }
        } else
        {
            if (fadeOffsetPosition > 0)
            {
                delta = (collider.bounds.max.x - targetPosition.x) / fadeOffsetPosition;
            }
        }
        currentVolume = Mathf.Lerp(0, 1, delta);
        audioSource.volume = currentVolume;
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
        audioSource.Stop();
    }
}
