using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PauseInformation : MonoBehaviour
{
    TextMeshProUGUI textMeshProUGUI;
    void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeManager.Instance != null) {
            if (TimeManager.Instance.IsPaused()) {
                if (TimeManager.Instance.pauseSignalApiNum > 0) {
                    textMeshProUGUI.text = "Paused due to waiting for LLM API Response";
                }
                else if (TimeManager.Instance.pauseSignalNum > 0) {
                    textMeshProUGUI.text = "Paused due to waiting for function call";
                }
                else {
                    textMeshProUGUI.text = "Paused";
                }
            }
            else {
                textMeshProUGUI.text = "";
            }
        }
    }


}
