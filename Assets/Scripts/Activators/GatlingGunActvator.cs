using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingGunActvator : BaseActivator
{
    public GatlingGun gatlingGun;

    public override void Activate(GameObject trigger)
    {
        gatlingGun.IsEnabled = true;
        SoundManager.Instance.Play(SoundManager.Instance.gatlingGunPickupSound);
    }

    public override void Desactivate()
    {
        gatlingGun.IsEnabled = false;
    }
}
