using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class AimingStateMachine : MonoBehaviour {

    public PlayerInput playerInput;
    public AimingDirectionResolver aimingDirectionResolver;
    private StateMachine<AimStates> stateMachine;

    // Use this for initialization
    void Awake() {
        stateMachine = StateMachine<AimStates>.Initialize(this);
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
        get { return stateMachine; }
    }
}
