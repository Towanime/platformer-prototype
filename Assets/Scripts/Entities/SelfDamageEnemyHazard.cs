using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Does hazard damage to target and then damages the game object that this component is attached to.
/// </summary>
public class SelfDamageEnemyHazard : Hazard
{
    [Tooltip("Should this component drop a soul?")]
    public bool dropSoul = false;

    void OnTriggerStay(Collider other)
    {
        EnemyDamageableEntity selfHarm = null;
        if (DoDamage(other.gameObject) && (selfHarm = GetComponent<EnemyDamageableEntity>()))
        {
            selfHarm.DropSoul = dropSoul;
            selfHarm.OnDamage(gameObject, damage);
        }
    }
}
