using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrophyActivator : BaseActivator
{
    public Text lblCoins;
    public Text lblBigCoins;
    public Text lblDeathCounter;
    public GameObject finalCanvas;
    public GameObject darkLordCanvas;
    public CoinCollector coinCollector;
    public ActionStateMachine playerStateMachine;

    public override void Activate(GameObject trigger)
    {
        SoundManager.Instance.StopAndPlay(SoundManager.Instance.bigCoinPickupSound);
        lblBigCoins.text = coinCollector.GetBigCoinResult();
        lblCoins.text = coinCollector.GetCoinResult();
        lblDeathCounter.text = " x " + playerStateMachine.GameOverCount;
        darkLordCanvas.SetActive(false);
        finalCanvas.SetActive(true);
        // stop everything?
        Time.timeScale = 0;
    }
}
