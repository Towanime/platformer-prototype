using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour {

    public Animator animator;
    public GroundCheck groundCheck;
    public AimingDirectionResolver aimingDirectionResolver;
    public CharacterMovement characterMovement;
    public GatlingGun gatlingGun;

    void Update () {
        bool grounded = groundCheck.IsGrounded;
        animator.SetBool("IsJumping", !groundCheck.IsGrounded);
        animator.SetInteger("AimDirection", aimingDirectionResolver.GetAimingDirectionValue(grounded));
        animator.SetBool("IsRunning", characterMovement.IsMoving);
        animator.SetBool("IsShooting", gatlingGun.IsFiringGun);
    }
}
