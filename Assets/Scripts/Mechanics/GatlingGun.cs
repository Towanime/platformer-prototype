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
    public float fireRate;
    [Tooltip("Optional bullet speed, this will override the speed on the bullet prefab if overrideBulletSpeed is set to true.")]
    public float bulletSpeed;
    [Tooltip("If true it will override the bullet's prefab speed with the one on this component.")]
    public bool overrideBulletSpeed;
    [Tooltip("Number of bullets until the gun has to cool down, use this and overheat rate for tuning.")]
    public float overheatLimit = 150;
    [Tooltip("Multiplier for the overheat gauge, this value will increase the overheat N times per bullet.")]
    public float overheatRate = 6;
    [Tooltip("Recover rate for the gun when not shooting.")]
    public float recoverRate = 2;
    public bool isEnabled = true;
    [Tooltip("Temporal way to show the overheat.")]
    public Text lblTemp;
    private SimplePlayerController controller;
    // cooldown vars
    private float currentCooldown;
    private bool wait;
    private float currentOverheat;
    // did it got overheated?
    private bool isOverheated;

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
        else
        {
            // if it has overheat then recover
            if(currentOverheat >= 0)
            {
                // start recovering if the gun is in no use or waiting to be used again
                currentOverheat = Mathf.Clamp(currentOverheat - (overheatRate * recoverRate), 0, overheatLimit);
                // got cooled down?
                if (currentOverheat <= 0) isOverheated = false;
                UpdateLabel();
            }
        }
    }
    /// <summary>
    /// Fires a bullet
    /// </summary>
    public void Fire()
    {
        if (!isEnabled || isOverheated || wait) return;
        GameObject bullet = BulletPool.instance.GetObject();
        // set bullet position and start moving?
        bullet.transform.position = emitor.transform.position;
        // where is it aiming?
        //bullet.transform.rotation = emitor.transform.rotation;
        Bullet component = bullet.GetComponent<Bullet>();
        component.SetDirection( (int) characterMovement.LastInputDirection);
        if (overrideBulletSpeed)
        {
            component.speed = bulletSpeed;
        }
        bullet.SetActive(true);
        // start cooldown
        wait = true;
        currentCooldown = 0;
        // increate overheat!
        currentOverheat += overheatRate;
        // is overheated?
        if (currentOverheat >= overheatLimit) isOverheated = true;
        UpdateLabel();
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
}
