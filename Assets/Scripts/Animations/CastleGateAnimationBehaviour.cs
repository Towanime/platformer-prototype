using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleGateAnimationBehaviour : StateMachineBehaviour {

    private bool eventSent;

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!eventSent && stateInfo.normalizedTime >= 1)
        {
            GameObject gameStateObject = GameObject.FindGameObjectWithTag("GameState");
            if (gameStateObject != null)
            {
                gameStateObject.GetComponent<GameStateMachine>().OnOpeningEventEnded();
            }
            eventSent = true;
        }
    }
}
