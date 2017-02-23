using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCoinPickupActivator : BaseActivator
{
    private CoinCollector collector;

    void Start()
    {
        collector = GameObject.FindGameObjectWithTag("Player").GetComponent<CoinCollector>();
    }

    public override void Activate(GameObject trigger)
    {
        SoundManager.Instance.StopAndPlay(SoundManager.Instance.bigCoinPickupSound);

        if (collector)
        {
            collector.PickupBigCoin();
        }
    }
}
