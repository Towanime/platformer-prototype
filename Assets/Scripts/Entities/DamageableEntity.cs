using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for entities that have a life and can take on damage.
/// </summary>
public class DamageableEntity : MonoBehaviour
{
    public bool ignoreDamage = false;
    public float life;
    protected float currentLife;

    void Start()
    {
        currentLife = life;
    }

    public virtual bool OnDamage(GameObject origin, float damage)
    {
        if (ignoreDamage) return false;
        ModifyCurrentLife(damage);
        if (currentLife <= 0)
        {
            OnDeath();
        }
        return true;
    }

    protected virtual void ModifyCurrentLife(float damage)
    {
        currentLife = Mathf.Max(currentLife - damage, 0);
    }

    protected virtual void OnDeath()
    {
        Destroy(gameObject);
    }

    public virtual void Refresh()
    {
        currentLife = life;
    }

    public float CurrentLife
    {
        get { return currentLife; }
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
