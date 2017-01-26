using UnityEngine;
using System.Collections;

public class SimplePlayerController : MonoBehaviour {
    public float speed;             //Floating point variable to store the player's movement speed.
    public float maxSpeed;
    private bool facingRight = true;
    private Rigidbody2D rigidbody;       //Store a reference to the Rigidbody2D component required to use 2D Physics.
    public bool isEnabled = true;
    // skills
    private Grab grabSkill;

    // Use this for initialization
    void Start()
    {
        //Get and store a reference to the Rigidbody2D component so that we can access it.
        rigidbody = GetComponent<Rigidbody2D>();
        grabSkill = GetComponent<Grab>();
    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        if (!isEnabled) return;

        // grab?
        bool grab = Input.GetButton("Fire1");
        if (grab)
        {
            // initiate grab and disable controller until is done
            grabSkill.Begin(facingRight?1:-1);
            IsEnabled = false;
        }

        float move = Input.GetAxis("Horizontal");

        rigidbody.velocity = new Vector2(move * maxSpeed, rigidbody.velocity.y);

        if (move > 0 && !facingRight)
        {
            Flip();
        } else if (move < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
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
