﻿using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {
    public PlayerInput playerInput;
    public bool processInput = true;
    public bool freezeMovement = false;
    public Animator animator;
    private CharacterController characterController;
    private Vector2 tmpVector2 = Vector2.zero;
    private Vector3 tmpVector3 = Vector3.zero;

    // Horizontal movement variables
    public float maxGroundSpeed = 0.15f;
    public float groundAcceleration = 0.5f;
    public float groundFriction = 1.2f;
    public float airFriction = 0.2f;
    /// <summary>
    /// Used when moving in one direction while pressing the other
    /// </summary>
    [Tooltip("Used when moving in one direction while pressing the other")]
    public float reactivityPercent = 0.5f;
    /// <summary>
    /// How much time the rotation will take until the character 
    /// faces the same direction that is being pressed
    /// </summary>
    [Tooltip("How much time the rotation will take until the character faces the same direction that is being pressed")]
    public float timeToFlipFacingDirection = 0.2f;
    private float currentHorizontalSpeed = 0f;
    /// <summary>
    /// Direction in which the character is moving (-1 = left, 1 = right).
    /// </summary>
    private float movingDirection = 1f;
    /// <summary>
    /// Direction that the player was pressing in the last frame (-1 = left, 1 = right). 
    /// If the player is not pressing any direction then the last direction pressed is retained.
    /// </summary>
    private float facingDirection = 1f;

    // Rotation variables
    private Quaternion fromRotation;
    private Quaternion toRotation;
    private Quaternion originalRotation;
    private float currentRotationTime = 0f;
    /// <summary>
    /// True if the character's rotation is being adjusted to 
    /// face the same direction that the player is pressing.
    /// </summary>
    private bool rotating = false;

    // Vertical movement variables
    /// <summary>
    /// Max speed that the character can have when falling down, 
    /// represented with positive numbers.
    /// </summary>
    [Tooltip("Max speed that the character can have when falling down, represented with positive numbers.")]
    public float maxFallingSpeed = 0.3f;
    /// <summary>
    /// One time modified given to the vertical 
    /// speed when the jump button is pressed.
    /// </summary>
    [Tooltip("One time modified given to the vertical speed when the jump button is pressed.")]
    public float jumpImpulse = 0.35f;
    public float gravity = 1.4f;
    /// <summary>
    /// Modifier applied to gravity when the character has jumped and 
    /// is ascending, used to make gravity lower so the character spends 
    /// more time going up than going down.
    /// </summary>
    [Tooltip("Modifier applied to gravity when the character has jumped and is ascending, used to make gravity lower so the character spends more time going up than going down.")]
    public float gravityAscensionModifier = 0.7f;
    /// <summary>
    /// Minimum time that the character has to be in the air after jumping, 
    /// used to set a limit for short jumps.
    /// </summary>
    [Tooltip("Minimum time that the character has to be in the air after jumping, used to set a limit for short jumps.")]
    public float minJumpTime = 0.1f;
    /// <summary>
    /// Maximum vertical speed that the player can have when the jump button is 
    /// not being held anymore, only applied after the time in minJumpTime has passed.
    /// </summary>
    [Tooltip("Maximum vertical speed that the player can have when the jump button is not being held anymore, only applied after the time in minJumpTime has passed.")]
    public float cutJumpSpeedLimit = 0.05f;
    /// <summary>
    /// Time given for a character to still be able to jump after 
    /// it has fallen of a ledge.
    /// </summary>
    [Tooltip("Time given for a character to still be able to jump after it has fallen of a ledge.")]
    public float jumpCallTolerance = 0.2f;
    private bool grounded;
    private float currentVerticalSpeed = 0f;
    private float timeInTheAir = 0f;
    private float timeSinceJumpStarted = 0f;
    private bool cutJumpShort = false;
    private bool characterJustJumped;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalRotation = transform.localRotation;
    }
    
    void FixedUpdate()
    {
        // Either -1, 0 or 1
        float horizontalInput = GetHorizontalInput();
        // Either -1 or 1, see lastInputDirection for details.
        float currentInputDirection = facingDirection;
        if (horizontalInput != 0)
        {
            currentInputDirection = horizontalInput;
        }
        grounded = IsGroundedInternal();
        UpdateSpeed(grounded, horizontalInput, currentInputDirection);
        if (freezeMovement)
        {
            currentHorizontalSpeed = 0;
            currentVerticalSpeed = 0;
        }
        UpdatePosition();
        UpdateRotation(currentInputDirection);
        if (currentHorizontalSpeed != 0)
        {
            movingDirection = Mathf.Sign(currentHorizontalSpeed);
        }
        facingDirection = currentInputDirection;
    }

    private bool IsGroundedInternal()
    {
        return characterController.isGrounded;
    }

    private float GetHorizontalInput()
    {
        // Either -1, 0 or 1
        return (processInput) ? playerInput.horizontalDirection : 0;
    }

    public void Jump()
    {
        bool canJump = grounded || timeInTheAir <= jumpCallTolerance;
        if (processInput && canJump)
        {
            cutJumpShort = false;
            characterJustJumped = true;
        }
    }

    void Update()
    {
        grounded = IsGroundedInternal();
        animator.SetBool("IsJumping", !grounded);
        // Do jump detection in Update() loop because its less likely to miss inputs than FixedUpdate()
        timeInTheAir = (grounded) ? 0 : timeInTheAir + Time.deltaTime;
    }

    private void UpdateSpeed(bool grounded, float horizontalInput, float currentInputDirection)
    {
        UpdateHorizontalSpeed(grounded, horizontalInput, currentInputDirection);
        UpdateVerticalSpeed(grounded);
    }

    private void UpdateHorizontalSpeed(bool grounded, float horizontalInput, float currentInputDirection)
    {
        bool pressingDirection = horizontalInput != 0;
        bool moving = currentHorizontalSpeed != 0;
        animator.SetBool("IsRunning", moving);
        if (moving && !pressingDirection)
        {
            float friction = (grounded) ? groundFriction : airFriction;
            currentHorizontalSpeed = currentHorizontalSpeed + friction * Time.fixedDeltaTime * movingDirection * -1;
            // If no direction is being pressed then clamp the speed to 0 once reached.
            if (movingDirection > 0)
            {
                currentHorizontalSpeed = Mathf.Max(0, currentHorizontalSpeed);
            }
            else
            {
                currentHorizontalSpeed = Mathf.Min(0, currentHorizontalSpeed);
            }
        }
        else if (pressingDirection)
        {
            bool movingInSameDirectionBeingPressed = movingDirection == currentInputDirection;
            if (movingInSameDirectionBeingPressed)
            {
                // Regular speed formula
                currentHorizontalSpeed = currentHorizontalSpeed + groundAcceleration * Time.fixedDeltaTime * currentInputDirection;
            }
            else
            {
                // If the direction being pressed is different than where the character is moving, apply additional acceleration.
                currentHorizontalSpeed = currentHorizontalSpeed + (groundAcceleration + groundAcceleration * reactivityPercent) * Time.fixedDeltaTime * currentInputDirection;
            }
        }
        currentHorizontalSpeed = Mathf.Min(Mathf.Abs(currentHorizontalSpeed), maxGroundSpeed) * Mathf.Sign(currentHorizontalSpeed);
    }

    private void UpdateVerticalSpeed(bool grounded)
    {
        float gravityValue = -gravity * Time.fixedDeltaTime;
        if (grounded)
        {
            // Always reset vertical speed on ground
            currentVerticalSpeed = 0;
        }
        else if (currentVerticalSpeed > 0)
        {
            // Lower the gravity value if the character is ascending after a jump
            gravityValue = gravityValue * gravityAscensionModifier;
        }
        currentVerticalSpeed += gravityValue;
        // Cut the jump the moment the player stops holding the button
        if (!processInput || !playerInput.holdingJump)
        {
            cutJumpShort = true;
        }
        if (characterJustJumped)
        {
            currentVerticalSpeed = jumpImpulse;
            timeSinceJumpStarted = 0;
            characterJustJumped = false;
            grounded = false;
        }
        if (!grounded)
        {
            // If cutting the jump, lower the vertical speed
            if (cutJumpShort && timeSinceJumpStarted > minJumpTime)
            {
                currentVerticalSpeed = Mathf.Min(currentVerticalSpeed, cutJumpSpeedLimit);
            }
            timeSinceJumpStarted += Time.fixedDeltaTime;
        }
        currentVerticalSpeed = Mathf.Max(currentVerticalSpeed, -maxFallingSpeed);
    }

    private void UpdatePosition()
    {
        tmpVector2.Set(currentHorizontalSpeed, currentVerticalSpeed);
        characterController.Move(tmpVector2);
    }

    private void UpdateRotation(float currentInputDirection)
    {
        Vector3 forward = transform.forward;
        // Start a new rotation only if the direction input has changed
        if (currentInputDirection != facingDirection)
        {
            // First calculate the currentRotationTime instead of setting it 
            // straight to 0 in case the character was already rotating when 
            // the controller changed direction.
            float delta = (forward.z + 1) / 2;
            if (currentInputDirection < 0)
            {
                delta = 1 - delta;
            }
            currentRotationTime = timeToFlipFacingDirection * delta;
            rotating = true;
            tmpVector3.Set(0, 0, -currentInputDirection);
            fromRotation = Quaternion.LookRotation(tmpVector3);
            tmpVector3.Set(0, 0, currentInputDirection);
            toRotation = Quaternion.LookRotation(tmpVector3);
        }
        if (rotating)
        {
            // Lerp between original and new rotation
            currentRotationTime += Time.fixedDeltaTime;
            float delta = currentRotationTime / timeToFlipFacingDirection;
            transform.rotation = Quaternion.Slerp(fromRotation, toRotation, delta) * originalRotation;
            if (currentRotationTime >= timeToFlipFacingDirection)
            {
                rotating = false;
            }
        }
    }

    public bool IsGrounded
    {
        get { return grounded; }
    }

    public float FacingDirection
    {
        get { return facingDirection; }
    }
}
