using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hazard that does damage to another object as long as it's inside the trigger.
/// </summary>
public class TouchHazard : Hazard {

    void OnTriggerStay(Collider other)
    {
        DoDamage(other.gameObject);
    }
}
