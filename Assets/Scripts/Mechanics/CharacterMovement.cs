using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {

    public CharacterController characterController;
    public AimingDirectionResolver aimingDirectionResolver;
    public GroundCheck groundCheck;
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
    private float currentHorizontalSpeed = 0f;
    /// <summary>
    /// Direction in which the character is moving (-1 = left, 1 = right).
    /// </summary>
    private float currentMovingDirection = 1f;
    /// <summary>
    /// Direction in which the character was pressing during the last update (-1 = left, 1 = right).
    /// </summary>
    private float currentInputDirection = 1f;
    /// <summary>
    /// Current input, reset to 0 after each Move() call
    /// </summary>
    private float horizontalInput = 0;
    /// <summary>
    /// True if the player is holding the jump button
    /// </summary>
    private bool holdingJump;

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
    /// <summary>
    /// If it's currently grounded.
    /// </summary>
    private bool grounded;
    /// <summary>
    /// If it was grounded in the last frame.
    /// </summary>
    private bool wasGrounded;
    private bool jumping;
    private float currentVerticalSpeed = 0f;
    private float timeInTheAir = 0f;
    private float timeSinceJumpStarted = 0f;
    private bool cutJumpShort = false;
    private bool characterJustJumped;

    public void UpdateInput(float horizontalInput, bool holdingJump)
    {
        this.horizontalInput = horizontalInput;
        this.holdingJump = holdingJump;
    }

    public void Move()
    {
        if (currentHorizontalSpeed != 0)
        {
            currentMovingDirection = Mathf.Sign(currentHorizontalSpeed);
        }
        // Either -1 or 1, see lastInputDirection for details.
        float newInputDirection = currentInputDirection;
        if (horizontalInput != 0)
        {
            newInputDirection = horizontalInput;
        }
        grounded = groundCheck.IsGrounded;
        UpdateSpeed(grounded, horizontalInput, newInputDirection);
        UpdatePosition();
        currentInputDirection = newInputDirection;
        UpdateInput(0, false);
    }

    public bool Jump(bool canJumpInMidAir)
    {
        bool canJump = canJumpInMidAir || (!jumping && (groundCheck.IsGrounded || timeInTheAir <= jumpCallTolerance));
        if (canJump)
        {
            cutJumpShort = false;
            characterJustJumped = true;
            return true;
        }
        return false;
    }

    void Update()
    {
        grounded = groundCheck.IsGrounded;
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
        if (moving && !pressingDirection)
        {
            float friction = (grounded) ? groundFriction : airFriction;
            currentHorizontalSpeed = currentHorizontalSpeed + friction * Time.fixedDeltaTime * -currentMovingDirection;
            // If no direction is being pressed then clamp the speed to 0 once reached.
            if (currentMovingDirection > 0)
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
            bool movingInSameDirectionBeingPressed = currentMovingDirection == currentInputDirection;
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
        ApplyGravity();
        ProcessJumpInput();
        ClampVerticalSpeed();
        wasGrounded = grounded;
    }

    private void ProcessJumpInput()
    {
        // Cut the jump the moment the player stops holding the button
        if (!holdingJump)
        {
            cutJumpShort = true;
        }
        if (grounded)
        {
            jumping = false;
        }
        if (characterJustJumped)
        {
            currentVerticalSpeed = jumpImpulse;
            timeSinceJumpStarted = 0;
            characterJustJumped = false;
            grounded = false;
            jumping = true;
        }
    }

    private void ClampVerticalSpeed()
    {
        if (!grounded && jumping)
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

    private void ApplyGravity()
    {
        float gravityValue = -gravity * Time.fixedDeltaTime;
        if (!grounded)
        {
            if (wasGrounded && currentVerticalSpeed < 0)
            {
                // Reset vertical speed the moment the player stops touching the ground (if it's not jumping)
                currentVerticalSpeed = 0;
            }
            else if (currentVerticalSpeed >= 0)
            {
                // Lower the gravity value if the character is ascending after a jump
                gravityValue = gravityValue * gravityAscensionModifier;
            }
        }
        currentVerticalSpeed += gravityValue;
    }

    private void UpdatePosition()
    {
        tmpVector2.Set(currentHorizontalSpeed, currentVerticalSpeed);
        characterController.Move(tmpVector2);
    }

    public void SetSpeed(float x, float y)
    {
        currentHorizontalSpeed = x;
        currentVerticalSpeed = y;
    }

    /// <summary>
    /// Adds an instant force in the X and Y axis.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void AddForce(float x, float y)
    {
        currentHorizontalSpeed = x;
        currentVerticalSpeed = y;
    }

    public bool IsMoving
    {
        get { return currentHorizontalSpeed != 0; }
    }

    public float CurrentVerticalSpeed
    {
        get { return currentVerticalSpeed; }
    }
}
