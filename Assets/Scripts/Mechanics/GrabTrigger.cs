using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trigger used in the arm to know when it hits a grababble entity or when returns to the original thrown point (player's body).
/// </summary>
public class GrabTrigger : MonoBehaviour
{
    private Grab grabComponent;

    void Start()
    {
        grabComponent = GetComponentInParent<Grab>();
    }
    
    public void OnTriggerEnter(Collider other)
    {
        grabComponent.OnCollision(other);
    }

    public void OnTriggerExit(Collider other)
    {

    }
}
