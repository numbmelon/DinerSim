using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    public List<NPCInnerVoice> npcList;

    public Dictionary<string, Dictionary<string, CommunicationInfo> > communicationLog = new();

    public Dictionary<int, SubscribedMessage> messagePool = new();
    public Dictionary<int, Dictionary<int, PeerToPeerMessage> > decentralizedCommunicationLog = new();

    public CooperationStrategy cooperationStrategy; 
    public int npcIdx = 0; 

    public List<string> npcManagerSuggestions = new(); 

    void Start()
    {
        EventHandler.BeforeEnterNextLevelEvent += InitNPCManager;
        EventHandler.AfterCurrentChallengeDayEvent += AfterCurrentDay;
        EventHandler.BeforeNextChallengeDayEvent += InitBeforeNextDay;
        EventHandler.BeginChallengeEvent += InitNPCManager;
    }

    public void AddInnerVoiceObj(NPCInnerVoice npcInnerVoice)
    {
        npcList.Add(npcInnerVoice);
        npcInnerVoice.npcIdx = npcIdx;
        // Debug.Log(npcInnerVoice.npcInfo.npcName);
        messagePool[npcIdx] = new SubscribedMessage(npcIdx);
        decentralizedCommunicationLog[npcIdx] = new();
        npcManagerSuggestions.Add("null");
        npcIdx += 1;
    }

    void InitNPCManager()
    {
        messagePool = new();
        npcList = new();
        communicationLog = new();
        npcManagerSuggestions = new();
        decentralizedCommunicationLog = new();
        npcIdx = 0;
    }

    void AfterCurrentDay()
    {
        CoinManager.Instance.CoinNum -= Math.Max(0, Settings.dailyWage - Settings.dailyWageDecrease + Settings.dailyWageIncrease + Settings.permanentWageIncrease) * npcList.Count;
        CoinManager.Instance.CoinNum -= Math.Max(0, Settings.dailyRent + Settings.permanentRentIncrease + Settings.dailyWageIncrease);
        NPCManagerAgent.Instance.latestReflectionList = new();
        foreach (NPCInnerVoice npcInnerVoice in npcList) {
            if (npcInnerVoice != null) NPCManagerAgent.Instance.GiveReflection(npcInnerVoice);
        }
    }

    void InitBeforeNextDay()
    {
        InitNPCManager();
        GameObject npcParent = GameObject.Find("NPCs");
        npcParent.SetActive(false);
        npcParent.SetActive(true);
    }


    public void DeleteAgent(NPCInnerVoice npcInnerVoice)
    {
        TimeManager.Instance.PauseGame();
        GameObject npcParent = GameObject.Find("NPCs");
        InitNPCManager();

        // Find and delete the agent
        foreach (Transform child in npcParent.transform)
        {
            NPCInnerVoice childNpcInnerVoice = child.GetComponent<NPCInnerVoice>();
            if (childNpcInnerVoice != null && childNpcInnerVoice == npcInnerVoice)
            {
                Destroy(child.gameObject);
                break;
            }
        }

        npcParent.SetActive(false);
        npcParent.SetActive(true);
        TimeManager.Instance.ResumeGame();
    }

    public string GetAllCentralizedSuggestionsMes()
    {
        string mes = "";
        List<string> mesList = new();
        for (int i = 0; i < npcIdx; i++) {
            mesList.Add("- " + npcList[i].npcInfo.npcName + " (" + npcList[i].npcInfo.npcRole.ToString() + "): "  + npcManagerSuggestions[i]);
        }
        mes = string.Join("\n", mesList);
        return mes;
    }

    public string GetAllMessagePoolMes(int npcIdx = -1)
    {
        string mes = "";
        List<string> mesList = new();
        foreach (var entry in messagePool) {
            if (entry.Value.message != "") {
                mesList.Add(entry.Value.GetMessage());
            }
        }
        mes = string.Join("\n", mesList);
        return mes;
    }

    public void AddNPCDecentralizedMes(int senderIdx, int receiverIdx, string message) 
    {
        if (!decentralizedCommunicationLog[senderIdx].ContainsKey(receiverIdx)) {
            decentralizedCommunicationLog[senderIdx][receiverIdx] = new PeerToPeerMessage(receiverIdx);
        }
        decentralizedCommunicationLog[senderIdx][receiverIdx].message = message;
    }

    public string GetNPCDecentralizedMes(int npcIdx = -1)
    {
        if (npcIdx == -1) return "";
        // string npcName = npcList[npcIdx].npcInfo.npcName;
        List<string> mesList = new();
        for (int i = 0; i < this.npcIdx; i++) {
            if (i == npcIdx) continue;
            string mes = "{\n";
            string receiver = npcList[i].npcInfo.npcName;
            mes += "- To " + receiver + ": ";
            if (decentralizedCommunicationLog[npcIdx].ContainsKey(i)) {
                mes += decentralizedCommunicationLog[npcIdx][i].GetMessage();
            }
            mes += "\n";
            mes += "- From " + receiver + ": ";
            if (decentralizedCommunicationLog[i].ContainsKey(npcIdx)) {
                mes += decentralizedCommunicationLog[i][npcIdx].GetMessage();
            }
            mes += "\n}";
            mesList.Add(mes);
        }

        return string.Join("\n", mesList);
    }

    public int GetNPCIdxByName(string npcName) 
    {
        for (int i = 0; i < npcIdx; i++) {
            if (npcList[i].npcInfo.npcName == npcName) return i;
        }
        return -1;
    }


    void InitCommunicationLog()
    {
        communicationLog = new Dictionary<string, Dictionary<string, CommunicationInfo>>();

        // Initialize the communication log for all NPCs
        foreach (NPCInnerVoice npc in npcList)
        {
            communicationLog[npc.npcInfo.npcName] = new Dictionary<string, CommunicationInfo>();
            foreach (NPCInnerVoice otherNpc in npcList)
            {
                if (npc != otherNpc)
                {
                    communicationLog[npc.npcInfo.npcName][otherNpc.npcInfo.npcName] = new CommunicationInfo();
                }
            }
        }
    }

}
