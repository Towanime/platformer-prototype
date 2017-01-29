using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableEntity : MonoBehaviour {
    public bool doDamage;
    public bool ignoreDamage = false;
    public int life;
    public GameObject spawnPoint;
    protected SpawnPoint spawn;
    protected int currentLife;

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

    public virtual void OnDamage(int damage)
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
        Destroy(gameObject);
        // check if it should respawn
        if (spawn)
        {
            spawn.Spawn();
        }
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

    public int Life
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
