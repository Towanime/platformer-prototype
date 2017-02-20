using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class AimingStateMachine : MonoBehaviour {

    public PlayerInput playerInput;
    public AimingDirectionResolver aimingDirectionResolver;
    private StateMachine<AimStates> stateMachine;

    void Awake()
    {
        if (stateMachine == null)
        {
            InitStateMachine();
        }
    }

    void Enabled_FixedUpdate()
    {
        aimingDirectionResolver.UpdateInput(playerInput.horizontalDirection, playerInput.verticalDirection);
    }

    void Disabled_Enter()
    {
        aimingDirectionResolver.UpdateInput(0, 0);
    }

    public StateMachine<AimStates> StateMachine
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
        stateMachine = StateMachine<AimStates>.Initialize(this);
    }
}
