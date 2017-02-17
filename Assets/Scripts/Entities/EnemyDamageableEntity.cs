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
            gameObject.SendMessage("OnDamageApplied");
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
            // set the spawn point if any
            if (spawn != null)
            {
                SoulDrop soulDrop = soul.GetComponent<SoulDrop>();
                soulDrop.Initialize(spawn);
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
