using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingDirectionResolver : MonoBehaviour {

    public PlayerInput playerInput;
    public CharacterMovement characterMovement;
    private Vector2 tmp;

    /// <summary>
    /// Returns a Vector2 with the direction the player is aiming at depending on the input
    /// X: Less than 0 = Left, 0 = Middle (Only if Y != 0), Bigger than 0 = Right
    /// Y: Less than 0 = Down, 0 = Forward, Bigger than 0 = Up
    /// If the player is grounded then the down directions are replaced for forward.
    /// </summary>
    public Vector2 GetAimingDirection(bool grounded)
    {
        float horizontalInput = playerInput.horizontalDirection;
        float verticalInput = playerInput.verticalDirection;
        // If its grounded don't aim down
        float y = (grounded) ? Mathf.Max(verticalInput, 0) : verticalInput;
        // If no horizontal direction is being held and Y is Up or Down then use the facing direction
        float x = (horizontalInput == 0 && y != 0) ? 0 : characterMovement.FacingDirection;
        tmp.Set(x, y);
        return tmp.normalized;
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
        return Mathf.Atan2(aimingDirection.y, aimingDirection.x) * Mathf.Rad2Deg;
    }
}
