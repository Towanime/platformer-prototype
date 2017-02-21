using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatHead : EnemyDamageableEntity
{
    public override bool OnDamage(GameObject origin, float damage)
    {
        bool damaged = base.OnDamage(origin, damage);
        if (damaged)
        {
            SoundManager.Instance.PlayRandom(SoundManager.Instance.catPainSounds);
        }
        return damaged;
    }

    protected override void OnDeath()
    {
        SoundManager.Instance.Play(SoundManager.Instance.catDeathSound);
        base.OnDeath();
    }
}
