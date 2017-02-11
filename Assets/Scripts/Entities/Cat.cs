using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : EnemyDamageableEntity, ISpawnPoint
{
    [Tooltip("Distance on the Y axis where the soul drop will appear.")]
    public float soulDropDistance;
    [Tooltip("Time to enable the cat head after the soul dissapears.")]
    public float spawnTime;
    //
    public Collider headCollider;
    public Animator animator;
    private bool isAlive = true;
    private float currentSpawnTime;
    private bool waiting;
    private SoulDrop soulDrop;

    public void Update()
    {
        if (waiting)
        {
            currentSpawnTime += Time.deltaTime;
            // check if it's ready to create the enemy
            if (currentSpawnTime >= spawnTime)
            {
                FinalizeSpawn();
            }
        }
    }

    public void GrabKill()
    {
        isAlive = false;
        animator.SetBool("IsDead", true);
        headCollider.enabled = false;
    }

    protected override void OnDeath()
    {
        isAlive = false;
        animator.SetBool("IsDead", true);
        // override with cat behavior
        // create drop soul and place
        GameObject soulObject = SoulPool.instance.GetObject();
        soulDrop = soulObject.GetComponent<SoulDrop>();
        soulDrop.Initialize(null);
        // set on top of the cat
        Vector3 position = transform.position;
        position.y += soulDropDistance;
        soulObject.transform.position = position;
        // activate soul drop and disable cat head
        // subscribe to soul event
        soulDrop.SoulDestroyedEvent += OnSoulDestroy;

        headCollider.enabled = false;
        soulObject.SetActive(true);
    }

    private void FinalizeSpawn()
    {
        isAlive = true;
        headCollider.enabled = true;
        currentSpawnTime = 0;
        Refresh();
        waiting = false;
        animator.SetTrigger("Respawn");
        animator.SetBool("IsDead", false);
    }

    private void OnSoulDestroy(GameObject sender, bool playerCollected)
    {
        // start the spawn counter
        waiting = true;
        // unsubscribe event!
        soulDrop.SoulDestroyedEvent -= OnSoulDestroy;
    }

    public bool Spawn()
    {
        waiting = true;
        return true;
    }

    public bool IsAlive
    {
        get { return this.isAlive; }
    }
}
