using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventHandler
{
    public static event Action<int, int> GameMinuteEvent;
    public static void CallGameMinuteEvent(int minute, int day)
    {
        GameMinuteEvent?.Invoke(minute, day);
    }

    public static event Action<int> GameSecondEvent;
    public static void CallGameSecondEvent(int second)
    {
        GameSecondEvent?.Invoke(second);
    }

    public static event Action PauseEvent;
    public static void CallPauseEvent()
    {
        PauseEvent?.Invoke();
    }

    public static event Action<int> GameDayEvent;
    public static void CallGameDayEvent(int day)
    {
        GameDayEvent?.Invoke(day);
    }

    public static event Action<string, Vector3> TransitionEvent;

    public static void CallTransitionEvent(string sceneName, Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }

    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }

    public static event Action AfterSceneLoadedEvent;
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }

    public static event Action BeforeEnterNextLevelEvent;
    public static void CallBeforeEnterNextLevelEvent()
    {
       BeforeEnterNextLevelEvent?.Invoke();
    }

    public static event Action AfterCompleteCurrentLevelEvent;
    public static void CallAfterCompleteCurrentLevelEvent()
    {
       AfterCompleteCurrentLevelEvent?.Invoke();
    }

    // public static event Action BeforeEnterNextDayEvent;
    // public static void CallBeforeEnterNextDayEvent()
    // {
    //    BeforeEnterNextDayEvent?.Invoke();
    // }

    // public static event Action AfterCompleteCurrentDayEvent;
    // public static void CallAfterCompleteCurrentDayEvent()
    // {
    //    AfterCompleteCurrentDayEvent?.Invoke();
    // }

    public static event Action BeginChallengeEvent;
    public static void CallBeginChallengeEvent()
    {
       BeginChallengeEvent?.Invoke();
    }

    public static event Action BeforeNextChallengeDayEvent;
    public static void CallBeforeNextChallengeDayEvent()
    {
       BeforeNextChallengeDayEvent?.Invoke();
    }

    public static event Action AfterCurrentChallengeDayEvent;
    public static void CallAfterCurrentChallengeDayEvent()
    {
       AfterCurrentChallengeDayEvent?.Invoke();
    }

    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }

    public static event Action<int> OnCoinNumChanged;
    public static void CallOnCoinNumChanged(int coinNum)
    {
        OnCoinNumChanged?.Invoke(coinNum);
    }

}