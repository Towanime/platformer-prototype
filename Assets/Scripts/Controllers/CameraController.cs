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

    // Hard world limits that the camera cannot pass
    [Tooltip("Left camera limit of the world.")]
    public Transform leftWorldLimit;
    [Tooltip("Right camera limit of the world.")]
    public Transform rightWorldLimit;
    [Tooltip("Lower camera limit of the world.")]
    public Transform lowerWorldLimit;

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

    void UpdateCameraPosition()
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition;
        // Set horizontal position first and with neutral vertical position
        newPosition.x = GetCameraTargetPositionX();
        newPosition.y = originalCameraPosition.y;
        // Adjust camera to horizontal and vertical limits 
        transform.position = newPosition;
        newPosition.x = GetCameraPositionAdjustedX();
        newPosition.y = GetCameraPositionAdjustedY();
        // Apply aiming vertical direction and adjust camera again to vertical limit
        newPosition.y = ApplyAimingDirectionOffset(newPosition.y);
        transform.position = newPosition;
        newPosition.y = GetCameraPositionAdjustedY();
        // Finally get current position by applying smoothing
        newPosition.x = Mathf.SmoothDamp(currentPosition.x, newPosition.x, ref currentFollowDampVelocityX, cameraFollowDampTimeX);
        newPosition.y = Mathf.SmoothDamp(currentPosition.y, newPosition.y, ref currentFollowDampVelocityY, cameraFollowDampTimeY);
        transform.position = newPosition;
    }

    void UpdateGuideDeltaX()
    {
        float targetDelta = GetFacingDirection() > 0 ? 1 : 0;
        guideDeltaX = Mathf.SmoothDamp(guideDeltaX, targetDelta, ref currentChangeDirectionDampVelocityX, cameraChangeDirectionDampTime);
    }

    /// <summary>
    /// Returns the adjusted X axis of the camera relative to world limits
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private float GetCameraPositionAdjustedX()
    {
        // Get the camera corners in world units
        tmp.Set(0, 0, distanceZ);
        Vector3 x1y1 = camera.ScreenToWorldPoint(tmp);
        tmp.Set(camera.pixelWidth, camera.pixelHeight, distanceZ);
        Vector3 x2y2 = camera.ScreenToWorldPoint(tmp);
        // Compare camera corners to world limits
        float newX = transform.position.x;
        if (leftWorldLimit != null && x1y1.x < leftWorldLimit.position.x)
        {
            newX += leftWorldLimit.position.x - x1y1.x;
        }
        if (rightWorldLimit != null && x2y2.x > rightWorldLimit.position.x)
        {
            newX -= x2y2.x - rightWorldLimit.position.x;
        }
        return newX;
    }

    /// <summary>
    /// Returns the adjusted Y axis of the camera relative to world limits
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    private float GetCameraPositionAdjustedY()
    {
        // Get the camera corners in world units
        tmp.Set(0, 0, distanceZ);
        Vector3 x1y1 = camera.ScreenToWorldPoint(tmp);
        // Compare camera corners to world limits
        float newY = transform.position.y;
        if (lowerWorldLimit != null && x1y1.y < lowerWorldLimit.position.y)
        {
            newY += lowerWorldLimit.position.y - x1y1.y;
        }
        return newY;
    }

    /// <summary>
    /// Calculates the final destination of the camera in the X axis
    /// </summary>
    /// <returns></returns>
    private float GetCameraTargetPositionX()
    {
        // Get the center of the camera in world coordinates
        tmp.Set(camera.pixelWidth / 2, camera.pixelHeight / 2, distanceZ);
        Vector3 worldPointCameraCenter = camera.ScreenToWorldPoint(tmp);
        // Get the corners of the guide in world coordinates
        Vector3[] guideWorldCorners = new Vector3[4];
        horizontalPositionGuide.GetWorldCorners(guideWorldCorners);
        // Get a position between the left and right side of the rectangle depending on the delta
        tmp.x = Mathf.Lerp(guideWorldCorners[3].x, guideWorldCorners[0].x, guideDeltaX);
        Vector3 worldPointCameraOffset = camera.ScreenToWorldPoint(tmp);
        // Set the goal to the character position plus the offset
        return target.transform.position.x + (worldPointCameraCenter.x - worldPointCameraOffset.x);
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
    /// Gets the direction in which the target is facing, right by default
    /// </summary>
    /// <returns></returns>
    private float GetFacingDirection()
    {
        return characterMovement != null ? characterMovement.FacingDirection : 1;
    }
}
