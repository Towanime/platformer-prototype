using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour {

    public float damage;
    public LayerMask targetLayerMask;

    protected bool IsEntityTarget(GameObject obj)
    {
        return Utils.IsObjectInLayerMask(targetLayerMask, obj);
    }

    protected bool DoDamage(GameObject obj)
    {
        if (IsEntityTarget(obj))
        {
            // maybe do othe checks here later
            // check if the object is damagable
            DamageableEntity entity = obj.GetComponent<DamageableEntity>();
            if (entity)
            {
                entity.OnDamage(gameObject, damage);
                return true;
            }
        }
        return false;
    }
}
