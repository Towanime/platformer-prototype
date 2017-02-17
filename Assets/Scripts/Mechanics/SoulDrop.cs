using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerCollected will be true only if the player touched the soul, it'll be false when the timer runs out.
/// </summary>
/// <param name="sender"></param>
/// <param name="playerCollected"></param>
public delegate void SoulDestroyEventHandler(GameObject sender, bool playerCollected);

public class SoulDrop : MonoBehaviour {

    [Tooltip("Time in seconds that the soul will be alive, if set to 0 the soul won't dissapear until collected.")]
    public float lifeSpan = 10;
    [Tooltip("If the soul can be collected by the player or not.")]
    public bool collectable = true;
    /// <summary>
    /// Subscribe to this to know when a souls dissapears by collection or time running out.
    /// </summary>
    public event SoulDestroyEventHandler SoulDestroyedEvent;
    private ISpawnPoint spawnPoint;
    private bool wasCollected;
    
    public void Initialize(ISpawnPoint spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }

    public void OnTriggerEnter(Collider other)
    {
        // if it's the player then release it!
        if (other.CompareTag("Player") && collectable)
        {
            SoundManager.Instance.StopAndPlay(SoundManager.Instance.souldPickupSound);
            wasCollected = true;
            Destroy();
        }
    }

    public void OnTriggerExit(Collider other)
    {

    }

    void OnEnable()
    {
        wasCollected = false;
        if (lifeSpan > 0)
        {
            Invoke("Destroy", lifeSpan);
        }
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void Destroy()
    {
        // notify the listeners
        if (SoulDestroyedEvent != null) SoulDestroyedEvent(gameObject, wasCollected);
        // Remove soul from teleport area if needed
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        TeleportTriggerArea teleportTriggerArea = player.GetComponentInChildren<TeleportTriggerArea>();
        if (teleportTriggerArea != null)
        {
            teleportTriggerArea.RemoveSoulFromArea(gameObject);
        }
        // trigger spawn if any!
        if (spawnPoint != null)
        {
            spawnPoint.Spawn();
        }
        BulletPool.instance.ReleaseObject(gameObject);
    }
}
