using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ChatSlotController : MonoBehaviour
{
    public TextMeshProUGUI chatText;
    public Button showDetailsButton;
    public string textContent;

    void Start()
    {

    }

    public void UpdateTextAndButton(string textContent, int maxLength)
    {
        this.textContent = textContent;
        if (textContent.Length > maxLength)
        {
            chatText.text = textContent[..maxLength];
            chatText.enableWordWrapping = false;
            chatText.overflowMode = TextOverflowModes.Ellipsis;           
            showDetailsButton.gameObject.SetActive(true);
        }
        else
        {
            showDetailsButton.gameObject.SetActive(false);
        }
    }

    public void OnShowDetailsClicked()
    {
        chatText.overflowMode = TextOverflowModes.Overflow;
        chatText.enableWordWrapping = true;
        showDetailsButton.gameObject.SetActive(false);
    }
}
