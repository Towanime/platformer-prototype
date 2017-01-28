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

    public void OnCollisionEnter(Collision collision)
    {

    }

    public void OnCollisionExit2D(Collision2D collision)
    {

    }

    public void OnCollisionExit(Collision collision)
    {

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

    }

    public void OnTriggerEnter(Collider other)
    {

    }

    public void OnTriggerExit2D(Collider2D collision)
    {

    }

    public void OnTriggerExit(Collider other)
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
