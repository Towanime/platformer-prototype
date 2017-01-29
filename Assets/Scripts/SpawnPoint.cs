﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    public GameObject enemyPrefab;
    public float spawnTime;
    private float currentSpawnTime;
    private bool waiting;


	// Use this for initialization
	void Start () {
		
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

    public bool Spawn()
    {
        if (waiting) return false;

        // start waiting
        waiting = true;
        currentSpawnTime = 0;

        return true;
    }

    private void CreateEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity) as GameObject;
        enemy.transform.position = transform.position;
        // setup spawn point
        DamageableEntity entity = enemy.GetComponent<DamageableEntity>();
        entity.SetSpawnPoint(gameObject);
    }
}
