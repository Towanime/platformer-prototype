using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathOnContact : MonoBehaviour
{
    public bool isEnabled;
    [Tooltip("Layers of objects that will kill this entity when colliding.")]
    public LayerMask contactLayer;

    void OnTriggerEnter(Collider other)
    {
        if (!isEnabled || !Util.IsObjectInLayerMask(contactLayer, other.gameObject)) return;

        EnemyDamageableEntity damageableEntity = gameObject.GetComponent<EnemyDamageableEntity>();
        if (damageableEntity != null)
        {
            damageableEntity.DropSoul = false;
        }
        gameObject.SendMessage("OnDeath");
    }
}