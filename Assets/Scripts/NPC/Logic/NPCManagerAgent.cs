using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NPCManagerAgent : Singleton<NPCManagerAgent>
{
    public List<string> latestReflectionList = new();
    public List<SpecialEvent> currentEvents = new();

    public List<NPCInfoSimple> npcInfoSimples = new();
    void Start()
    {
        
    }

    void Update()
    {

    }

    void InitManager()
    {
        latestReflectionList = new();
    }

    public void GiveReflection(NPCInnerVoice npcInnerVoice) 
    {
        string basicPrompt = Settings.managerChallengeLevelBegin + "\nNow you need to evaluate the actions of an agent and provide feedback. Below is the detailed information of the agent for the day:\n\n" + npcInnerVoice.GetBasicAndObservationPrompt(true, false, false, false, false, true, true) + "\n\n";
        basicPrompt += @"Now please think step by step first, and then provide a reflection within 1-2 sentences in the following JSON format:
<reflection>
{
""reflection"": ""REFLECTION""
}
</reflection>";
        TimeManager.Instance.PauseGame();
        StartCoroutine(LMManager.Instance.CreateAPICall(basicPrompt, npcInnerVoice.gameObject, "manager_reflection", null, ()=>TimeManager.Instance.ResumeGame()));
    }

    public void ManagerTakeAction()
    {
        string basicPrompt = Settings.managerChallengeLevelBegin + "\n\n";

        basicPrompt += "#### Current Day's Coin Number:\n" + "It is Day " + ChallengeLevelManager.Instance.GameLevel + ", you have " + CoinManager.Instance.CoinNum.ToString() +" coins left.\n\n#### Previous Day's Employee Reflection:\n";
        basicPrompt += string.Join("\n", latestReflectionList) == "" ? "null" : string.Join("\n", latestReflectionList);
        basicPrompt += "\n\nAdditionally, each day starts with an event generation phase. The manager needs to select the most beneficial event for the restaurant from three random events. Each day also includes one fixed random negative event to increase the challenge.\n\n### Current Event Pool:\n";
        basicPrompt += TaskManager.Instance.specialEventPool.GetCurrentEventPoolString() + "\n### Today's Event Choices:\n";
        currentEvents = TaskManager.Instance.specialEventPool.GetRandomNonNegativeEvents(3);
        for (int i = 0; i < currentEvents.Count; i++){
            basicPrompt += $"{i + 1}. {currentEvents[i].eventName}: {currentEvents[i].description}\n";
        }
        basicPrompt += "\n### Recruitment and Dismissal:\nRecruiting an employee costs 150 coins. Here is today's list of potential recruits. You can choose to recruit one or none:\n";

        npcInfoSimples = new();
        for(int i = 0; i < 3; i++) {
            npcInfoSimples.Add(AgentCreator.Instance.GetRandomNPC());
            basicPrompt += $"{i + 1}. {npcInfoSimples[i].Name}, {npcInfoSimples[i].Role}, {npcInfoSimples[i].Characteristic}\n"; 
        }


        basicPrompt += "\nHere is the current employee list. You can choose to dismiss one employee or none. Note that your restaurant requires at least one waiter and one cook to operate:\n";
        for(int i = 0; i < NPCManager.Instance.npcList.Count; i++) {
            basicPrompt += $"{i + 1}. {NPCManager.Instance.npcList[i].npcInfo.npcName}: {NPCManager.Instance.npcList[i].npcInfo.npcRole}, {NPCManager.Instance.npcList[i].npcInfo.npcCharacteristic}\n";
        }
        basicPrompt += $"\n### Cooperation Strategies:\nHere are the three cooperation strategies. The current strategy is {NPCManager.Instance.cooperationStrategy.ToString()}, you can keep it or change it:\n";
        basicPrompt += @"1. Decentralized Peer-to-Peer Cooperation: In this method, agents communicate freely with one another in a peer-to-peer manner. In each action round, each agent can freely choose its communication partner(s) and engage in one-on-one interactions, storing the communication information exchanged with other agents.
2. Centralized Manager-Driven Cooperation: In a centralized mode, communication and actions are mediated by a manager agent. The manager directs the actions of the agents based on the overall status of the restaurant, specific agent information, and the manager's past guidance.
3. Shared Message Pool: Agents communicate through a subscribe-publish message pool, where messages are visible to all agents, facilitating transparent and real-time information sharing.

### Return Your Choices:
Return your final choices in the following JSON format, enclosed within ""<manager>xxx</manager>"". If no changes are made, use index 0 for the respective field.
<manager>
{
""eventIdx"": INDEX,
""recruitIdx"": INDEX,
""dismissIdx"": INDEX,
""strategyIdx"": INDEX
}
</manager>

Please provide 1-2 sentences to explain your thought process and return the instruction in a valid format:
";
        TimeManager.Instance.PauseGame();
        StartCoroutine(LMManager.Instance.CreateAPICall(basicPrompt, gameObject, "manager_action", null, ()=>TimeManager.Instance.ResumeGame()));

    }

    public void ExecuteManagerAction(ManagerInfoJson managerInfoJson)
    {
        int eventIdx = managerInfoJson.eventIdx;
        if (eventIdx != 0 && eventIdx <= currentEvents.Count) {
            TaskManager.Instance.specialEventPool.AddEvent(currentEvents[eventIdx - 1]);
        }
        

        int dismissIdx = managerInfoJson.dismissIdx;
        if (dismissIdx != 0 && dismissIdx <= NPCManager.Instance.npcList.Count) {
            NPCManager.Instance.DeleteAgent(NPCManager.Instance.npcList[dismissIdx - 1]);
        }

        int recruitIdx = managerInfoJson.recruitIdx;
        if (recruitIdx != 0 && recruitIdx <= NPCManager.Instance.npcList.Count) {
            AgentCreator.Instance.AddAgent(npcInfoSimples[recruitIdx - 1]);
        }


        int strategyIdx = managerInfoJson.strategyIdx;
        if (strategyIdx == 1) {
            NPCManager.Instance.cooperationStrategy = CooperationStrategy.Decentralized;
        }
        if (strategyIdx == 2) {
            NPCManager.Instance.cooperationStrategy = CooperationStrategy.Centralized;
        }
        if (strategyIdx == 3) {
            NPCManager.Instance.cooperationStrategy = CooperationStrategy.MessagePool;
        }
    }

}
