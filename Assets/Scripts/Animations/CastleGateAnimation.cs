using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleGateAnimation : MonoBehaviour {

	void DoorSound()
    {
        SoundManager.Instance.Play(SoundManager.Instance.castleGateSound);
    }

    void ChainsSound()
    {
        SoundManager.Instance.Play(SoundManager.Instance.castleGateChainsSound);
    }
}
