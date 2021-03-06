﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GatlingGun : MonoBehaviour {
    [Tooltip("Object from where the bullets will spawn. Recomended an empty object with no collisions.")]
    public GameObject emitor;
    [Tooltip("Offset in degrees used to randomize the direction of the bullet. Ex: if the bullet is going right and the value is 5, the angle will be randomized in a range of -5 to 5.")]
    public float bulletAngleRandomOffset = 1.5f;
    [Tooltip("Offset in coordinates used to randomize the spawn point of the bullet.")]
    public float bulletSpawnPositionRandomOffset = 0.15f;
    [Tooltip("Delay between bullets.")]
    public float fireRate = 0.05f;
    [Tooltip("Optional bullet speed, this will override the speed on the bullet prefab if overrideBulletSpeed is set to true.")]
    public float bulletSpeed = 15f;
    [Tooltip("If true it will override the bullet's prefab speed with the one on this component.")]
    public bool overrideBulletSpeed = true;
    [Tooltip("Time in seconds of continuous firing until the gun has to cool down, use this for tuning.")]
    public float overheatLimit = 5;
    [Tooltip("Multiplier for how fast the overheat should recover when not shooting. Ex: if overheatLimit is 6s, a value of 2 will make it recover in 3s.")]
    public float recoverRate = 2;
    public bool isEnabled = true;
    [Tooltip("Object that contains the physical gun.")]
    public GameObject gatlingGunObject;
    [Tooltip("At which point of the overheat state will the laugh sound be played. 0 means the sound won't play and 1 full overheat.")]
    [Range(0.0f, 1.0f)]
    public float overheatLaughSoundPosition = 0.8f;
    [Tooltip("At which point of the overheat state will the kettle boiling sound be played. 0 means the sound won't play and 1 full overheat.")]
    [Range(0.0f, 1.0f)]
    public float overheatKettleSoundPosition = 0.7f;
    // cooldown vars
    private float currentCooldown;
    private bool wait;
    private float currentOverheat;
    // did it got overheated?
    private bool isFiringGun = false;
    private bool lastFiringGun = false;
    private bool isOverheated;
    private Vector2 tmp;

    void Start()
    {
        SetGunEnabled(isEnabled);
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
        float newOverheat = Mathf.Clamp(currentOverheat + overheatModifier, 0, overheatLimit);
        // Update the isOverheated value
        if (newOverheat <= 0)
        {
            isOverheated = false;
            OnOverheatChange(isOverheated);
        }
        if (newOverheat >= overheatLimit)
        {
            isOverheated = true;
            OnOverheatChange(isOverheated);
        }
        PlayOverheatSound(currentOverheat, newOverheat, overheatLaughSoundPosition, SoundManager.Instance.overheatLaughSound);
        PlayOverheatSound(currentOverheat, newOverheat, overheatKettleSoundPosition, SoundManager.Instance.overheatKettleSound);
        currentOverheat = newOverheat;
        lastFiringGun = isFiringGun;
        isFiringGun = false;
    }

    void OnOverheatChange(bool isOverheated)
    {
        if (isOverheated)
        {
            SoundManager.Instance.Play(SoundManager.Instance.overheatCoolDownSound);
            SoundManager.Instance.Play(SoundManager.Instance.reachedOverheatSound);
        } else
        {
            SoundManager.Instance.Stop(SoundManager.Instance.overheatCoolDownSound);
            SoundManager.Instance.Stop(SoundManager.Instance.reachedOverheatSound);
            SoundManager.Instance.Stop(SoundManager.Instance.overheatLaughSound);
            SoundManager.Instance.Stop(SoundManager.Instance.overheatKettleSound);
        }
    }

    void PlayOverheatSound(float currentOverheat, float newOverheat, float soundPosition, AudioClipInfo audioClipInfo)
    {
        if (soundPosition == 0) return;
        float currentOverheatPercentage = currentOverheat / overheatLimit;
        float newOverheatPercentage = newOverheat / overheatLimit;
        if (isOverheated && currentOverheatPercentage >= soundPosition && newOverheatPercentage <= soundPosition)
        {
            SoundManager.Instance.Play(audioClipInfo);
        }
    }

    /// <summary>
    /// Fires a bullet
    /// </summary>
    public bool Fire(Vector3 bulletOrigin, float aimingAngle)
    {
        if (!isEnabled || isOverheated) return false;
        isFiringGun = true;
        if (!wait)
        {
            GameObject bullet = BulletPool.instance.GetObject();
            // clear bullet trail
            bullet.GetComponent<TrailRenderer>().Clear();
            bullet.transform.position = GetBulletSpawnPosition(bulletOrigin, aimingAngle);
            Bullet component = bullet.GetComponent<Bullet>();
            component.SetDirection(GetBulletDirection(aimingAngle));
            if (overrideBulletSpeed)
            {
                component.speed = bulletSpeed;
            }
            bullet.SetActive(true);
            // start cooldown
            wait = true;
            currentCooldown = 0;
            SoundManager.Instance.Play(SoundManager.Instance.bulletSpawnSound);
            return true;
        }
        return false;
    }

    private Vector3 GetBulletSpawnPosition(Vector3 bulletOrigin, float aimingAngle)
    {
        Vector3 spawnPosition = bulletOrigin;
        // Randomize position of the bullet by a position offset;
        float randomOffset = Random.Range(-bulletSpawnPositionRandomOffset, bulletSpawnPositionRandomOffset);
        // Add offset but first convert it to the angle of the spawn point
        float cos = Mathf.Cos(aimingAngle * Mathf.Deg2Rad);
        float sin = Mathf.Sin(aimingAngle * Mathf.Deg2Rad);
        float tx = 0;
        float ty = randomOffset;
        tmp.x = cos * tx - sin * ty;
        tmp.y = sin * tx + cos * ty;
        spawnPosition.x += tmp.x;
        spawnPosition.y += tmp.y;
        spawnPosition.z = 0;
        return spawnPosition;
    }

    private Vector2 GetBulletDirection(float aimingAngle)
    {
        // Randomize direction of the bullet by an offset;
        float angle = aimingAngle;
        angle += Random.Range(-bulletAngleRandomOffset, bulletAngleRandomOffset);
        tmp.x = Mathf.Cos(angle * Mathf.Deg2Rad);
        tmp.y = Mathf.Sin(angle * Mathf.Deg2Rad);
        return tmp;
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
            return this.lastFiringGun;
        }
    }

    public bool IsEnabled
    {
        get {
            return isEnabled;
        }
        set {
            isEnabled = value;
            // show/hide gun
            SetGunEnabled(isEnabled);
        }
    }

    /// <summary>
    /// Enables the object that holds the renderer for the gun
    /// </summary>
    /// <param name="enabled"></param>
    public void SetGunEnabled(bool enabled)
    {
        gatlingGunObject.SetActive(enabled);
    }
}
