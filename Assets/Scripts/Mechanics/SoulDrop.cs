using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulDrop : MonoBehaviour {

    public float lifeSpan = 10;
    private SpawnPoint spawnPoint;
    
    public void Initialize(SpawnPoint spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }

    public void OnTriggerEnter(Collider other)
    {
        // if it's the player then release it!
        if (other.CompareTag("Player"))
        {
            Destroy();
        }
    }

    public void OnTriggerExit(Collider other)
    {

    }

    void OnEnable()
    {
        Invoke("Destroy", lifeSpan);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void Destroy()
    {
        // Remove soul from teleport area if needed
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        TeleportTriggerArea teleportTriggerArea = player.GetComponentInChildren<TeleportTriggerArea>();
        teleportTriggerArea.RemoveSoulFromArea(gameObject);
        // trigger spawn if any!
        if (spawnPoint)
        {
            spawnPoint.Spawn();
        }
        BulletPool.instance.ReleaseObject(gameObject);
    }
}
