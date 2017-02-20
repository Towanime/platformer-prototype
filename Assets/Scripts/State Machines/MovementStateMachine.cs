using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class MovementStateMachine : MonoBehaviour {

    public PlayerInput playerInput;
    public CharacterMovement characterMovement;
    public GroundCheck groundCheck;
    private StateMachine<MovementStates> stateMachine;

    void Awake()
    {
        if (stateMachine == null)
        {
            InitStateMachine();
        }
    }

    void Frozen_Enter()
    {
        characterMovement.SetSpeed(0, 0);
    }

    void InputDisabled_Enter()
    {
        characterMovement.UpdateInput(0, false);
    }

    void InputDisabled_FixedUpdate()
    {
        characterMovement.Move();
    }

    void InputEnabled_FixedUpdate()
    {
        bool isLockingAim = groundCheck.IsGrounded && playerInput.lockAim;
        characterMovement.UpdateInput(isLockingAim ? 0 : playerInput.horizontalDirection, playerInput.holdingJump);
        characterMovement.Move();
    }

    public StateMachine<MovementStates> StateMachine
    {
        get {
            if (stateMachine == null)
            {
                InitStateMachine();
            }
            return stateMachine;
        }
    }

    void InitStateMachine()
    {
        stateMachine = StateMachine<MovementStates>.Initialize(this);
    }
}
