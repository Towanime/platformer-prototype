using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStartAnimationBehaviour : StateMachineBehaviour {

    private bool finishedEventSent;
    private bool bounceEventSent;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!bounceEventSent && stateInfo.normalizedTime >= 0.75f)
        {
            SoundManager.Instance.Play(SoundManager.Instance.startTextBounceSound);
            bounceEventSent = true;
        }
        if (!finishedEventSent && stateInfo.normalizedTime >= 1)
        {
            GameObject gameStateObject = GameObject.FindGameObjectWithTag("GameState");
            if (gameStateObject != null)
            {
                gameStateObject.GetComponent<GameStateMachine>().OnLevelStartAnimationEnded();
            }
            finishedEventSent = true;
        }
    }
}
