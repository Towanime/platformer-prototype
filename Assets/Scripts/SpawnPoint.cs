using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour, ISpawnPoint {
    public GameObject enemyPrefab;
    public float spawnTime;
    public bool spawnOnAwake;
    private float currentSpawnTime;
    private bool waiting;


	// Use this for initialization
	void Start () {
        if (spawnOnAwake) waiting = true; // setup initial spawn
	}
	
	// Update is called once per frame
	void Update () {
        if (waiting)
        {
            currentSpawnTime += Time.deltaTime;
            // check if it's ready to create the enemy
            if(currentSpawnTime >= spawnTime)
            {
                CreateEnemy();
                waiting = false;
            }
        }
	}

    /// <summary>
    /// Will start the timer and other spawn conditions to check before creating the enemy on the level.
    /// </summary>
    /// <returns></returns>
    public bool Spawn()
    {
        if (waiting) return false;

        // start waiting
        waiting = true;
        currentSpawnTime = 0;

        return true;
    }

    /// <summary>
    /// Implement enemy creation here, it is called after the timer is resolved.
    /// </summary>
    private void CreateEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity) as GameObject;
        enemy.transform.position = transform.position;
        // setup spawn point
        EnemyDamageableEntity entity = enemy.GetComponent<EnemyDamageableEntity>();
        entity.spawn = this;
    }
}
