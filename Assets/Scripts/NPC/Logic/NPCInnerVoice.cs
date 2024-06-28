using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
// using UnityEngine.UI;

public class NPCInnerVoice : MonoBehaviour
{
    public NPCInfo_SO npcInfo_origin;
    public NPCInfo_SO npcInfo;

    public float npcTakeActionCd;
    private float npcTakeActionTik = 2f;
   
    public bool enableAgent;
    public bool isOccupied;

    public int npcIdx;
    public List<string> reflectionList;

    void Awake()
    {
        npcInfo = CopyNPCInfo(npcInfo_origin);
    }

    void Start()
    {
        ClearAllData();
        // if (npcInfo.npcRole == RestaurantRole.Cook) npcInfo.npcStatus = "ready to cook some dish";
        // if (npcInfo.npcRole == RestaurantRole.Waiter) npcInfo.npcStatus = "ready to take out dish from oder_pickup_area and serve customers";
    }   

    void Update()
    {
        if (TimeManager.Instance.IsPaused()) return;
        if (NPCManager.Instance == null) return;
        if (enableAgent == false || isOccupied == true) {
            return;
        }

        npcTakeActionTik -= Time.deltaTime;
        if (npcTakeActionTik <= 0) {
            TakeAction();
            npcTakeActionTik = npcTakeActionCd + Settings.extraActionTik;
        }
    }

    public NPCInfo_SO CopyNPCInfo(NPCInfo_SO original)
    {
        NPCInfo_SO copy = ScriptableObject.CreateInstance<NPCInfo_SO>();
        copy.npcName = original.npcName;
        copy.npcRole = original.npcRole;
        copy.npcBackground = original.npcBackground;
        copy.npcStatus = original.npcStatus;
        copy.npcCharacteristic= original.npcCharacteristic;
        return copy;
    }

    void OnEnable()
    {
        StartCoroutine(AddInnerVoiceObj());
        EventHandler.BeforeEnterNextLevelEvent += ClearAllData;
        EventHandler.BeforeNextChallengeDayEvent += ClearAllData;
    }

    void OnDisable()
    {
        // NPCManager.Instance?.npcList.Remove(this);
        EventHandler.BeforeEnterNextLevelEvent -= ClearAllData;
        EventHandler.BeforeNextChallengeDayEvent -= ClearAllData;
    }

    IEnumerator AddInnerVoiceObj()
    {
        while (NPCManager.Instance == null) yield return null;
        NPCManager.Instance.AddInnerVoiceObj(this);
    }

    public void ClearAllData()
    {
        if (npcInfo != null) {
            npcInfo.historyInstructions.Clear();
            npcInfo.actionList.Clear();
            npcInfo.chatHistory.Clear();
        }

        isOccupied = false;
    }

    public void AddChatHistory(ChatConfig chatConfig)
    {
        npcInfo.chatHistory.Add(chatConfig);
    }

    private string GetBackgroundPrompt()
    {
        string agentBackground = npcInfo.npcName + ", a " + npcInfo.npcRole.ToString() + ", " + npcInfo.npcBackground + "\nReading all the following sections beign with \"******[SECTION NAME]*******\"carefully. After you get all the necessary details, move to the \"******Task******\" section to complete your goal.";

        return agentBackground; 
    }

    private string GetCharacterPrompt()
    {
        return Settings.characterSettingSectionTitle + npcInfo.npcCharacteristic;
    }

    private string GetStatusPrompt()
    {
        string extraMes = "";
        Vector2Int pos = (Vector2Int)GridMapManager.Instance.grid.WorldToCell(transform.position);
        extraMes += "\nYour Position: " + pos.ToString() + "\nItem you are holding: ";

        GameObject item = gameObject?.GetComponentInChildren<ItemPickUp>().holdItem;
        if (item == null) {
            extraMes += "null\n";
        } 
        else {
            extraMes += item.GetComponent<Item>().GetMessage() + "\n";
        }
        return Settings.characterStatusSectionTitle + npcInfo.npcStatus + extraMes;
    }

    private string GetOtherAgentStatus()
    {
        string mes = "";
        List<string> mesList = new();
        if (NPCManager.Instance?.npcList.Count > 1) {
            mes += Settings.otherAgentStatusSectionTitle;
            foreach (NPCInnerVoice npc in NPCManager.Instance.npcList) {
                string agentMes = "";
                if (npc != this) {
                    if (npc == null) continue;
                    agentMes += "Name: " + npc.npcInfo.npcName + "\n";
                    agentMes += "Role: " + npc.npcInfo.npcRole.ToString() + "\n";
                    agentMes += "Status: " + npc.npcInfo.npcStatus + "\n";
                    Vector2Int pos = (Vector2Int)GridMapManager.Instance.grid.WorldToCell(npc.transform.position);
                    agentMes += "Pos: " + pos.ToString() + "\n";
                    mesList.Add(agentMes);
                }
            }
            mes += String.Join("\n", mesList);
        }
        else {
            return "";
        }
        
        return mes;
    }

    private string GetItemAtHandPrompt()
    {
        GameObject item = gameObject?.GetComponentInChildren<ItemPickUp>().holdItem;
        string mes;
        if (item == null) {
            mes = "null";
        } 
        else {
            mes = item.GetComponent<Item>().GetMessage();
        }
        return Settings.itemAtHandSectionTitle + mes;
    }

    private string GetItemInformationPrompt()
    {
        string itemInformation = Settings.itemInformationSectionTitle;
        itemInformation += "This section shows information about all the items you can see in the current scene, in the form of \"ITEM_NAME (STATUS) {POSITION}\"\n";
        itemInformation += ItemManager.Instance.GetAllItemMessage(npcInfo.npcRole);
        return itemInformation;
    }

    private string GetHistoryInstructionPrompt()
    {
        string instructions = Settings.characterHistorySectionTitle;
        if (npcInfo.historyInstructions.Count == 0) {
            instructions += "null"; 
        }
        else {
            instructions += string.Join("\n", npcInfo.historyInstructions.Select(hi => hi.GetWithMes()));
        }
        return instructions;
    }

    public string GetBasicAndObservationPrompt(bool getCharacter = true, bool getItem = true, bool getOtherAgentStatus = true, bool getItemAtHand = true, bool getOrders = true, bool getHistory = true, bool getReflection = true)
    {
        string res = "";
        List<string> mesList = new();
        if (getCharacter) {
            string backgroundPrompt = GetBackgroundPrompt();
            mesList.Add(backgroundPrompt);
            string characterPrompt = GetCharacterPrompt();
            mesList.Add(characterPrompt);
        }
        string statusPrompt = GetStatusPrompt();
        mesList.Add(statusPrompt);

        if (npcInfo.npcRole == RestaurantRole.Common || npcInfo.npcRole == RestaurantRole.Cook) {
            string recipe = Settings.recipeSectionTitle;
            if (TaskManager.Instance.gameMode == GameMode.Task) {
                int levelIdx = Math.Min(TaskManager.Instance.taskLevel, Settings.dishPerLevelList.Length);
                recipe += "In this level, the customer can order these dishes: " + Settings.dishPerLevelList[levelIdx - 1] + "\n";
            }
            recipe +=  Settings.recipeStr;
            mesList.Add(recipe);
        }

        if (getOtherAgentStatus && NPCManager.Instance != null && NPCManager.Instance.cooperationStrategy != CooperationStrategy.Single) {
            string otherAgentStatusPrompt = GetOtherAgentStatus();
            if (otherAgentStatusPrompt != "") mesList.Add(otherAgentStatusPrompt);
        }
        if (getItemAtHand) {
            string itemPrompt = GetItemAtHandPrompt();
            mesList.Add(itemPrompt);
        }
        if (getItem) {
            string itemInformationPrompt = GetItemInformationPrompt();
            mesList.Add(itemInformationPrompt);
        }
        if (getOrders) {
            if (npcInfo.npcRole == RestaurantRole.Waiter || npcInfo.npcRole == RestaurantRole.Common) {
                string orderInformationPrompt = Settings.seatsRequireServiceSectionTitle + OrderManager.Instance.GetAllOrderMessage();
                mesList.Add(orderInformationPrompt);
            }
            if (npcInfo.npcRole == RestaurantRole.Cook) {
                string orderedDishInformationPrompt = Settings.ordersSectionTitle + OrderManager.Instance.GetAllOrderedDishNameAndCount();
                mesList.Add(orderedDishInformationPrompt);
            }
        }
        if (getHistory) {
            string historyInstructionPrompt = GetHistoryInstructionPrompt();
            mesList.Add(historyInstructionPrompt);
        }
        if (getReflection && TaskManager.Instance && TaskManager.Instance.gameMode == GameMode.Challenge && reflectionList.Count > 0) {
            mesList.Add(Settings.managerSuggestionSectionTitle + String.Join("\n", reflectionList));
        }
        res = String.Join("\n\n", mesList);
        if (res == "") res = "null";
        return res;
    }

    public string GetMessagePoolMes()
    {
        string mes = Settings.messagePoolSectionTitle + "The Message Pool facilitates cooperation effectively. In this section, you can see the latest subscribe-publish messages sent by you and other agents, helping to determine each other's needs and tasks, formatted as\n";
        mes += @"{
""sender"": ""SENDER_NAME"",
""receiver"": ""RECEIVER_NAME"",
""message"": ""MESSAGE""
}
Then, Here is the Message Pool currently:
";
        string messagePoolmes = NPCManager.Instance.GetAllMessagePoolMes(npcIdx);
        mes += messagePoolmes;
        if (messagePoolmes == "") mes += "null";
        return mes;
    }

    public string GetDecentralizedMes()
    {
        string mes = Settings.peerToPeerMessageSectionTitle + "In the Peer-To-Peer Messages Section, you can see the interactions between you and other agents. In this section, you can see the latest messages sent by you and other agents, helping to determine each other's needs and tasks, formatted as\n";
        mes += @"{
- To AGENT_NAME: MESSAGE_1
- From AGENT_NAME: MESSAGE_2
}
Then, Here is the Peer-To-Peer Messages Section currently:
";
        string peerToPeerMessage = NPCManager.Instance.GetNPCDecentralizedMes(npcIdx);
        mes += peerToPeerMessage;
        return mes;
    }


    public string GetCentralizedBegin()
    {
        string mes = Settings.managerGuideStartString;
        mes += NPCManager.Instance.GetAllCentralizedSuggestionsMes();
        mes += "\nBased on this information, now you need to continue to guide " + npcInfo.npcName + " now, following is all the information this agent could get:\n";
        return mes;
    }

    public string GenerateActionPrompt()
    {
        string  prompt = "";
        if (NPCManager.Instance.cooperationStrategy == CooperationStrategy.Centralized) {
            prompt += GetCentralizedBegin();
        }
        prompt += GetBasicAndObservationPrompt();
        if (NPCManager.Instance.cooperationStrategy == CooperationStrategy.MessagePool) {
            prompt = prompt + "\n\n" + GetMessagePoolMes();
        }
        else if (NPCManager.Instance.cooperationStrategy == CooperationStrategy.Decentralized) {
            prompt = prompt + "\n\n" + GetDecentralizedMes();
        }

        if (OrderManager.Instance) {
            prompt = prompt + "\n\n" + Settings.customerComplaintSectionTitle + OrderManager.Instance.GetCustomerComplaint();
        }
        if (npcInfo.npcRole == RestaurantRole.Cook) {
            prompt = prompt + "\n\n" + Settings.cookActionsAvailable;
        }
        else { 
            if (npcInfo.npcRole == RestaurantRole.Waiter) {
                prompt = prompt + "\n\n" + Settings.waiterActionsAvailable;
            }
            else if (npcInfo.npcRole == RestaurantRole.Common) {
                prompt = prompt + "\n\n" + Settings.singleAgentActionsAvailable;
            }

            if (TaskManager.Instance.taskLevel >= 6 || TaskManager.Instance.gameMode == GameMode.Challenge) {
                prompt += "[TalkAt] item_name //  talk to customer sitting at the place named item_name (e.g. desk)";
            }
        }
        // TODO: some optional action maybe

        return prompt;
    }

    public void TakeAction()
    {
        isOccupied = true;
        string prompt = GenerateActionPrompt();
        if (npcInfo.npcRole == RestaurantRole.Cook) {
            if (NPCManager.Instance.cooperationStrategy == CooperationStrategy.Centralized) {
                prompt = prompt + "\n" + Settings.managerGuideCookTaskBegin;
            }
            else {
                prompt = prompt + "\n" + Settings.npcCookTakeActionStringBegin;
            }
            if (NPCManager.Instance.cooperationStrategy == CooperationStrategy.MessagePool) {
                prompt = prompt + "3. (Optional) " + Settings.messagePoolPromptingStr;
            }
            else if (NPCManager.Instance.cooperationStrategy == CooperationStrategy.Decentralized) {
                prompt = prompt + "3. (Optional) " + Settings.decentralizedPromptingStr;
            }

            prompt = prompt + Settings.npcCookTakeActionStringExamples + Settings.npcCookTakeActionStringEnd;
        }
        else if (npcInfo.npcRole == RestaurantRole.Waiter) {
            if (NPCManager.Instance.cooperationStrategy == CooperationStrategy.Centralized) {
                prompt = prompt + "\n" + Settings.managerGuideWaiterTaskBegin;
            }
            else {
                prompt = prompt + "\n" + Settings.npcWaiterTakeActionStringBegin;
            }
            if (NPCManager.Instance.cooperationStrategy == CooperationStrategy.MessagePool) {
                prompt = prompt + "3. (Optional) " + Settings.messagePoolPromptingStr;
            }
            else if (NPCManager.Instance.cooperationStrategy == CooperationStrategy.Decentralized) {
                prompt = prompt + "3. (Optional) " + Settings.decentralizedPromptingStr;
            }
            prompt = prompt + Settings.npcWaiterTakeActionStringExamples + Settings.npcWaiterTakeActionStringEnd;
        }
        else if (npcInfo.npcRole == RestaurantRole.Common) {
            prompt = prompt + "\n" + Settings.singleAgentTakeActionString;
        }

        if (NPCManager.Instance.cooperationStrategy == CooperationStrategy.Centralized) {
            prompt += "(Summarize your thoughts into one clear sentence that can be used as your suggestion. Remember, you are the manager. You do not execute actions yourself; you just generate various instructions for all other agents.)\n";
        }
        
        // Debug.Log("call api");
        StartCoroutine(LMManager.Instance.CreateAPICall(prompt, gameObject));
    }
}
