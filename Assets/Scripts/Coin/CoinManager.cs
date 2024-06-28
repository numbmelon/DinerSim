using System.Collections;
using System.Collections.Generic;
using LMagent.Transition;
using UnityEngine;

public class CoinManager : Singleton<CoinManager>
{
    private int coinNum;
    public int bonus = 0;
    public int successOrder = 0;
    public int revenue = 0;

    void Start()
    {
        // EventHandler.BeforeEnterNextLevelEvent += ClearCoin;
        EventHandler.BeginChallengeEvent += SetInitCoin;
    }

    public int CoinNum
    {
        get { return coinNum; }
        set
        {
            if(coinNum != value)  {
                coinNum = value;
        
                EventHandler.CallOnCoinNumChanged(coinNum);
            }
        }
    }

    public void ClearCoin()
    {
        bonus = 0;
        successOrder = 0;
        CoinNum = 0;
        revenue = 0;
    }

    public void GetCoins(int num)
    {
        CoinNum += num;
        revenue += num;
    }

    void Update()
    {
        // TODO：enter next level
        // if (successOrder == 1) {
        //     EventHandler.CallBeforeEnterNextLevelEvent();
        // }
    }

    void SetInitCoin()
    {
        CoinNum = 100;
        bonus = 0;
        successOrder = 0;
        revenue = 0;
    }
}
