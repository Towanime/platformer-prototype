using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class ActionStateMachine : MonoBehaviour {

    public PlayerInput playerInput;
    public GroundCheck groundCheck;
    public AimingDirectionResolver aimingDirectionResolver;
    public CharacterMovement characterMovement;
    public Grab grabSkill;
    public GatlingGun gatlingGun;
    public Teleport teleport;
    public PlayerHealth playerHealth;

    // State machines
    private StateMachine<ActionStates> actionStateMachine;
    private StateMachine<VulnerabilityStates> vulnerabilityStateMachine;
    private StateMachine<MovementStates> movementStateMachine;
    private StateMachine<AimStates> aimStateMachine;
    private GameObject damageOrigin;
    private PlayerCheckpoint playerCheckpoint;

    // Use this for initialization
    void Start() {
        vulnerabilityStateMachine = GetComponent<VulnerabilityStateMachine>().StateMachine;
        movementStateMachine = GetComponent<MovementStateMachine>().StateMachine;
        aimStateMachine = GetComponent<AimingStateMachine>().StateMachine;
        actionStateMachine = StateMachine<ActionStates>.Initialize(this, ActionStates.Idle);
        playerCheckpoint = GetComponentInChildren<PlayerCheckpoint>();
    }

    void Idle_Enter()
    {
        // In idle the player can move and aim freely
        movementStateMachine.ChangeState(MovementStates.InputEnabled);
        aimStateMachine.ChangeState(AimStates.Enabled);
    }

    void Idle_Update()
    {
        // In idle the player can do every action
        if (playerInput.shooting)
        {
            actionStateMachine.ChangeState(ActionStates.Shooting);
        }
        if (playerInput.teleported)
        {
            actionStateMachine.ChangeState(ActionStates.Teleporting);
        }
        if (playerInput.grabbed)
        {
            actionStateMachine.ChangeState(ActionStates.GrabStarted);
        }
        if (playerInput.threw)
        {
            Throw();
        }
        if (playerInput.jumped)
        {
            actionStateMachine.ChangeState(ActionStates.Jumping);
        }
    }

    void GrabStarted_Enter()
    {
        bool begun = false;
        if (!grabSkill.IsHolding && CanUseOffensiveAbilities())
        {
            begun = grabSkill.Begin();
            if (begun)
            {
                movementStateMachine.ChangeState(MovementStates.Frozen);
                aimStateMachine.ChangeState(AimStates.Disabled);
                actionStateMachine.ChangeState(ActionStates.GrabRunning);
            }
        }
        if (!begun)
        {
            actionStateMachine.ChangeState(actionStateMachine.LastState);
        }
    }

    void GrabRunning_Update()
    {
        if (!grabSkill.IsRunning)
        {
            actionStateMachine.ChangeState(ActionStates.Idle);
        }
    }

    void Jumping_Enter()
    {
        // Let player jump in midair if he's floating
        bool canJumpInMidAir = actionStateMachine.LastState == ActionStates.Floating;
        bool jumped = characterMovement.Jump(canJumpInMidAir);
        if (jumped)
        {
            SoundManager.Instance.PlayRandom(SoundManager.Instance.jumpSounds);
        }
        actionStateMachine.ChangeState(ActionStates.Idle);
    }

    void Shooting_Enter()
    {
        if (gatlingGun.IsEnabled && !gatlingGun.IsOverheated && !grabSkill.IsHolding && CanUseOffensiveAbilities())
        {
            // Enable movement inputs if he's on the ground but freze movement if the shooting began on air
            MovementStates nextMovementState = IsGrounded ? MovementStates.InputEnabled : MovementStates.Frozen;
            movementStateMachine.ChangeState(nextMovementState);
            aimStateMachine.ChangeState(AimStates.Enabled);
            SoundManager.Instance.Play(SoundManager.Instance.loadingGunSound);
        } else
        {
            if (playerInput.startedShooting && gatlingGun.IsOverheated)
            {
                SoundManager.Instance.PlayRandom(SoundManager.Instance.fireWhenOverheatSounds);
            }
            actionStateMachine.ChangeState(actionStateMachine.LastState);
        }
    }

    void Shooting_Update()
    {
        if (playerInput.teleported)
        {
            actionStateMachine.ChangeState(ActionStates.Teleporting);
        }
        if (playerInput.grabbed)
        {
            actionStateMachine.ChangeState(ActionStates.GrabStarted);
        }
        // Revert back to idle once he stops shooting
        if (!playerInput.shooting || gatlingGun.IsOverheated)
        {
            actionStateMachine.ChangeState(ActionStates.Idle);
        }
    }

    void Shooting_FixedUpdate()
    {
        float aimingAngle = aimingDirectionResolver.GetAimingAngle(IsGrounded);
        gatlingGun.Fire(aimingAngle);
    }

    void Teleporting_Enter()
    {
        bool begun = false;
        if (teleport.HasTarget)
        {
            begun = teleport.BeginTeleport();
            if (begun)
            {
                teleport.StartShadowWalkEffect();
                SoundManager.Instance.StopAndPlay(SoundManager.Instance.shadowWalkSound);
                movementStateMachine.ChangeState(MovementStates.Frozen);
                aimStateMachine.ChangeState(AimStates.Disabled);
            }
        }
        if (!begun)
        {
            actionStateMachine.ChangeState(actionStateMachine.LastState);
        }
    }

    void Teleporting_FixedUpdate()
    {
        bool finished = teleport.UpdateTeleport();
        if (finished)
        {
            actionStateMachine.ChangeState(ActionStates.FloatingStarted);
        }
    }

    void FloatingStarted_Enter()
    {
        teleport.BeginFloating();
        teleport.StopShadowWalkEffect(true);
        actionStateMachine.ChangeState(ActionStates.Floating);
    }

    void Floating_Enter()
    {
        movementStateMachine.ChangeState(MovementStates.Frozen);
        aimStateMachine.ChangeState(AimStates.Enabled);
    }

    void Floating_Update()
    {
        if (IsGrounded)
        {
            actionStateMachine.ChangeState(ActionStates.Idle);
        }
        if (playerInput.shooting)
        {
            actionStateMachine.ChangeState(ActionStates.Shooting);
        }
        if (playerInput.teleported)
        {
            actionStateMachine.ChangeState(ActionStates.Teleporting);
        }
        if (playerInput.grabbed)
        {
            actionStateMachine.ChangeState(ActionStates.GrabStarted);
        }
        if (playerInput.jumped)
        {
            actionStateMachine.ChangeState(ActionStates.Jumping);
        }
        if (playerInput.threw)
        {
            Throw();
        }
    }

    void Floating_FixedUpdate()
    {
        bool finished = teleport.UpdateFloating();
        if (finished)
        {
            actionStateMachine.ChangeState(ActionStates.Idle);
        }
    }

    void Floating_Exit()
    {
        teleport.StopShadowWalkEffect(false);
    }

    void Damaged_Enter()
    {
        vulnerabilityStateMachine.ChangeState(VulnerabilityStates.Invulnerable);
        movementStateMachine.ChangeState(MovementStates.InputDisabled);
        aimStateMachine.ChangeState(AimStates.Disabled);
        playerHealth.BeginKnockback(damageOrigin);
        SoundManager.Instance.PlayRandom(SoundManager.Instance.avatarPainSounds);
    }

    void Damaged_Update()
    {
        bool finished = playerHealth.UpdateKnockback();
        if (finished)
        {
            if (grabSkill.IsRunning)
            {
                actionStateMachine.ChangeState(ActionStates.GrabRunning);
            } else
            {
                actionStateMachine.ChangeState(ActionStates.Idle);
            }
        }
    }

    void Spawning_Enter()
    {
        movementStateMachine.ChangeState(MovementStates.Frozen);
        aimStateMachine.ChangeState(AimStates.Disabled);
        vulnerabilityStateMachine.ChangeState(VulnerabilityStates.Disabled);
        playerCheckpoint.BeginSpawn();
    }

    void Spawning_Exit()
    {
        vulnerabilityStateMachine.ChangeState(VulnerabilityStates.Invulnerable);
    }

    void Spawning_Update()
    {
        if (!playerCheckpoint.IsSpawning)
        {
            actionStateMachine.ChangeState(ActionStates.Idle);
        }
    }

    void Death_Enter()
    {
        playerHealth.Refresh();
        actionStateMachine.ChangeState(ActionStates.Spawning);
    }

    public bool OnDamage(GameObject origin, float damage)
    {
        bool damaged = false;
        if (vulnerabilityStateMachine.State == VulnerabilityStates.Vulnerable && actionStateMachine.State != ActionStates.Teleporting)
        {
            // Make it 1 always because 1 unit = 1 heart
            float damageToApply = 1;
            damaged = playerHealth.ApplyDamage(origin, damageToApply);
            if (damaged) {
                damageOrigin = origin;
                ActionStates nextState = (playerHealth.CurrentLife > 0) ? ActionStates.Damaged : ActionStates.Death;
                actionStateMachine.ChangeState(nextState);
            }
        }
        return damaged;
    }

    public void OnFall()
    {
        float damageToApply = 1;
        playerHealth.ApplyDamage(null, damageToApply);
        ActionStates nextState = (playerHealth.CurrentLife > 0) ? ActionStates.Spawning : ActionStates.Death;
        actionStateMachine.ChangeState(nextState);
    }

    private bool CanUseOffensiveAbilities()
    {
        return vulnerabilityStateMachine.State != VulnerabilityStates.Invulnerable;
    }

    private bool Throw()
    {
        bool threw = false;
        if (grabSkill.IsHolding)
        {
            Vector2 aimingDirection = aimingDirectionResolver.GetAimingDirection(IsGrounded);
            Vector3 throwOriginPosition = aimingDirectionResolver.GetAimEmitorPosition(aimingDirection);
            threw = grabSkill.ThrowEnemy(aimingDirection, throwOriginPosition);
        }
        return threw;
    }

    private bool IsGrounded
    {
        get { return groundCheck.IsGrounded; }
    }
}
