using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatHead : EnemyDamageableEntity
{
    [Tooltip("Distance on the Y axis where the soul drop will appear.")]
    public float soulDropDistance;
    [Tooltip("Time to enable the cat head after the soul dissapears.")]
    public float spawnTime;
    //
    public Collider headCollider;
    public Animator animator;
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
                Spawn();
            }
        }
    }

    protected override void OnDeath()
    {
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
        //ShowRenderer(false);
        // subscribe to soul event
        soulDrop.SoulDestroyedEvent += OnSoulDestroy;

        headCollider.enabled = false;
        soulObject.SetActive(true);
    }

    private void Spawn()
    {
        ShowRenderer(true);
        headCollider.enabled = true;
        currentSpawnTime = 0;
        Refresh();
        waiting = false;
        animator.SetTrigger("Respawn");
        animator.SetBool("IsDead", false);
    }

    protected void ShowRenderer(bool show)
    {
        Renderer[] renderChildren = GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderChildren.Length; ++i)
        {
            renderChildren[i].enabled = show;
        }

    }

    private void OnSoulDestroy(GameObject sender, bool playerCollected)
    {
        // start the spawn counter
        waiting = true;
        // unsubscribe event!
        soulDrop.SoulDestroyedEvent -= OnSoulDestroy;
    }
}
