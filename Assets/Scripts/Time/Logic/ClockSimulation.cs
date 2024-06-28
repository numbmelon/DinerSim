using System;
using UnityEngine;
using UnityEngine.UI;

public class ClockSimulation : MonoBehaviour
{
    Image clockImage;   

    private float fillAmount = 0;

    private void Awake()
    {
        clockImage = GetComponent<Image>();
    }

    private void Update()
    {
        fillAmount = TimeManager.Instance.secondTimer * 1.0f / ChallengeLevelManager.Instance.GetLevelTimeLimit();

        if (fillAmount > 1)
            fillAmount = 0;

        clockImage.fillAmount = fillAmount;

        if (fillAmount <= 0.4f)
        {
            clockImage.color = Color.green;
        }
        else if (fillAmount > 0.4f && fillAmount <= 0.8f)
        {
            clockImage.color = Color.yellow;
        }
        else
        {
            clockImage.color = Color.red;
        }
    }
}
