using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableEntity : MonoBehaviour {
    public bool doDamage;
    public bool ignoreDamage = false;
    public float life;
    public GameObject spawnPoint;
    public bool dropSoul;
    protected SpawnPoint spawn;
    protected float currentLife;

    void Start()
    {
        InitSpawnPoint();
    }

    void Update()
    {
        // to test spawn
        if (doDamage)
        {
            OnDamage(10);
            doDamage = false;
        }
    }

    public virtual void OnDamage(float damage)
    {
        if (ignoreDamage) return;
        currentLife -= damage;
        if (currentLife <= 0)
        {
            OnDeath();
        }
    }

    protected virtual void OnDeath()
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
        Destroy(gameObject);
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

    public virtual void Refresh()
    {
        currentLife = life;
    }

    public float Life
    {
        get
        {
            return life;
        }
        set
        {
            life = value;
            currentLife = value;
        }
    }
}
