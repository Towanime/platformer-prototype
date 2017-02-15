using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pickup : MonoBehaviour {

    /**
     * List to hold activators to call when this switch is "activated".
     * */
    public List<BaseActivator> activators;
    public LayerMask target;

    public void OnTriggerEnter(Collider other)
    {
        if (Util.IsObjectInLayerMask(target, other.gameObject))
        {
            foreach (BaseActivator activator in activators)
            {
                activator.Activate(gameObject);
            }
            Destroy(gameObject);
        }
    }
}
