using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinCollector : MonoBehaviour {
    public Text lblCoins;
    public Text lblBigCoins;
    public float hideLabelsAfter = 5;
    private int totalCoins;
    private int currentCoins;
    private int totalBigCoins;
    private int currentBigCoins;

    void Start()
    {
        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
        totalBigCoins = GameObject.FindGameObjectsWithTag("Big Coin").Length;
    }
	
    public void PickupCoin()
    {
        CancelInvoke("HideCoinLabel");
        currentCoins++;
        lblCoins.gameObject.SetActive(true);
        lblCoins.text = GetCoinResult();
        Invoke("HideCoinLabel", hideLabelsAfter);
    }

    public void PickupBigCoin()
    {
        CancelInvoke("HideBigCoinLabel");
        currentBigCoins++;
        lblBigCoins.gameObject.SetActive(true);
        lblBigCoins.text = GetBigCoinResult();
        Invoke("HideBigCoinLabel", hideLabelsAfter);
    }

    private void HideCoinLabel()
    {
        lblCoins.gameObject.SetActive(false);
    }

    private void HideBigCoinLabel()
    {
        lblBigCoins.gameObject.SetActive(false);
    }

    public string GetCoinResult()
    {
        return currentCoins + " / " + totalCoins;
    }

    public string GetBigCoinResult()
    {
        return currentBigCoins + " / " + totalBigCoins;
    }
}
