using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trigger attached in the arm to know when it hits a grababble entity. It notifies the grab component on the player.
/// </summary>
public class GrabTrigger : MonoBehaviour
{
    public LayerMask layersToIgnore;
    private Grab grabComponent;

    void Start()
    {
        grabComponent = GetComponentInParent<Grab>();
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (Util.IsObjectInLayerMask(layersToIgnore, other.gameObject)) return;
        grabComponent.OnCollision(other);
    }

    public void OnTriggerExit(Collider other)
    {

    }
}
