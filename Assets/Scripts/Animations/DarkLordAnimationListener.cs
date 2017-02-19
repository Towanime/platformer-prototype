using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkLordAnimationListener : MonoBehaviour {

    public GroundCheck groundCheck;

	void OnFootstepFrame ()
    {
        if (groundCheck.IsGrounded)
        {
            SoundManager.Instance.Play(SoundManager.Instance.darkLordFootsteepSound);
        }
    }
}
