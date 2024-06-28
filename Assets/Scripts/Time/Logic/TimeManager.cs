using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TimeManager : Singleton<TimeManager>
{
    public int gameDay, gameMinute, gameSecond;
    public int secondTimer;

    private bool gameClockPause;
    private bool gameClockApiPause;
    private float tikTime;
    public int pauseSignalNum = 0;
    public int pauseSignalApiNum = 0;

    private bool playerPause = false;

    protected override void Awake()
    {
        base.Awake();
        NewGameTime();
    }


    private void Start()
    {
        // EventHandler.CallGameDayEvent(gameDay);
        // EventHandler.CallGameMinuteEvent(gameMinute, gameDay);
        EventHandler.BeforeEnterNextLevelEvent += NewGameTime;
        EventHandler.BeforeNextChallengeDayEvent += NewGameTime;
        EventHandler.BeginChallengeEvent += NewGameTime;
    }


    private void Update()
    {
        if (!IsPaused() && TaskManager.Instance != null) {
            //
            if (TaskManager.Instance.gameMode == GameMode.Task && secondTimer >= TaskManager.Instance.taskInfo?.timeLimit) return;
            if (TaskManager.Instance.gameMode == GameMode.Challenge && (ChallengeLevelManager.Instance == null || secondTimer >= ChallengeLevelManager.Instance.GetLevelTimeLimit())) return;
            
            tikTime += Time.deltaTime;

            if (tikTime >= Settings.secondThreshold) {
                tikTime -= Settings.secondThreshold;
                UpdateGameTime();
            }
            
            // if (secondTimer >= 150f || CoinManager.Instance.successOrder == NPCCreator.Instance.orderInfo.totalOrdersNum) {
            //     StartCoroutine(PauseForSeconds(5.0f));
            // }
        }
    }


    public void PauseByPlayer()
    {
        playerPause = true;
    }


    public void ResumeByPlayer()
    {
        playerPause = false;
    }

    public bool IsPaused()
    {
        return gameClockPause || gameClockApiPause || playerPause;
    }

    public void ErasePauseSingal()
    {
        pauseSignalNum = 0;
        pauseSignalApiNum = 0;
        gameClockApiPause = false;
        gameClockPause = false;
    }


    public void ResumeGame()
    {
        pauseSignalNum -= 1;
        if (pauseSignalNum == 0) gameClockPause = false;
        if (pauseSignalNum < 0) pauseSignalNum = 0;
    }

    public void ResumeGameWhileTryApi()
    {
        pauseSignalApiNum -= 1;
        if (pauseSignalApiNum == 0) gameClockApiPause = false;
        if (pauseSignalApiNum < 0) pauseSignalApiNum = 0;
    }


    public void PauseGame()
    {   
        pauseSignalNum += 1;
        gameClockPause = true;
    }

    public void PauseGameWhileTryApi()
    {   
        pauseSignalApiNum += 1;
        gameClockApiPause = true;
    }

    public void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameDay = 1;

        secondTimer = 0;
        EventHandler.CallGameSecondEvent(secondTimer);
    }


    public WorldTime GetGameTime()
    {
        return new WorldTime(gameDay, gameMinute, gameSecond);
    }


    private void UpdateGameTime()
    {
        // if (secondTimer >= LevelManager.Instance.GetLevelTimeLimit()) {
        //     return;
        // }
        secondTimer++;
        gameSecond++;
        EventHandler.CallGameSecondEvent(secondTimer);
        if (gameSecond > Settings.secondHold) {
            gameMinute++;
            gameSecond = 0;
            EventHandler.CallGameMinuteEvent(gameMinute, gameDay);
        }
        // if (secondTimer == LevelManager.Instance.GetLevelTimeLimit()) {
        //     EventHandler.CallGameDayEvent(gameDay);
        //     // LevelManager.Instance.LevelUp();
        //     secondTimer = 0;

        //     gameSecond = 0;
        //     gameMinute = 0;
        //     gameDay += 1;

        // }
    }

    // IEnumerator PauseForSeconds(float seconds)
    // {
    //     PauseGame();
    //     // Time.timeScale = 0;
    //     yield return new WaitForSecondsRealtime(seconds);
    //     EventHandler.CallAfterCompleteCurrentLevelEvent();
    //     // LevelManager.Instance.EnterNextData();
    //     EventHandler.CallBeforeEnterNextLevelEvent();

    //     // Time.timeScale = 1;
    //     ResumeGame();
    // }
}
