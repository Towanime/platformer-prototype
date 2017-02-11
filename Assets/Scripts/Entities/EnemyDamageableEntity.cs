using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for enemies that can take damage and die, dropping souls and respawn.
/// </summary>
public class EnemyDamageableEntity : DamageableEntity {

    public GameObject spawnPoint;
    public bool dropSoul;
    protected SpawnPoint spawn;

    void Start()
    {
        Refresh();
        InitSpawnPoint();
    }

    public virtual void Initialize()
    {

    }

    protected override void OnDeath()
    {
        // drop soul if it has to
        if (dropSoul)
        {
            GameObject soul = SoulPool.instance.GetObject();
            soul.transform.position = transform.position;
            // set the spawn point if any
            if (spawn)
            {
                SoulDrop soulDrop = soul.GetComponent<SoulDrop>();
                soulDrop.Initialize(spawn);
            }
            soul.SetActive(true);
        } else if (spawn) // check if it should respawn
        {
            spawn.Spawn();
        }
        base.OnDeath();
    }

    public void SetSpawnPoint(GameObject gameobject)
    {
        spawnPoint = gameobject;
        InitSpawnPoint();
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
