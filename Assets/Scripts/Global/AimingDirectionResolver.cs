using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingDirectionResolver : MonoBehaviour {

    // Emitor positions for 8 direction aiming
    public Transform upAimEmitor;
    public Transform upForwardAimEmitor;
    public Transform forwardAimEmitor;
    public Transform downForwardAimEmitor;
    public Transform downAimEmitor;

    private float horizontalInput;
    private float verticalInput;
    private Vector2 tmpVector2;
    private Vector3 tmpVector3;


    /// <summary>
    /// Direction that the player was pressing in the last frame (-1 = left, 1 = right). 
    /// If the player is not pressing any direction then the last direction pressed is retained.
    /// </summary>
    private float currentFacingDirection = 1f;

    // Rotation variables
    private Quaternion fromRotation;
    private Quaternion toRotation;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private float currentRotationTime = 0f;
    /// <summary>
    /// True if the character's rotation is being adjusted to 
    /// face the same direction that the player is pressing.
    /// </summary>
    private bool rotating = false;


    [Tooltip("Used when moving in one direction while pressing the other")]
    public FlipingMethod flipingMethod = FlipingMethod.Mirror;
    /// <summary>
    /// How much time the rotation will take until the character 
    /// faces the same direction that is being pressed
    /// </summary>
    [Tooltip("How much time the rotation will take until the character faces the same direction that is being pressed. Only used if flip method is \"Rotation\"")]
    public float timeToFlipFacingDirection = 0.2f;

    public enum FlipingMethod
    {
        Mirror,
        Rotation
    }

    void Start()
    {
        originalScale = transform.localScale;
        originalRotation = transform.localRotation;
    }

    public void UpdateInput(float horizontalInput, float verticalInput)
    {
        this.horizontalInput = horizontalInput;
        this.verticalInput = verticalInput;
    }

    void FixedUpdate()
    {
        float newFacingDirection = currentFacingDirection;
        if (horizontalInput != 0)
        {
            newFacingDirection = horizontalInput;
        }
        UpdateRotation(newFacingDirection);
        currentFacingDirection = newFacingDirection;
    }

    private void UpdateRotation(float newFacingDirection)
    {
        Vector3 forward = transform.forward;
        // Start a new rotation only if the direction input has changed
        if (newFacingDirection != currentFacingDirection)
        {
            if (flipingMethod == FlipingMethod.Rotation)
            {
                // First calculate the currentRotationTime instead of setting it 
                // straight to 0 in case the character was already rotating when 
                // the controller changed direction.
                float delta = (forward.z + 1) / 2;
                if (newFacingDirection < 0)
                {
                    delta = 1 - delta;
                }
                currentRotationTime = timeToFlipFacingDirection * delta;
                rotating = true;
                tmpVector3.Set(0, 0, -newFacingDirection);
                fromRotation = Quaternion.LookRotation(tmpVector3);
                tmpVector3.Set(0, 0, newFacingDirection);
                toRotation = Quaternion.LookRotation(tmpVector3);
            }
            else if (flipingMethod == FlipingMethod.Mirror)
            {
                // For horizontal mirroring just change the sign of the Z value in the scale
                tmpVector3.Set(originalScale.x, originalScale.y, originalScale.z * newFacingDirection);
                transform.localScale = tmpVector3;
            }
        }
        if (rotating)
        {
            // Lerp between original and new rotation
            currentRotationTime += Time.deltaTime;
            float delta = currentRotationTime / timeToFlipFacingDirection;
            transform.rotation = Quaternion.Slerp(fromRotation, toRotation, delta) * originalRotation;
            if (currentRotationTime >= timeToFlipFacingDirection)
            {
                rotating = false;
            }
        }
    }

    /// <summary>
    /// Returns a Vector2 with the direction the player is aiming at depending on the input
    /// X: Less than 0 = Left, 0 = Middle (Only if Y != 0), Bigger than 0 = Right
    /// Y: Less than 0 = Down, 0 = Forward, Bigger than 0 = Up
    /// If the player is grounded then the down directions are replaced for forward.
    /// </summary>
    public Vector2 GetAimingDirection(bool grounded)
    {
        // If its grounded don't aim down
        float y = (grounded) ? Mathf.Max(verticalInput, 0) : verticalInput;
        // If no horizontal direction is being held and Y is Up or Down then use the facing direction
        float x = (horizontalInput == 0 && y != 0) ? 0 : currentFacingDirection;
        tmpVector2.Set(x, y);
        return tmpVector2.normalized;
    }

    /// <summary>
    /// Returns an int representing one of the constants in the AimingDirection class 
    /// with the direction the player is aiming at. 
    /// It returns the same values no matter if the character is facing left or right.
    /// If the player is grounded then the down directions are replaced for forward.
    /// </summary>
    public int GetAimingDirectionValue(bool grounded)
    {
        Vector2 aimingDirection = GetAimingDirection(grounded);
        return GetAimingDirectionValue(aimingDirection);
    }

    public int GetAimingDirectionValue(Vector2 aimingDirection)
    {
        float x = aimingDirection.x;
        float y = aimingDirection.y;
        int aimingDirectionEnum = AimingDirection.Forward;
        if (x == 0 && y > 0)
        {
            aimingDirectionEnum = AimingDirection.Up;
        }
        else if (x != 0 && y > 0)
        {
            aimingDirectionEnum = AimingDirection.UpDiagonal;
        }
        else if (x != 0 && y < 0)
        {
            aimingDirectionEnum = AimingDirection.DownDiagonal;
        }
        else if (x == 0 && y < 0)
        {
            aimingDirectionEnum = AimingDirection.Down;
        }
        return aimingDirectionEnum;
    }

    /// <summary>
    /// Returns a float representing the aiming direction in degrees
    /// If the player is grounded then the down directions are replaced for forward.
    /// </summary>
    public float GetAimingAngle(bool grounded)
    {
        Vector2 aimingDirection = GetAimingDirection(grounded);
        return GetAimingAngle(aimingDirection);
    }

    public float GetAimingAngle(Vector2 aimingDirection)
    {
        return Mathf.Atan2(aimingDirection.y, aimingDirection.x) * Mathf.Rad2Deg;
    }

    public Vector3 GetAimEmitorPosition(bool grounded)
    {
        int aimingDirectionValue = GetAimingDirectionValue(grounded);
        return GetAimEmitorPosition(aimingDirectionValue);
    }

    public Vector3 GetAimEmitorPosition(Vector2 aimingDirection)
    {
        int aimingDirectionValue = GetAimingDirectionValue(aimingDirection);
        return GetAimEmitorPosition(aimingDirectionValue);
    }

    public Vector3 GetAimEmitorPosition(int aimingDirectionValue)
    {
        switch (aimingDirectionValue)
        {
            case AimingDirection.Up:
                return upAimEmitor.position;
            case AimingDirection.UpDiagonal:
                return upForwardAimEmitor.position;
            case AimingDirection.DownDiagonal:
                return downForwardAimEmitor.position;
            case AimingDirection.Down:
                return downAimEmitor.position;
            default:
                return forwardAimEmitor.position;
        }
    }

    public float FacingDirection
    {
        get { return currentFacingDirection; }
    }
}
