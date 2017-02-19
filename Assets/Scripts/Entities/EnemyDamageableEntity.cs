using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for enemies that can take damage and die, dropping souls and respawn.
/// </summary>
public class EnemyDamageableEntity : DamageableEntity {

    [Tooltip("Game object that has a component that implements the ISpawnPoint interface.")]
    public GameObject spawnPoint;
    public bool dropSoul;
    public bool permanentSoul;
    public ISpawnPoint spawn;

    void Start()
    {
        Refresh();
        InitSpawnPoint();
    }

    public virtual void Initialize()
    {

    }

    public override bool OnDamage(GameObject origin, float damage)
    {
        bool damaged = base.OnDamage(origin, damage);
        if (damaged)
        {
            gameObject.SendMessage("OnDamageApplied", SendMessageOptions.DontRequireReceiver);
        }
        return damaged;
    }

    protected override void OnDeath()
    {
        // drop soul if it has to
        if (dropSoul)
        {
            GameObject soul = SoulPool.instance.GetObject();
            soul.transform.position = transform.position;
            SoulDrop soulDrop = soul.GetComponent<SoulDrop>();
            // set the spawn point if any
            if (spawn != null)
            {
                soulDrop.Initialize(spawn);
            }
            // is it permanent?
            if (permanentSoul)
            {
                soulDrop.collectable = false;
                soulDrop.lifeSpan = 0;
            }
            soul.SetActive(true);
        } else if (spawn != null) // check if it should respawn
        {
            spawn.Spawn();
        }
        base.OnDeath();
    }

    private void InitSpawnPoint()
    {
        if (!spawnPoint) return;
        spawn = spawnPoint.GetComponent<SpawnPoint>();
    }

    public bool DropSoul
    {
        get { return dropSoul; }
        set { dropSoul = value; }
    }
}
