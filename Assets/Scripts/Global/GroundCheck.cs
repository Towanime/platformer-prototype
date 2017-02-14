using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

    public Transform groundCheckObject;
    public LayerMask groundLayer;
    public float groundRadius = 0.2f;
    private bool grounded = false;
    private bool calculated = false;

    private void Update()
    {
        calculated = false;
    }

    private void FixedUpdate()
    {
        calculated = false;
    }

    public bool IsGrounded
    {
        get {
            if (!calculated)
            {
                grounded = Physics.CheckSphere(groundCheckObject.position, groundRadius, groundLayer);
                calculated = true;
            }
            return grounded;
        }
    }
}
