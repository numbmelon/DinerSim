using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;
using Unity.Collections;

public class LMManager : Singleton<LMManager>
{
    public LMInfo_SO baseInfo;
    public List<Metrics> metricsList = new();
    private List<APICall> apiCalls = new();
    private string cmdLogFilePath;
    private string resLogFilePath;
    private string finalFilePath;
    public string timeStamp;
    public int totalOrdersNum;
    public int maxBonus;
    public int maxSocre;
    public float runningTime;

    public string currentLogDirectory;

    public int completion_tokens = 0;
    public int prompt_tokens = 0;
    public int total_tokens = 0;
    public int apiResponseNum = 0;

    public int perfectCompletedNum = 0;


    protected override void Awake()
    {
        base.Awake();
        timeStamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        EventHandler.BeforeEnterNextLevelEvent += CreateNewLevelLogFile;
        EventHandler.BeginChallengeEvent += CreateLogFile;
    }

    void Start()
    {
        // Application.Quit();
        // CreateLogFile();

        // EventHandler.BeforeEnterNextLevelEvent +=
    }

    public void CreateLogFile()
    {
        string logDirectory = Path.Combine(Application.dataPath, "../Log/"  + timeStamp);
        if (TaskManager.Instance.gameMode == GameMode.Task) {
            logDirectory = Path.Combine(logDirectory, "level" + TaskManager.Instance.taskLevel.ToString() + "_" + TaskManager.Instance.taskIndex.ToString());
        }
        else {
            logDirectory = Path.Combine(logDirectory, "challenge");
        }
        currentLogDirectory = logDirectory;

        Directory.CreateDirectory(logDirectory); // check if path exists

        
        cmdLogFilePath = Path.Combine(logDirectory, "command.json");
        resLogFilePath = Path.Combine(logDirectory, "result.json");

        SaveCmdLog(); // init the log file
    }

    public void SaveChallengeRecord(ChallengeLevelRecord record)
    {
        string logDirectory = Path.Combine(Application.dataPath, "../Log/"  + timeStamp + "/challenge");
        Directory.CreateDirectory(logDirectory);
        finalFilePath = Path.Combine(logDirectory, "challenge_results"+ ChallengeLevelManager.Instance.GameLevel.ToString() + ".json");
        JSONWithChallengeRecord jSONWithChallengeRecord = new() {
            challengeLevelRecord = record
        };
        string json = JsonUtility.ToJson(jSONWithChallengeRecord, true);
        File.WriteAllText(finalFilePath, json);
    }

    public void CreateNewLevelLogFile()
    {
        SaveCmdLog();
        SaveResLog();
        SaveFinalLog(false);
        apiCalls = new();
        completion_tokens = 0;
        prompt_tokens = 0;
        total_tokens = 0;
        apiResponseNum = 0;
        perfectCompletedNum = 0;
        CreateLogFile();


        CoinManager.Instance.ClearCoin();
    }

    public void SaveCmdLog()
    {
        try
        {
            // Debug.Log(apiCalls.Count());
            JSONWithListOfAPICallObjects jsonWithListOfObjects = new()
            {
                apiCalls = apiCalls
            };
            string json = JsonUtility.ToJson(jsonWithListOfObjects, true);
            File.WriteAllText(cmdLogFilePath, json);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("error occurred while saving the logs: " + ex.Message);
        }
    }

    public void SaveResLog()
    {
        try {
            // todo: change file name here
            Metrics metrics = new Metrics(CoinManager.Instance.CoinNum, CoinManager.Instance.bonus, CoinManager.Instance.successOrder, totalOrdersNum, maxSocre, maxBonus, TaskManager.Instance.taskInfo?.timeLimit == null ? 0f : TaskManager.Instance.taskInfo.timeLimit, runningTime, TaskManager.Instance.taskInfo?.taskName, completion_tokens, prompt_tokens, total_tokens, apiResponseNum, perfectCompletedNum);
            string json = JsonUtility.ToJson(metrics, true);
            metricsList.Add(metrics);
            File.WriteAllText(resLogFilePath, json);

        }
        catch (Exception ex)
        {
            Debug.LogError("error occurred while saving the logs: " + ex.Message);
        }
    }

    public void SaveFinalLog(bool clearMesList = true)
    {
        string logDirectory = Path.Combine(Application.dataPath, "../Log/"  + timeStamp + "/all_results");
        Directory.CreateDirectory(logDirectory);
        finalFilePath = Path.Combine(logDirectory, "all_results_" + TaskManager.Instance.taskLevel.ToString() + ".json");
    
        SummaryMetrics sumMetrics = new (metricsList);

        JSONWithListOfMetricsObjects jsonWithListOfObjects = new()
        {
            summaryMetrics = sumMetrics,
            metricsList = metricsList
        };

        string json = JsonUtility.ToJson(jsonWithListOfObjects, true);
        File.WriteAllText(finalFilePath, json);

        if (clearMesList) metricsList = new();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator CreateAPICall(string prompt, GameObject npc, string mode="default", HistoryInstruction instruction = null, Action callback = null)
    {
        TimeManager.Instance.PauseGameWhileTryApi();
        var apiCall = new APICall(npc?.GetComponent<NPCInnerVoice>()?.npcInfo.npcName, prompt, "");
        apiCalls.Add(apiCall);

        int attempts_max_times = 10;
        bool is_success = false;
        // UnityWebRequest www = null;
        try {
            while (attempts_max_times > 0 && !is_success) {
                using (var www = new UnityWebRequest(baseInfo.llmBackendUrl, "POST")) {
                    var jsonToSend = new Request
                    {
                        prompt = prompt
                    };
                    Debug.Log(prompt);

                    www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(jsonToSend)));
                    www.downloadHandler = new DownloadHandlerBuffer();
                    www.SetRequestHeader("Content-Type", "application/json");

                    
                    yield return www.SendWebRequest();

                    // TimeManager.Instance.ResumeGameWhileTryApi();

                    if (mode == "default" && npc != null) {
                        npc?.GetComponent<NPCInnerVoice>()?.AddChatHistory(new ChatConfig(Role.System, prompt));
                    }
                    else {
                        if (npc == null) {
                            // if (attempts_max_times != 10) {
                            // TimeManager.Instance.ResumeGameWhileTryApi();
                            // }
                            TimeManager.Instance.ResumeGameWhileTryApi();
                            break;
                        }
                    }

                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log(www.error);
                        // npc.GetComponent<NPCInnerVoice>().isOccupied = false;
                        // if (attempts_max_times == 10) {
                        //     TimeManager.Instance.PauseGameWhileTryApi();
                        // }
                        attempts_max_times -= 1;
                        if (attempts_max_times == 0) {
                            if (npc.GetComponent<NPCInnerVoice>()) npc.GetComponent<NPCInnerVoice>().isOccupied = false;
                            TimeManager.Instance.ResumeGameWhileTryApi();
                        }
                    }
                    else
                    {
                        // if (attempts_max_times != 10) {
                        //     TimeManager.Instance.ResumeGameWhileTryApi();
                        // }
                        TimeManager.Instance.ResumeGameWhileTryApi();
                        is_success = true;
                        Response responseJson = JsonUtility.FromJson<Response>(www.downloadHandler.text);

                        string responseContent = responseJson.content;
                        apiCall.response = responseContent;

                        completion_tokens += responseJson.completion_tokens;
                        prompt_tokens += responseJson.prompt_tokens;
                        total_tokens += responseJson.total_tokens;
                        apiResponseNum += 1;

                        Debug.Log(responseContent);
                        SaveCmdLog();
                        if (npc == null) {
                            break;
                        }
                        if (mode == "default") {
                            npc.GetComponent<NPCInnerVoice>().AddChatHistory(new ChatConfig(Role.NPC, responseContent));
                            StartCoroutine(InstructionExecutor.Instance.ExecuteCmd(responseContent, npc));
                        }
                        else if (mode == "talk") {
                            StartCoroutine(InstructionExecutor.Instance.ExecuteTalkCmd(responseContent, npc, instruction));
                        }
                        else if (mode == "manager_reflection") {
                            string reflection = InstructionExecutor.Instance.instructionParser.ParseReflection(responseContent);
                            Debug.Log(reflection);
                            if (reflection != null) {
                                if (npc != null) {
                                    npc.GetComponent<NPCInnerVoice>().reflectionList.Add(reflection);
                                    NPCManagerAgent.Instance.latestReflectionList.Add("To " + npc.GetComponent<NPCInnerVoice>().npcInfo.npcName + ": " + reflection);
                                }
                            }
                        }
                        else if (mode == "manager_action") {
                            ManagerInfoJson managerInfoJson = InstructionExecutor.Instance.instructionParser.ParseManagerAction(responseContent);
                            NPCManagerAgent.Instance.ExecuteManagerAction(managerInfoJson);
                        }
                    }
                }
            }
        }
        finally {
            callback?.Invoke();
        }
    }


    void OnApplicationQuit()
    {
        if (TaskManager.Instance != null && TaskManager.Instance.gameMode == GameMode.Task) {
            Debug.Log("playing mode ended, saving log...");
            runningTime = TimeManager.Instance.secondTimer;
            // Debug.Log(currentLogDirectory);
            SaveCmdLog();
            SaveResLog();
            NPCCreator.Instance.RecordCustomerStartEndTime();
            SaveFinalLog();
            Debug.Log("log save finished!");
        }
    }

    private class Request
    {
        public string prompt;
    }

    private class Response
    {
        public string content;
        public int completion_tokens;
        public int prompt_tokens;
        public int total_tokens;
    }
}
