using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BackendConfigUI : MonoBehaviour
{
    public TMP_InputField backendUrlInputField;
    public Button confirmButton; 
    void Start()
    {
        // Add a listener to the confirm button to handle button clicks
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
    }

    public void OpenUI()
    {
       gameObject.SetActive(true);
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }

    private void OnConfirmButtonClick()
    {
        string backendUrl = backendUrlInputField.text;

        if (LMManager.Instance != null) {
            LMManager.Instance.baseInfo.llmBackendUrl = backendUrl;
        }
        CloseUI();
    }
}
