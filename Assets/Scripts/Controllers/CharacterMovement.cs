using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {
    public float maxGroundSpeed = 0.15f;
    public float currentGroundSpeed = 0f;
    public float groundAcceleration = 0.5f;
    public float groundFriction = 1.2f;
    /// <summary>
    /// Used when moving in one direction while pressing the other
    /// </summary>
    public float reactivityPercent = 0.5f;
    /// <summary>
    /// How much time the rotation will take until the character 
    /// faces the same direction that is being pressed
    /// </summary>
    public float timeToFlipFacingDirection = 0.2f;
    /// <summary>
    /// Direction in which the character is moving
    /// </summary>
    private float movingDirection = 1f;
    /// <summary>
    /// Direction that the player was pressing in the last frame (-1 = left, 1 = right). 
    /// If the player is not pressing any direction then the last direction pressed is retained.
    /// </summary>
    private float lastInputDirection = 1f;
    // Rotation variables
    private Quaternion fromRotation;
    private Quaternion toRotation;
    private float currentRotationTime = 0f;
    /// <summary>
    /// True if the character's rotation is being adjusted to 
    /// face the same direction that the player is pressing.
    /// </summary>
    private bool rotating = false;
    private Vector2 tmp;
    
    void FixedUpdate()
    {
        // Either -1, 0 or 1
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        // Either -1 or 1, see lastInputDirection for details.
        float currentInputDirection = lastInputDirection;
        if (horizontalInput != 0)
        {
            currentInputDirection = horizontalInput;
        }
        UpdateSpeed(horizontalInput, currentInputDirection);
        UpdatePosition();
        UpdateRotation(currentInputDirection);
        if (currentGroundSpeed != 0)
        {
            movingDirection = Mathf.Sign(currentGroundSpeed);
        }
        lastInputDirection = currentInputDirection;
    }

    private void UpdateSpeed(float horizontalInput, float currentInputDirection)
    {
        bool pressingDirection = horizontalInput != 0;
        bool moving = currentGroundSpeed != 0;
        if (moving && !pressingDirection)
        {
            currentGroundSpeed = currentGroundSpeed + groundFriction * Time.fixedDeltaTime * movingDirection * -1;
            // If no direction is being pressed then clamp the speed to 0 once reached.
            if (movingDirection > 0)
            {
                currentGroundSpeed = Mathf.Max(0, currentGroundSpeed);
            }
            else
            {
                currentGroundSpeed = Mathf.Min(0, currentGroundSpeed);
            }
        }
        else if (pressingDirection)
        {
            bool movingInSameDirectionBeingPressed = movingDirection == currentInputDirection;
            if (movingInSameDirectionBeingPressed)
            {
                // Regular speed formula
                currentGroundSpeed = currentGroundSpeed + groundAcceleration * Time.fixedDeltaTime * currentInputDirection;
            }
            else
            {
                // If the direction being pressed is different than where the character is moving, apply additional acceleration.
                currentGroundSpeed = currentGroundSpeed + (groundAcceleration + groundAcceleration * reactivityPercent) * Time.fixedDeltaTime * currentInputDirection;
            }
        }
        currentGroundSpeed = Mathf.Min(Mathf.Abs(currentGroundSpeed), maxGroundSpeed) * Mathf.Sign(currentGroundSpeed);
    }

    private void UpdatePosition()
    {
        tmp.Set(currentGroundSpeed, 0);
        transform.Translate(tmp, Space.World);
    }

    private void UpdateRotation(float currentInputDirection)
    {
        Vector3 forward = transform.forward;
        // Start a new rotation only if the direction input has changed
        if (currentInputDirection != lastInputDirection)
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
            fromRotation = Quaternion.LookRotation(new Vector3(0, 0, -currentInputDirection));
            toRotation = Quaternion.LookRotation(new Vector3(0, 0, currentInputDirection));
        }
        if (rotating)
        {
            // Lerp between original and new rotation
            currentRotationTime += Time.fixedDeltaTime;
            float delta = currentRotationTime / timeToFlipFacingDirection;
            transform.rotation = Quaternion.Slerp(fromRotation, toRotation, delta);
            if (currentRotationTime >= timeToFlipFacingDirection)
            {
                rotating = false;
            }
        }
    }
}
