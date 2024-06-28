using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelBoard : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI timeText;
    // public Button startBtn;

    void OnEnable()
    {
        levelText.text = "Level " + ChallengeLevelManager.Instance.GameLevel.ToString();
        // goalText.text = ChallengeLevelManager.Instance.GetLevelGoal().ToString();
        timeText.text = ChallengeLevelManager.Instance.GetLevelTimeLimit().ToString();
        
        // TimeManager.Instance.PauseGame();
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
