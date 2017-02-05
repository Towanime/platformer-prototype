using UnityEngine;
using System.Collections;

public class SimplePlayerController : MonoBehaviour {
    public PlayerInput playerInput;
    public bool isEnabled = true;
    public GroundCheck groundCheck;
    public AimingDirectionResolver aimingDirectionResolver;
    private Vector2 tmp;
    // Mechanics
    public CharacterMovement characterMovement;
    public Grab grabSkill;
    public GatlingGun gatlingGun;
    public Teleport teleport;

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
                IsEnabled = !grabSkill.Begin((int)characterMovement.FacingDirection);
            }
        }
        bool jumped = false;
        bool grounded = groundCheck.IsGrounded;
        if (playerInput.jumped && CanJump())
        {
            bool canJumpInAir = teleport.IsFloating;
            jumped = characterMovement.Jump(canJumpInAir);
        }
        if (teleport.IsFloating && IsStopFloating(grounded, jumped))
        {
            teleport.IsFloating = false;
        }
        characterMovement.freezeMovement = IsMovementFrozen(grounded);
        if (playerInput.teleported && teleport.HasTarget)
        {
            teleport.DoTeleport();
        }
    }

    private bool CanJump()
    {
        // Can jump if the player is not running, shooting or teleporting
        return !grabSkill.IsRunning && !gatlingGun.IsFiringGun && !teleport.IsTeleporting;
    }

    private bool IsStopFloating(bool grounded, bool jumped)
    {
        // Float stops if the player is on the ground, shooting, grabbing or has jumped
        return grounded || gatlingGun.IsFiringGun || grabSkill.IsRunning || jumped;
    }

    private bool IsMovementFrozen(bool grounded)
    {
        // Movement is frozen if the player is teleporting, floating, or is in the air while shooting or grabbing
        return teleport.IsTeleporting || teleport.IsFloating || (!grounded && (gatlingGun.IsFiringGun || grabSkill.IsRunning));
    }

    void FixedUpdate()
    {
        if (!isEnabled) return;
        
        if (playerInput.shooting)
        {
            bool grounded = groundCheck.IsGrounded;
            gatlingGun.Fire(aimingDirectionResolver.GetAimingAngle(grounded));
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
