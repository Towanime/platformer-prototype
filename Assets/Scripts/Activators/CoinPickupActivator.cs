using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickupActivator : BaseActivator {
    private CoinCollector collector;

    void Start()
    {
        collector = GameObject.FindGameObjectWithTag("Player").GetComponent<CoinCollector>();
    }

    public override void Activate(GameObject trigger)
    {
        SoundManager.Instance.StopAndPlay(SoundManager.Instance.coinPickupSound);
        if (collector)
        {
            collector.PickupCoin();
        }
    }
}
