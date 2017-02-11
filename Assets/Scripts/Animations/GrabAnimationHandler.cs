using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// using this as we can't use animation events at the moment!
public class GrabAnimationHandler : StateMachineBehaviour
{
    private bool finished;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        finished = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!finished && stateInfo.normalizedTime >= 1)
        {
            finished = true;
            // get the grab component for the player and start the throw
            animator.gameObject.GetComponent<Grab>().ArmThrow();
        }
    }
}
