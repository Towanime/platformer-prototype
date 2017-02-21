using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : EnemyDamageableEntity
{
    public override bool OnDamage(GameObject origin, float damage)
    {
        bool damaged = base.OnDamage(origin, damage);
        if (damaged)
        {
            SoundManager.Instance.Play(SoundManager.Instance.lanternHitSound);
        }
        return damaged;
    }
}