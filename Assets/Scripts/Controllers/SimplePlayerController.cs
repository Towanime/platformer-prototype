﻿using UnityEngine;
using System.Collections;

public class SimplePlayerController : MonoBehaviour {
    public PlayerInput playerInput;
    public CharacterMovement characterMovement;
    public bool isEnabled = true;
    // skills
    private Grab grabSkill;
    private GatlingGun gatlingGun;
    private Teleport teleport;

    // Use this for initialization
    void Start()
    {
        grabSkill = GetComponent<Grab>();
        gatlingGun = GetComponent<GatlingGun>();
        teleport = GetComponentInChildren<Teleport>();
    }

    void Update()
    {
        if (!isEnabled) return;

        // Detect grab
        if (playerInput.grabbed && grabSkill.CanAct())
        {
            // initiate grab and disable controller until is done
            if (grabSkill.IsHolding)
            {
                // destroys object
                grabSkill.Crush();
            }
            else
            {
                // if the grab initiates correctly then disable the controller!
                IsEnabled = !grabSkill.Begin((int)characterMovement.LastInputDirection);
            }
        }
        // Jump only if not firing the gun and not using the grab skill
        if (playerInput.jumped && !grabSkill.IsRunning && !gatlingGun.IsFiringGun)
        {
            characterMovement.Jump();
        }
        // Freeze movement if the player is in the air while firing the gun or using the grab skill
        if (teleport.IsTeleporting || (!characterMovement.IsGrounded && (gatlingGun.IsFiringGun || grabSkill.IsRunning)))
        {
            characterMovement.freezeMovement = true;
        } else
        {
            characterMovement.freezeMovement = false;
        }
        if (playerInput.teleported && teleport.HasTarget)
        {
            teleport.DoTeleport();
        }
    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        if (!isEnabled) return;
        
        if (playerInput.shooting) // shooting and throwing
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
