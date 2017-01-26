using UnityEngine;
using System.Collections;

public class Grab : MonoBehaviour {
    public float grabDistance = 3;
    public float throwSpeed = 5;
    // arm needs a rigid body and colission
    public GameObject arm;
    public bool isEnabled = true;
    // 
    private bool checkDistance;
    private float initialPositionX;
    private float finalPositionX;
    private int throwDirection;
    private SimplePlayerController controller;

	// Use this for initialization
	void Start () {
        controller = GetComponent<SimplePlayerController>();
	}
	
	void FixedUpdate () {
        if (!isEnabled || !checkDistance) return;

        // check distance manually 
        if (checkDistance && arm.transform.position.x >= finalPositionX)
        {
            // return with penalty
            Rigidbody2D rb = arm.GetComponent<Rigidbody2D>();

            rb.velocity = new Vector2((throwDirection * -1) * throwSpeed, rb.velocity.y);
            arm.GetComponent<BoxCollider2D>().enabled = false;
        }
	}

    public void Begin(int direction)
    {
        checkDistance = true;
        throwDirection = direction;
        initialPositionX = arm.transform.position.x;
        finalPositionX = initialPositionX + grabDistance;
        Rigidbody2D rb = arm.GetComponent<Rigidbody2D>();
        arm.GetComponent<BoxCollider2D>().enabled = true;

        rb.velocity = new Vector2(direction * throwSpeed, rb.velocity.y);
    }

    public bool IsEnabled
    {
        set
        {
            this.isEnabled = value;
        }
        get
        {
            return this.isEnabled;
        }
    }
}
