using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : Singleton<TaskManager>
{
    public GameMode gameMode = GameMode.Task;
    public int taskLevel = 1;
    public int taskIndex = 1;
    public TaskInfo taskInfo;

    public int currentOrderFinishNum;

    public bool enterNewDataSingal = false;
    public bool enterChallengeSingal = true;

    private bool enterNextLevel = false;
    private bool hasLaunched = false;
    
    private bool isLaunchingTask = false;

    public SpecialEventPool specialEventPool = new();

    // Start is called before the first frame update
    void Start()
    {
        // if (gameMode == GameMode.Task) {
        //     LoadData(false, false);
        // }
        // else if (gameMode == GameMode.Challenge) {
        //     LoadChallengeLevel(false);
        // }
    }

    public void LaunchTask(string mode, string taskLevelString, string taskIndexString)
    {
        if (isLaunchingTask) return;
        isLaunchingTask = true;
        if (mode == "Task") {
            gameMode = GameMode.Task;
            if (int.TryParse(taskLevelString, out int taskLevelTmp) && int.TryParse(taskIndexString, out int taskIndexTmp)) {
                taskLevel = taskLevelTmp;
                taskIndex = taskIndexTmp;
            }
            if (!hasLaunched) {
                hasLaunched = true;
                LoadData(false, false);
            }
            else {
                LoadData(false);
            }
        }
        else {
            gameMode = GameMode.Challenge;
            hasLaunched = true;
            LoadChallengeLevel(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enterNewDataSingal) {
            enterNewDataSingal = false;
            TimeManager.Instance.ErasePauseSingal();
            LoadData(false);
            return;
        }

        if (taskInfo != null) {
            if (TimeManager.Instance.IsPaused()) return;
            // if (gameMode == GameMode.Task) {
            //     if (CoinManager.Instance.successOrder == taskInfo.orderInfo.totalOrdersNum || TimeManager.Instance.secondTimer >= taskInfo.timeLimit) {
            //         LoadData();
            //     }
            // }
            else if(gameMode == GameMode.Challenge) {
                if (TimeManager.Instance.secondTimer >= ChallengeLevelManager.Instance?.GetLevelTimeLimit()) {
                    LoadChallengeLevel();
                }
            }
        }
    }

    void LoadChallengeLevel(bool callEvent = true)
    {
        TimeManager.Instance.PauseGame();
        // EventHandler.CallBeginChallengeEvent();
        StartCoroutine(LoadChallengeLevelCoroutine(callEvent, ()=> {TimeManager.Instance.ResumeGame(); isLaunchingTask = false;}));

    }

    void LoadData(bool addIndex = true, bool callEvent = true)
    {   
        TimeManager.Instance.PauseGame();
        if (gameMode == GameMode.Task) {
        currentOrderFinishNum = 0;
        if (addIndex) taskIndex += 1;
        if (taskIndex > Settings.taskNumPerLevelList[taskLevel - 1]) {
            if (taskLevel >= 9) {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            }
            else {
                // TODO: add a method %%%%
                LMManager.Instance.runningTime = TimeManager.Instance.secondTimer;
                LMManager.Instance.SaveCmdLog();
                LMManager.Instance.SaveResLog();
                NPCCreator.Instance.RecordCustomerStartEndTime();
                LMManager.Instance.SaveFinalLog();
                // LMManager.Instance.metricsList = new();
                // %%%%% 
                taskLevel += 1;
                taskIndex = 1;
                enterNextLevel = true;
            }
        }
        }
        StartCoroutine(LoadJsonFile(callEvent, ()=> {TimeManager.Instance.ResumeGame(); isLaunchingTask = false;}));
    }

    public IEnumerator LoadChallengeLevelCoroutine(bool callEvent = false, Action callback = null)
    {
        // TimeManager.Instance.PauseGame();

        while (ItemCreator.Instance == null || NPCCreator.Instance == null || AgentCreator.Instance == null || LMManager.Instance == null || NPCManager.Instance == null || ChallengeLevelManager.Instance == null || TimeManager.Instance == null || CoinManager.Instance == null) {
            yield return null;
        }
        if (!callEvent) {
            specialEventPool = new();
            EventHandler.CallBeginChallengeEvent();
            AgentCreator.Instance.ChallengeModeResetAgents();
            specialEventPool.AddEvent(specialEventPool.GetRandomNegativeEvent());
            NPCManagerAgent.Instance.ManagerTakeAction();
        }
        else {
            yield return new WaitForSeconds(5);
            EventHandler.CallAfterCurrentChallengeDayEvent();
            specialEventPool.EndOfDay();
            specialEventPool.AddEvent(specialEventPool.GetRandomNegativeEvent());
            if (ChallengeLevelManager.Instance.GameLevel >= 4) specialEventPool.AddEvent(specialEventPool.GetRandomNegativeEvent());
            NPCManagerAgent.Instance.ManagerTakeAction();

            // CoinManager.Instance.revenue
            // ChallengeLevelManager.Instance.GameLevel
            // currentOrderFinishNum

            ChallengeLevelRecord record = new(CoinManager.Instance.revenue,
            ChallengeLevelManager.Instance.GameLevel,currentOrderFinishNum, CoinManager.Instance.CoinNum);

            LMManager.Instance.SaveChallengeRecord(record);

            if (CoinManager.Instance.CoinNum < 0) {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                yield break;
            }
            

            EventHandler.CallBeforeNextChallengeDayEvent();
            
            
        }
        callback?.Invoke();
    }

    public IEnumerator LoadJsonFile(bool callEvent = false, Action callback = null)
    {
        // TimeManager.Instance.PauseGame();
        // Debug.Log(taskLevel.ToString() + " " + taskIndex.ToString());
        while (ItemCreator.Instance == null || NPCCreator.Instance == null || AgentCreator.Instance == null || LMManager.Instance == null || NPCManager.Instance == null) {
            Debug.Log("something is null");
            yield return null;
        }

        LMManager.Instance.runningTime = TimeManager.Instance.secondTimer;

        // yield return new WaitForSeconds(2);

        if (callEvent) {
            yield return new WaitForSeconds(3);
            EventHandler.CallAfterCompleteCurrentLevelEvent();
            // while (TimeManager.Instance.IsPaused()) yield return null;

            EventHandler.CallBeforeEnterNextLevelEvent();
            if (enterNextLevel) {
                enterNextLevel = false;
                LMManager.Instance.metricsList = new();
            }
        }
        else {
            LMManager.Instance.CreateLogFile();
        }
        
        TextAsset jsonData = Resources.Load<TextAsset>("task_info_level" + taskLevel.ToString() + "_" + taskIndex.ToString());

        // TODO: modify this line to judge if jsonData is exist
        if (jsonData == null) yield return new WaitForFixedUpdate();

        if (jsonData != null) {
            taskInfo = JsonUtility.FromJson<TaskInfo>(jsonData.text); 
            while (taskInfo == null) yield return null;

            // Agent
            AgentCreator.Instance.ResetAgents(taskInfo.agentInfo);

            // Scene
            ItemCreator.Instance.InitializeScene(taskInfo.sceneInfo);

            // Order
            NPCCreator.Instance.InitNPCCreator(taskInfo.orderInfo);

            // TODO: Trace
        } 
        callback?.Invoke();
        // TimeManager.Instance.ResumeGame();

    }

    void OnApplicationQuit()
    {
        if (gameMode == GameMode.Challenge) {
            ChallengeLevelRecord record = new(CoinManager.Instance.revenue,
            ChallengeLevelManager.Instance.GameLevel,currentOrderFinishNum, CoinManager.Instance.CoinNum);
            LMManager.Instance.SaveChallengeRecord(record);
        }
    }
}
