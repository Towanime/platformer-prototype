using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject target;
    [Tooltip("UI rectangle used to determine where the camera will focus horizontally. If the player is facing right then the left side will be used and viceversa.")]
    public RectTransform horizontalPositionGuide;
    public AimingDirectionResolver aimingDirectionResolver;
    public GroundCheck groundCheck;
    public CharacterMovement characterMovement;
    public LayerMask groundLayer;

    // Hard world limits that the camera cannot pass
    [Tooltip("Left camera limit of the world.")]
    public Transform leftWorldLimit;
    [Tooltip("Right camera limit of the world.")]
    public Transform rightWorldLimit;
    [Tooltip("Lower camera limit of the world.")]
    public Transform lowerWorldLimit;

    [Tooltip("Maximum distance from the target to the nearest platform that there has to be for the camera to fix its vertical focus on the platform.")]
    public float platformSnapDistance = 3f;
    [Tooltip("Offset to apply in Y to the camera when it's focused on a platform.")]
    public float platformCameraOffset = -2f;
    [Tooltip("Offset to apply in Y to the camera when the character is falling down.")]
    public float fallingOffset = -6f;
    [Tooltip("Time that will take for the camera to go from 0 to the Falling Offset value.")]
    public float timeToReachMaxFallingOffset = 0.3f;
    [Tooltip("Curve for the value that is applied for the falling offset.")]
    public AnimationCurve fallingOffsetAnimationCurve;

    [Tooltip("Time taken for the camera to reach the target in X.")]
    public float cameraFollowDampTimeX = 0.05f;
    [Tooltip("Time taken for the camera to reach the target in Y.")]
    public float cameraFollowDampTimeY = 0.3f;
    [Tooltip("Time taken for the camera to reposition itself completely to a new facing direction of the target.")]
    public float cameraChangeDirectionDampTime = 0.3f;
    [Tooltip("Amount of units used to move the vertically when the target is aiming.")]
    public float aimingOffsetY = 1.5f;

    private Camera camera;
    private Vector3 originalCameraPosition;
    /// <summary>
    /// Distance in Z between the camera and the target
    /// </summary>
    private float distanceZ;

    // Variables that keep track of the velocity being applied to the smoothing operations
    private float currentFollowDampVelocityX;
    private float currentFollowDampVelocityY;
    private float currentChangeDirectionDampVelocityX;

    /// <summary>
    /// Used to know which part of the guide rectangle to target currently, goes from 0...1. 0 = Left side, 1 = Right side
    /// </summary>
    private float guideDeltaX = 1;
    private float elapsedFallingTime;
    private Vector3 cameraCenterWorld;
    private Vector3 cameraBottomLeftCornerWorld;
    private Vector3 cameraTopRightCornerWorld;
    private Vector3 tmp;

    // Use this for initialization
    void Start ()
    {
        camera = GetComponent<Camera>();
        originalCameraPosition = transform.position;
        // Get distance in Z between target and camera
        distanceZ = (target.transform.position - originalCameraPosition).z;
    }
	
	void FixedUpdate ()
    {
        UpdateGuideDeltaX();
        UpdateCameraPosition();
    }

    void UpdateCameraWorldCoordinates()
    {
        // center
        tmp.Set(camera.pixelWidth / 2, camera.pixelHeight / 2, distanceZ);
        cameraCenterWorld = camera.ScreenToWorldPoint(tmp);
        // bottom-left
        tmp.Set(0, 0, distanceZ);
        cameraBottomLeftCornerWorld = camera.ScreenToWorldPoint(tmp);
        // top-right
        tmp.Set(camera.pixelWidth, camera.pixelHeight, distanceZ);
        cameraTopRightCornerWorld = camera.ScreenToWorldPoint(tmp);
    }

    private void UpdateGuideDeltaX()
    {
        float targetDelta = GetFacingDirection() > 0 ? 1 : 0;
        guideDeltaX = Mathf.SmoothDamp(guideDeltaX, targetDelta, ref currentChangeDirectionDampVelocityX, cameraChangeDirectionDampTime);
    }

    void UpdateCameraPosition()
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition;
        // Set desired horizontal and vertical position first
        newPosition.x = GetCameraNextPositionX();
        newPosition.y = GetCameraNextPositionY();
        // Adjust camera to horizontal and vertical limits 
        SetPosition(newPosition);
        UpdateCameraWorldCoordinates();
        newPosition.x = GetCameraPositionAdjustedX();
        newPosition.y = GetCameraPositionAdjustedY();
        // Apply aiming vertical direction and adjust camera again to vertical limit
        newPosition.y = ApplyAimingDirectionOffset(newPosition.y);
        SetPosition(newPosition);
        UpdateCameraWorldCoordinates();
        newPosition.y = GetCameraPositionAdjustedY();
        // Finally get current position by applying smoothing
        newPosition.x = Mathf.SmoothDamp(currentPosition.x, newPosition.x, ref currentFollowDampVelocityX, cameraFollowDampTimeX);
        newPosition.y = Mathf.SmoothDamp(currentPosition.y, newPosition.y, ref currentFollowDampVelocityY, cameraFollowDampTimeY);
        SetPosition(newPosition);
    }

    void SetPosition(Vector3 position)
    {
        transform.position = position;
        UpdateCameraWorldCoordinates();
    }

    /// <summary>
    /// Calculates the final destination of the camera in the X axis
    /// </summary>
    /// <returns></returns>
    private float GetCameraNextPositionX()
    {
        // Get the corners of the guide in world coordinates
        Vector3[] guideWorldCorners = new Vector3[4];
        horizontalPositionGuide.GetWorldCorners(guideWorldCorners);
        // Get a position between the left and right side of the rectangle depending on the delta
        tmp.x = Mathf.Lerp(guideWorldCorners[3].x, guideWorldCorners[0].x, guideDeltaX);
        Vector3 worldPointCameraOffset = camera.ScreenToWorldPoint(tmp);
        // Set the goal to the character position plus the offset
        return target.transform.position.x + (cameraCenterWorld.x - worldPointCameraOffset.x);
    }

    /// <summary>
    /// Returns the adjusted X axis of the camera relative to world limits
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private float GetCameraPositionAdjustedX()
    {
        // Compare camera corners to world limits
        float newX = transform.position.x;
        if (leftWorldLimit != null && cameraBottomLeftCornerWorld.x < leftWorldLimit.position.x)
        {
            newX += leftWorldLimit.position.x - cameraBottomLeftCornerWorld.x;
        }
        if (rightWorldLimit != null && cameraTopRightCornerWorld.x > rightWorldLimit.position.x)
        {
            newX -= cameraTopRightCornerWorld.x - rightWorldLimit.position.x;
        }
        return newX;
    }

    /// <summary>
    /// Calculates the final destination of the camera in the Y axis
    /// </summary>
    /// <returns></returns>
    private float GetCameraNextPositionY()
    {
        // First check if there's a platform beneath the target (no matter if its grounded or not)
        RaycastHit[] hitInfoArray = Physics.RaycastAll(target.transform.position, Vector3.down, platformSnapDistance, groundLayer.value);
        if (hitInfoArray.Length > 0)
        {
            return GetCameraFocusOnNearestPlatformPositionY(hitInfoArray);
        }
        // If there's no platform just focus on the target
        return GetCameraFocusOnTargetPositionY();
    }

    /// <summary>
    /// Gets the position that the camera has to move to when focusing vertically on the nearest platform.
    /// </summary>
    /// <param name="hitInfoArray"></param>
    /// <returns></returns>
    private float GetCameraFocusOnNearestPlatformPositionY(RaycastHit[] hitInfoArray)
    {
        // Get the closest platform
        RaycastHit closestPlatform = hitInfoArray[0];
        for (int i = 1; i<hitInfoArray.Length; i++)
        {
            RaycastHit currentPlatform = hitInfoArray[i];
            if (currentPlatform.distance < closestPlatform.distance)
            {
                closestPlatform = currentPlatform;
            }
        }
        float distanceToMove = closestPlatform.point.y - platformCameraOffset - cameraCenterWorld.y;
        return transform.position.y + distanceToMove;
    }

    /// <summary>
    /// Gets the position that the camera has to move to when focusing vertically on the target.
    /// </summary>
    /// <returns></returns>
    private float GetCameraFocusOnTargetPositionY()
    {
        float distanceY = target.transform.position.y - cameraCenterWorld.y;
        return transform.position.y + distanceY + GetFallingOffset();
    }

    /// <summary>
    /// Gets the offset to apply when focusing vertically on the target. 
    /// If the target is falling this offset will be between 0 and fallingOffset, 
    /// otherwise it will be 0.
    /// </summary>
    /// <returns></returns>
    private float GetFallingOffset()
    {
        float currentFallingOffset = 0;
        if (GetCurrentVerticalSpeed() < 0 && !groundCheck.IsGrounded)
        {
            elapsedFallingTime += Time.deltaTime;
            float t = fallingOffsetAnimationCurve.Evaluate(elapsedFallingTime / timeToReachMaxFallingOffset);
            currentFallingOffset = Mathf.Lerp(0, fallingOffset, t);
        } else
        {
            elapsedFallingTime = 0;
        }
        return currentFallingOffset;
    }

    /// <summary>
    /// Apply an offset to the Y position if aiming is being applied
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    private float ApplyAimingDirectionOffset(float y)
    {
        if (aimingDirectionResolver != null && groundCheck != null)
        {
            float aimingDirectionY = aimingDirectionResolver.GetAimingDirection(groundCheck.IsGrounded).y;
            return y + aimingDirectionY * aimingOffsetY;
        }
        return y;
    }

    /// <summary>
    /// Returns the adjusted Y axis of the camera relative to world limits
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    private float GetCameraPositionAdjustedY()
    {
        // Compare camera corners to world limits
        float newY = transform.position.y;
        if (lowerWorldLimit != null && cameraBottomLeftCornerWorld.y < lowerWorldLimit.position.y)
        {
            newY += lowerWorldLimit.position.y - cameraBottomLeftCornerWorld.y;
        }
        return newY;
    }

    /// <summary>
    /// Gets the direction in which the target is facing, right by default
    /// </summary>
    /// <returns></returns>
    private float GetFacingDirection()
    {
        return characterMovement != null ? characterMovement.FacingDirection : 1;
    }

    private float GetCurrentVerticalSpeed()
    {
        return characterMovement != null ? characterMovement.CurrentVerticalSpeed : 0;
    }
}
