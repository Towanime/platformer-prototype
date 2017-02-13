using UnityEngine;
using System.Collections;

public class SimplePlayerController : MonoBehaviour {
    public PlayerInput playerInput;
    public bool isEnabled = true;
    public GroundCheck groundCheck;
    public AimingDirectionResolver aimingDirectionResolver;
    [Tooltip("Delay after an enemy is thrown for when the player can begin shooting.")]
    public float throwEnemyDelay = 0.2f;
    private float throwEnemyTimer;
    private bool isInThrowEnemyDelay;
    // Mechanics
    public CharacterMovement characterMovement;
    public Grab grabSkill;
    public GatlingGun gatlingGun;
    public Teleport teleport;

    void Update()
    {
        if (!isEnabled) return;

        HandleGrab();
        HandleCrush();
        bool grounded = groundCheck.IsGrounded;
        HandleThrow(grounded);
        bool jumped = HandleJump();
        HandleTeleport(grounded, jumped);
        characterMovement.freezeMovement = IsMovementFrozen(grounded);
    }

    private void HandleGrab()
    {
        // Detect grab
        if (playerInput.grabbed && grabSkill.CanAct() && !grabSkill.IsHolding)
        {
            // initiate grab and disable controller until is done
            // if the grab initiates correctly then disable the controller!
            IsEnabled = !grabSkill.Begin();
        }
    }

    private void HandleCrush()
    {
        // Detect crush
        if (playerInput.crushed && grabSkill.CanAct() && grabSkill.IsHolding)
        {
            // destroys object
            grabSkill.Crush();
        }
    }

    private bool HandleJump()
    {
        bool jumped = false;
        if (playerInput.jumped && CanJump())
        {
            bool canJumpInAir = teleport.IsFloating;
            jumped = characterMovement.Jump(canJumpInAir);
        }
        return jumped;
    }

    private void HandleTeleport(bool grounded, bool jumped)
    {
        if (teleport.IsFloating && IsStopFloating(grounded, jumped))
        {
            teleport.IsFloating = false;
        }
        if (playerInput.teleported && teleport.HasTarget)
        {
            teleport.DoTeleport();
        }
    }

    private void HandleShooting()
    {
        if (playerInput.shooting && CanShoot())
        {
            bool grounded = groundCheck.IsGrounded;
            float aimingAngle = aimingDirectionResolver.GetAimingAngle(grounded);
            gatlingGun.Fire(aimingAngle);
        }
    }

    private void HandleThrow(bool grounded)
    {
        if (isInThrowEnemyDelay)
        {
            throwEnemyTimer += Time.deltaTime;
            if (throwEnemyTimer >= throwEnemyDelay)
            {
                isInThrowEnemyDelay = false;
            }
        }
        if (playerInput.threw && CanThrowEnemy())
        {
            Vector2 aimingDirection = aimingDirectionResolver.GetAimingDirection(grounded);
            Vector3 throwOriginPosition = aimingDirectionResolver.GetAimEmitorPosition(aimingDirection);
            // If the throw happened start a timer so that the character has to wait before shooting again
            bool threwEnemy = grabSkill.ThrowEnemy(aimingDirection, throwOriginPosition);
            if (threwEnemy)
            {
                throwEnemyTimer = 0;
                isInThrowEnemyDelay = true;
            }
        }
    }

    private bool CanJump()
    {
        // Can jump if the player is not running, shooting or teleporting
        return !grabSkill.IsRunning && !gatlingGun.IsFiringGun && !teleport.IsTeleporting;
    }

    private bool CanThrowEnemy()
    {
        return grabSkill.IsHolding;
    }

    private bool CanShoot()
    {
        return !grabSkill.IsHolding && !isInThrowEnemyDelay;
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

        HandleShooting();
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
