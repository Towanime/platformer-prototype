using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickupActivator : BaseActivator {

    public override void Activate(GameObject trigger)
    {
        SoundManager.Instance.StopAndPlay(SoundManager.Instance.coinPickupSound);
    }
}
