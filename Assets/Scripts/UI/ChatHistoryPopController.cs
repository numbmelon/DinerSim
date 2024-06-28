using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatHistoryPopController : MonoBehaviour
{
    public GameObject chatList;
    public GameObject sysChatSlot;
    public GameObject npcChatSlot;
    public GameObject nameItem;
    public GameObject nameListPanel;
    public GameObject chatDetailedInformation;
    public Dictionary<string, GameObject> chatDetailedInformationDict = new();
    public Dictionary<string, int> chatConfigNumDict = new();
    private GameObject currentChatDetailedInformation;

    void Awake() {
        UpdateChatConfig();
    }

    void Update()
    {
        UpdateChatConfig();
    }
    
    public void OpenPop()
    {
        chatList.SetActive(true);
        if (chatDetailedInformationDict.Values == null) return;
        foreach (GameObject chatInfo in chatDetailedInformationDict.Values) {
            if (currentChatDetailedInformation == chatInfo) {
                chatInfo.SetActive(true);
            }
            else {
                chatInfo.SetActive(false);
            }
        }
    }

    public void ClosePop()
    {
        chatList.SetActive(false);
    }

    public void UpdateChatConfig()
    {
        // TODO: UI 
        return;
        foreach (var npc in NPCManager.Instance.npcList) {
            if (string.IsNullOrEmpty(npc.npcInfo.npcName)) {
                continue;
            }
                
            if (!chatDetailedInformationDict.TryGetValue(npc.npcInfo.npcName, out GameObject chatInfoPanel)) {
                GameObject newNameItem = Instantiate(nameItem, nameListPanel.transform);
                newNameItem.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = npc.npcInfo.npcName;
                newNameItem.GetComponent<Button>().onClick.AddListener(() => ShowChatDetailForNPC(npc.npcInfo.npcName));

                GameObject newChatInfoPanel = Instantiate(chatDetailedInformation, chatList.transform);
                chatDetailedInformationDict[npc.npcInfo.npcName] = newChatInfoPanel;
                chatConfigNumDict[npc.npcInfo.npcName] = 0;

                // newNameItem.GetComponent<NameItemController>().chatDetailedInformation = newChatInfoPanel;
            }

            int currentCount = chatConfigNumDict[npc.npcInfo.npcName];
            if (currentCount < npc.npcInfo.chatHistory.Count) {
                Transform panelTransform = chatDetailedInformationDict[npc.npcInfo.npcName].transform.Find("ChatHolder").transform.Find("Panel");
                for (int i = currentCount; i < npc.npcInfo.chatHistory.Count; i++)
                {
                    ChatConfig chatConfig = npc.npcInfo.chatHistory[i];
                    GameObject chatSlotPrefab = chatConfig.role == Role.System ? sysChatSlot : npcChatSlot;
                    GameObject newChatSlot = Instantiate(chatSlotPrefab, panelTransform);
                    StartCoroutine(DisplayTextGradually(newChatSlot, chatConfig?.message));
                    // LayoutRebuilder.ForceRebuildLayoutImmediate(newChatSlot.GetComponent<RectTransform>());
                }

            }

            chatConfigNumDict[npc.npcInfo.npcName] = npc.npcInfo.chatHistory.Count;
        }

    }

    private void ShowChatDetailForNPC(string npcName)
    {
        currentChatDetailedInformation = chatDetailedInformationDict[npcName];
        foreach (GameObject chatInfo in chatDetailedInformationDict.Values) {
            if (currentChatDetailedInformation == chatInfo) {
                chatInfo.SetActive(true);
            }
            else {
                chatInfo.SetActive(false);
            }
        }
    }

    IEnumerator DisplayTextGradually(GameObject chatSlot, string fullText) {
        if (chatSlot == null) {
            yield break;
        }
        string currentText = "";
        chatSlot.GetComponentInChildren<TextMeshProUGUI>().text = "";
        if (chatSlot.GetComponent<ChatSlotController>() != null) {
            chatSlot.GetComponent<ChatSlotController>().UpdateTextAndButton(fullText, 50);
            LayoutRebuilder.ForceRebuildLayoutImmediate(chatSlot.GetComponent<RectTransform>());
            if (currentChatDetailedInformation != null) {
                LayoutRebuilder.ForceRebuildLayoutImmediate(currentChatDetailedInformation?.transform.Find("ChatHolder").transform.Find("Panel").gameObject.GetComponent<RectTransform>());
            }
            yield return null;
            if (currentChatDetailedInformation != null) {
                LayoutRebuilder.ForceRebuildLayoutImmediate(currentChatDetailedInformation?.transform.Find("ChatHolder").GetComponent<ScrollRect>().content);
            }

            yield break;
        }
        foreach (char c in fullText) {
            currentText += c;
            chatSlot.GetComponentInChildren<TextMeshProUGUI>().text += c;
            // if (currentText.Length % 50 == 0) 
            //     yield return null;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatSlot.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(currentChatDetailedInformation?.transform.Find("ChatHolder").transform.Find("Panel").gameObject.GetComponent<RectTransform>());
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(currentChatDetailedInformation?.transform.Find("ChatHolder").GetComponent<ScrollRect>().content);
        yield return null;
        // LayoutRebuilder.ForceRebuildLayoutImmediate(chatSlot.GetComponentInParent<RectTransform>());
    }
}
