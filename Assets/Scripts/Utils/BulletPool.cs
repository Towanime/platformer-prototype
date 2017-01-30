using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour {
    public static BulletPool instance;
    // prefab to pool
    public GameObject pooledPrefab;
    public int poolSize = 100;
    public bool dynamicSize = true;
    // list to hold references
    private List<GameObject> pool;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(pooledPrefab, gameObject.transform) as GameObject;
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    // get available object
    public GameObject GetObject()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }
        // nothing? try to see if you can create new instances
        if (dynamicSize)
        {
            GameObject obj = Instantiate(pooledPrefab) as GameObject;
            pool.Add(obj);
            return obj;
        }
        return null;
    }

    public void ReleaseObject(GameObject toRelease)
    {
        toRelease.SetActive(false);
    }
}
