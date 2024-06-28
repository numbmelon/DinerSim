using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DateBoard : MonoBehaviour
{
    public TextMeshProUGUI DateText;
    // Start is called before the first frame update
    void Start()
    {
        // DateText = GetComponent<TextMeshProUGUI>();
        EventHandler.GameSecondEvent += UpdateText;
    }

    // Update is called once per frame
    void Update()
    {
        // if (ChallengeLevelManager.Instance != null) {
        //     DateText.text = "Day " + ChallengeLevelManager.Instance.GameLevel.ToString();
        // }
    }

    private void OnDestroy()
    {
        EventHandler.GameSecondEvent -= UpdateText;
    }

    private void UpdateText(int second)
    {
        // if (DateText != null) DateText.text = second.ToString();
        if (DateText != null && ChallengeLevelManager.Instance != null)
        {
            int totSecond = (int)ChallengeLevelManager.Instance.GetLevelTimeLimit() - second;

            // Calculate minutes and seconds
            int minutes = totSecond / 60;
            int seconds = totSecond % 60;

            // Format as "MM:SS"
            DateText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
