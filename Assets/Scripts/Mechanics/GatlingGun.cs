using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingGun : MonoBehaviour {
    public bool isEnabled = true;
    public GameObject emitor;
    // delay between shots
    public float fireDelay;
    private SimplePlayerController controller;
    public CharacterMovement characterMovement;
    // cooldown vars
    private float currentCooldown;
    private bool wait;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<SimplePlayerController>();
    }

    void Update()
    {
        if (wait)
        {
            currentCooldown += Time.deltaTime;
            // turn off wait if the time is up
            if (currentCooldown >= fireDelay)
            {
                wait = false;
            }
        }
    }
    /// <summary>
    /// Fires a bullet
    /// </summary>
    public void Fire()
    {
        if (wait) return;
        GameObject bullet = BulletPool.instance.GetObject();
        // set bullet position and start moving?
        bullet.transform.position = emitor.transform.position;
        //bullet.transform.rotation = emitor.transform.rotation;
        bullet.GetComponent<Bullet>().SetDirection( (int) characterMovement.lastInputDirection);
        bullet.SetActive(true);
        // start cooldown
        wait = true;
        currentCooldown = 0;
    }
}
