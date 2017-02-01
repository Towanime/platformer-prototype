using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GatlingGun : MonoBehaviour {
    public CharacterMovement characterMovement;
    [Tooltip("Object from where the bullets will spawn. Recomended an empty object with no collisions.")]
    public GameObject emitor;
    // 
    [Tooltip("Delay between bullets.")]
    public float fireRate = 0.05f;
    [Tooltip("Optional bullet speed, this will override the speed on the bullet prefab if overrideBulletSpeed is set to true.")]
    public float bulletSpeed;
    [Tooltip("If true it will override the bullet's prefab speed with the one on this component.")]
    public bool overrideBulletSpeed;
    [Tooltip("Time in seconds of continuous firing until the gun has to cool down, use this for tuning.")]
    public float overheatLimit = 5;
    [Tooltip("Multiplier for how fast the overheat should recover when not shooting. Ex: if overheatLimit is 6s, a value of 2 will make it recover in 3s.")]
    public float recoverRate = 2;
    public bool isEnabled = true;
    [Tooltip("Temporal way to show the overheat.")]
    public Text lblTemp;
    private SimplePlayerController controller;
    // cooldown vars
    private float currentCooldown;
    private bool wait;
    public float currentOverheat;
    // did it got overheated?
    private bool isFiringGun = false;
    private bool isOverheated;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<SimplePlayerController>();
    }

    void FixedUpdate()
    {
        if (wait)
        {
            currentCooldown += Time.fixedDeltaTime;
            // turn off wait if the time is up
            if (currentCooldown >= fireRate)
            {
                wait = false;
            }
        }
        // Update the overheat counter, go up if firing the gun and down if not firing it.
        float overheatModifier = Time.fixedDeltaTime * (isFiringGun ? 1 : -recoverRate);
        currentOverheat = Mathf.Clamp(currentOverheat + overheatModifier, 0, overheatLimit);
        // Update the isOverheated value
        if (currentOverheat <= 0) isOverheated = false;
        if (currentOverheat >= overheatLimit) isOverheated = true;
        UpdateLabel();
        isFiringGun = false;
    }

    /// <summary>
    /// Fires a bullet
    /// </summary>
    public void Fire()
    {
        if (!isEnabled || isOverheated) return;
        isFiringGun = true;
        if (!wait)
        {
            GameObject bullet = BulletPool.instance.GetObject();
            // set bullet position and start moving?
            bullet.transform.position = emitor.transform.position;
            // where is it aiming?
            //bullet.transform.rotation = emitor.transform.rotation;
            Bullet component = bullet.GetComponent<Bullet>();
            component.SetDirection((int)characterMovement.LastInputDirection);
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

    private void UpdateLabel()
    {
        lblTemp.text = currentOverheat + "/" + overheatLimit;
    }

    public bool IsOverheated
    {
        get
        {
            return this.isOverheated;
        }
    }

    public bool IsFiringGun
    {
        get
        {
            return this.isFiringGun;
        }
    }
}
