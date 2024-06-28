using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskConfigUI : MonoBehaviour
{
    public TMP_Dropdown modeDropdown;
    public TMP_Dropdown taskLevelDropdown;
    public TMP_Dropdown taskIndexDropdown;
    public TMP_Dropdown strategyDropdown;
    public GameObject taskLevelDropdownObject;
    public GameObject taskIndexDropdownObject;
    public GameObject parentGameobject;

    private Dictionary<int, List<int>> taskIndexOptions = new Dictionary<int, List<int>>()
    {
        { 1, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } },
        { 2, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } },
        { 3, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } },
        { 4, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 } },
        { 5, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 } },
        { 6, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 } },
        { 7, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 } },
        { 8, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 } },
        { 9, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 } },
    };

    void Start()
    {
        modeDropdown.onValueChanged.AddListener(delegate { OnModeChanged(); });
        taskLevelDropdown.onValueChanged.AddListener(delegate { OnTaskLevelChanged(); });

        InitializeModeDropdown();
        InitializeStrategyDropdown();
        OnModeChanged(); // Initialize the dropdowns based on the initial mode value
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var (mode, taskLevel, taskIndex, strategy) = GetDropdownInfo();
            Debug.Log($"Mode: {mode}, Task Level: {taskLevel}, Task Index: {taskIndex}, Strategy: {strategy}");
        }
    }

    void InitializeModeDropdown()
    {
        List<string> options = new List<string> { "Task", "Challenge" };
        modeDropdown.ClearOptions();
        modeDropdown.AddOptions(options);
    }

    void InitializeStrategyDropdown()
    {
        List<string> options = new List<string> { "Centralized", "Decentralized", "Message Pool", "No Communication" };
        strategyDropdown.ClearOptions();
        strategyDropdown.AddOptions(options);
    }

    void InitializeTaskLevelDropdown()
    {
        List<string> options = new List<string>();
        for (int i = 1; i <= 9; i++)
        {
            options.Add(i.ToString());
        }
        taskLevelDropdown.ClearOptions();
        taskLevelDropdown.AddOptions(options);
    }

    void InitializeTaskIndexDropdown(int level)
    {
        List<string> options = new List<string>();
        foreach (var index in taskIndexOptions[level])
        {
            options.Add(index.ToString());
        }
        taskIndexDropdown.ClearOptions();
        taskIndexDropdown.AddOptions(options);
    }

    void OnModeChanged()
    {
        if (modeDropdown.value == 0) // Task mode
        {
            taskLevelDropdownObject.SetActive(true);
            taskIndexDropdownObject.SetActive(true);
            InitializeTaskLevelDropdown();
            OnTaskLevelChanged();
        }
        else // Challenge mode
        {
            taskLevelDropdownObject.SetActive(false);
            taskIndexDropdownObject.SetActive(false);
        }
    }

    void OnTaskLevelChanged()
    {
        int selectedLevel = taskLevelDropdown.value + 1; // Dropdown index starts at 0
        InitializeTaskIndexDropdown(selectedLevel);
    }

    public (string mode, string taskLevel, string taskIndex, string strategy) GetDropdownInfo()
    {
        string mode = modeDropdown.options[modeDropdown.value].text;
        string taskLevel = "";
        string taskIndex = "";
        string strategy = strategyDropdown.options[strategyDropdown.value].text;

        if (mode == "Task")
        {
            taskLevel = taskLevelDropdown.options[taskLevelDropdown.value].text;
            taskIndex = taskIndexDropdown.options[taskIndexDropdown.value].text;
        }

        return (mode, taskLevel, taskIndex, strategy);
    }

    public void LaunchTask()
    {
        if (TaskManager.Instance == null || NPCManager.Instance == null) return;
        var (mode, taskLevel, taskIndex, strategy) = GetDropdownInfo();
        if (strategy == "Centralized") NPCManager.Instance.cooperationStrategy = CooperationStrategy.Centralized;
        if (strategy == "Decentralized") NPCManager.Instance.cooperationStrategy = CooperationStrategy.Decentralized;
        if (strategy == "Message Pool") NPCManager.Instance.cooperationStrategy = CooperationStrategy.MessagePool;
        if (strategy == "No Communication") NPCManager.Instance.cooperationStrategy = CooperationStrategy.Single;
        TaskManager.Instance.LaunchTask(mode, taskLevel, taskIndex);
        CloseUI();
    }

    // Function to close the UI by setting its GameObject to false
    public void CloseUI()
    {
        parentGameobject.gameObject.SetActive(false);
    }

    public void OpenUI()
    {
        parentGameobject.gameObject.SetActive(true);
    }
}
