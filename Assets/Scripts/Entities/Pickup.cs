using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pickup : MonoBehaviour {

    public LayerMask target;

    public void OnTriggerEnter(Collider other)
    {
        if (Util.IsObjectInLayerMask(target, other.gameObject))
        {
            Destroy(gameObject);
        }
    }
}
