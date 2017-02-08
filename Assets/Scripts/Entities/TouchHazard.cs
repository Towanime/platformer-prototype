using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHazard : Hazard {

    void OnTriggerStay(Collider other)
    {
        DoDamage(other.gameObject);
    }
}
