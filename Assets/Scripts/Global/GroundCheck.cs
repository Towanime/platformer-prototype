using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

    public Transform groundCheckObject;
    public LayerMask groundLayer;
    public float groundRadius = 0.2f;

    public bool IsGrounded
    {
        get { return Physics.CheckSphere(groundCheckObject.position, groundRadius, groundLayer); }
    }
}
