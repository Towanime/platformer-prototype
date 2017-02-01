using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingGun : MonoBehaviour {
    public CharacterMovement characterMovement;
    [Tooltip("Object from where the bullets will spawn. Recomended an empty object with no collisions.")]
    public GameObject emitor;
    // 
    [Tooltip("Delay between bullets.")]
    public float fireRate;
    [Tooltip("Optional bullet speed, this will override the speed on the bullet prefab if overrideBulletSpeed is set to true.")]
    public float bulletSpeed;
    [Tooltip("If true it will override the bullet's prefab speed with the one on this component..")]
    public bool overrideBulletSpeed;
    private SimplePlayerController controller;
    public bool isEnabled = true;
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
            if (currentCooldown >= fireRate)
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
        if (!isEnabled || wait) return;
        GameObject bullet = BulletPool.instance.GetObject();
        // set bullet position and start moving?
        bullet.transform.position = emitor.transform.position;
        // where is it aiming?
        //bullet.transform.rotation = emitor.transform.rotation;
        Bullet component = bullet.GetComponent<Bullet>();
        component.SetDirection( (int) characterMovement.lastInputDirection);
        if (overrideBulletSpeed)
        {
            component.speed = bulletSpeed;
        }
        bullet.SetActive(true);
        // start cooldown
        wait = true;
        currentCooldown = 0;
    }
}
