using UnityEngine;
using System.Collections;

public class SimplePlayerController : MonoBehaviour {
    public PlayerInput playerInput;
    public CharacterMovement characterMovement;
    public bool isEnabled = true;
    // skills
    private Grab grabSkill;
    private GatlingGun gatlingGun;

    // Use this for initialization
    void Start()
    {
        grabSkill = GetComponent<Grab>();
        gatlingGun = GetComponent<GatlingGun>();
    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        if (!isEnabled) return;

        // grab or fire?
        if (playerInput.grabbing && grabSkill.CanAct())
        {
            // initiate grab and disable controller until is done
            if (grabSkill.IsHolding)
            {
                // destroys object
                grabSkill.Crush();
            }else
            {
                // if the grab initiates correctly then disable the controller!
                IsEnabled = !grabSkill.Begin((int)characterMovement.lastInputDirection);
            }
        } else if (playerInput.shooting) // shooting and throwing
        {
            gatlingGun.Fire();
        }
    }

    public bool IsEnabled
    {
        set
        {
            this.isEnabled = value;
            this.characterMovement.processInput = value;
        }
        get
        {
            return this.isEnabled;
        }
    }
}
