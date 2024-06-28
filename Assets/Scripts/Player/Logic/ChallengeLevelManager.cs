using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

public class ChallengeLevelManager : Singleton<ChallengeLevelManager>
{
    public int GameLevel {
        get {return m_GameLevel;}
        set {m_GameLevel = value;}
    }
    
    [SerializeField] private int m_GameLevel = 1;
    private System.Random random = new System.Random();

    // [SerializeField] private GameInfo gameInfo;


    protected override void Awake()
    {
        base.Awake();
        GameLevel = 1;
        // Debug.Log(GameLevel);
        EventHandler.BeginChallengeEvent += BeginChallengeLevel;
        // EventHandler.AfterCompleteCurrentLevelEvent += EnterNextData;
        EventHandler.BeforeNextChallengeDayEvent += EnterNextLevel;
    }

    void Start()
    {

    }

    public void EnterNextLevel()
    {
        GameLevel += 1;
//         dataIdx += 1;
//         if (dataIdx > 10) {
// #if UNITY_EDITOR
//         EditorApplication.isPlaying = false;
// #else
//         Application.Quit();
// #endif
//             return;
//         }
    }

    public void BeginChallengeLevel()
    {
        GameLevel = 1;
    }


    // public int GetLevelGoal()
    // {
    //     if (GameLevel >= 1 && GameLevel <= 3) {
    //         return Settings.levelGoals[GameLevel - 1];
    //     }
    //     return -1;
    // }

    public float GetLevelTimeLimit()
    {
        if (TaskManager.Instance != null) {
            if (TaskManager.Instance.gameMode == GameMode.Task) {
                if (TaskManager.Instance.taskInfo != null) {
                    return TaskManager.Instance.taskInfo.timeLimit;
                }
            }
            else if (TaskManager.Instance.gameMode == GameMode.Challenge) {
                if (GameLevel >= 1 && GameLevel <= 4) {
                    return Settings.levelTimeLimit[GameLevel - 1];
                }  
            }
        }
        return 180f;
 
        // return -1;
    }


    public OrderData GetLevelRandomOrderData()
    {
        OrderData orderData = new()
        {
            seatIdx = 1
        };
        float chance = UnityEngine.Random.Range(0f, 1f);
        if (chance < GetLevelOrderDrinkProbability() || Settings.noBeveragesToday) {
            orderData.wantsBeverage = true;
        }
        else {
            orderData.wantsBeverage = false;
        }
        if (chance < GetLevelRecommendationProbability() || Settings.recommendDishesToday) {
            orderData.foodType = "none";
        }
        else {
            orderData.foodType = GetRandomDish();
        }
        // TODO: add flavors
        return orderData;
    }

    public float GetLevelOrderDrinkProbability()
    {
        if (GameLevel >= 1 && GameLevel <= 4) {
            return Settings.levelOrderDrinkProbability[GameLevel - 1];
        }
        return 0.5f;
    }

    public float GetLevelRecommendationProbability()
    {
        if (GameLevel >= 1 && GameLevel <= 4) {
            return Settings.levelRecommendationProbability[GameLevel - 1];
        }
        return 0.5f;
    }

    public string GetRandomDish()
    {
        // Flatten the 2D array into a single list of dishes
        List<string> allDishes = new List<string>();
        int listCount = Settings.dishPerChallengeLevelList.Length;
        if (GameLevel > listCount) {
            allDishes.AddRange(Settings.dishPerChallengeLevelList[listCount - 1]);
        }
        else {
            allDishes.AddRange(Settings.dishPerChallengeLevelList[GameLevel - 1]);
        }
        return allDishes[random.Next(allDishes.Count)];
    }


    public int GetLevelDishNum()
    {
        if (GameLevel >= 1 && GameLevel <= 4) {
            return Settings.levelDishNum[GameLevel - 1];
        }   
        return -1;
    }

    public int GetLevelCustomerNum()
    {
        if (GameLevel >= 1 && GameLevel <= 3) {
            return Settings.levelCustomerNum[GameLevel - 1];
        }
        return 99999;
    }


    public void LevelUp()
    {
        if (GameLevel < 5) {
            GameLevel += 1;
        }
        
        UIManager.Instance.ShowLevelBoard();
    }
}
