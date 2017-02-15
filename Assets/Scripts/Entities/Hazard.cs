using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for any component that applies a given damage to a GameObject.
/// </summary>
public class Hazard : MonoBehaviour {

    [Tooltip("If true, the hazard will do damage to the target. Use this instead of disabling the whole component.")]
    public bool isActive;
    [Tooltip("Damage to apply.")]
    public float damage;
    [Tooltip("Layers of the objects that can be damaged by this component.")]
    public LayerMask targetLayerMask;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Checks if the given GameObject is part of the targets of this component depending on its layer.
    /// </summary>
    /// <param name="obj">Object to apply damage to.</param>
    /// <returns>True if the layer of the object corresponds to the targetLayerMask</returns>
    private bool IsEntityTarget(GameObject obj)
    {
        return Util.IsObjectInLayerMask(targetLayerMask, obj);
    }

    /// <summary>
    /// Does damage to the given object only if the component is enabled 
    /// and the object is one of the targets of this component.
    /// </summary>
    /// <param name="obj">Object to apply damage to.</param>
    /// <returns>True if the object received any damage.</returns>
    protected bool DoDamage(GameObject obj)
    {
        // First check if the object's layer corresponds to the targetLayerMask
        if (isActive && IsEntityTarget(obj))
        {
            // maybe do othe checks here later
            // check if the object is damagable
            DamageableEntity entity = obj.GetComponent<DamageableEntity>();
            if (entity)
            {
                return entity.OnDamage(gameObject, damage);
            }
        }
        return false;
    }

    /// <summary>
    /// Turns on the hazard.
    /// </summary>
    public void ActivateHazard()
    {
        isActive = true;
    }

    public void DesactivateHazard()
    {
        isActive = false;
    }
}
