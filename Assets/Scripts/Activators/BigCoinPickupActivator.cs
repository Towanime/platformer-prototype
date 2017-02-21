using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCoinPickupActivator : BaseActivator {

    public override void Activate(GameObject trigger)
    {
        SoundManager.Instance.StopAndPlay(SoundManager.Instance.bigCoinPickupSound);
    }
}
