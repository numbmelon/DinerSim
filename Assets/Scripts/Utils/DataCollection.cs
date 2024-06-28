using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WorldTime
{
    public int day, minute, second;

    public WorldTime(int day, int minute, int second) 
    {
        this.day = day;
        this.minute = minute;
        this.second = second;
    }
}

[System.Serializable]
public class TileDetails
{
    public int gridX, gridY;
    public bool isObstacle;
    public bool isCanDropItem;
    // public bool isWaitingForServe;
    public GameObject item;
    // TODO: more details
}


[System.Serializable]
public class FoodProperty
{
    public string foodName;
    public bool cuttable;
    public bool isCut;
    public Sprite sprite;
    public bool isCooked;
    public int coinValue;
    public bool spicy, salty, sour, sweet;
    public FoodProperty(string foodName, bool isCut) 
    {
        this.foodName = foodName;
        this.isCut = isCut;
    }

    public FoodProperty(string foodName, bool isCut, Sprite sprite) 
    {
        this.foodName = foodName;
        this.isCut = isCut;
        this.sprite = sprite;
    }

    public FoodProperty(string foodName, bool isCut, Sprite sprite, bool isCooked) 
    {
        this.foodName = foodName;
        this.isCut = isCut;
        this.sprite = sprite;
        this.isCooked = isCooked;
    }

    public FoodProperty(string foodName, bool cuttable, bool isCut, Sprite sprite, bool isCooked, int coinValue) 
    {
        this.foodName = foodName;
        this.cuttable = cuttable;
        this.isCut = isCut;
        this.sprite = sprite;
        this.isCooked = isCooked;
        this.coinValue = coinValue;
    }

    public bool EqualBaseProperty(FoodProperty other) 
    {
        return foodName == other.foodName && isCooked == other.isCooked;
    }

    public bool EqualProperty(FoodProperty other) 
    {
        return foodName == other.foodName && isCooked == other.isCooked && isCut == other.isCut;
    }

    public bool Season(string flavor) {
        switch (flavor) {
            case "spicy":
                spicy = true;
                break;
            case "salty":
                salty = true;
                break;
            case "sour":
                sour = true;
                break;
            case "sweet":
                sweet = true;
                break;
            default:
                return false;
        }
        return true;
    }
}


[System.Serializable]
public class Recipe
{
    public string dishName; 
    public List<FoodProperty> ingredients; 
    public FoodProperty result;
    public GameObject foodPrefab;
    public int cookingTime;
}


[System.Serializable]
public class TileProperty
{
    public Vector2Int tileCoordinate;
    public GridType gridType;
    public bool boolTypeValue;
}


[System.Serializable]
public class Seat 
{
    public Vector2Int chairGridPos;
    public bool isOccupied;
    public Desk desk;
    public GameObject npc;
    public string complaint="";

    public Seat(Vector2Int chairGridPos, bool isOccupied, Desk desk) 
    {
        this.chairGridPos = chairGridPos;
        this.isOccupied = isOccupied;
        this.desk = desk;
    }
}


[System.Serializable]
public class Order
{
    public GameObject npc;
    public Seat seat;
    public FoodProperty foodProperty;
}

[System.Serializable]
public class ChatConfig
{
    public Role role;
    public string message;
    // TODO: more info, e.g. time
    public ChatConfig(Role role, string message) 
    {
        this.role = role;
        this.message = message;
    }
}

[System.Serializable]
public class APICall
{
    public string npcName;
    public string prompt;
    public string response;
    public APICall(string npcName, string prompt, string response) 
    {
        this.npcName = npcName;
        this.prompt = prompt;
        this.response = response;
    }

    public object Clone() {
        APICall cloneObject = new APICall(this.npcName, this.prompt, this.response);
        return cloneObject;
    }
}

[System.Serializable]
public class JSONWithListOfAPICallObjects
{
    public List<APICall> apiCalls;
}

[System.Serializable]
public class JSONWithChallengeRecord
{
    public ChallengeLevelRecord challengeLevelRecord;
}

[System.Serializable]
public class CustomerRoutine
{
    public int arriveTime;
    public int leaveTime;

    public CustomerRoutine(int arriveTime, int leaveTime) {
        this.arriveTime = arriveTime;
        this.leaveTime = leaveTime;
    }
}

[System.Serializable]
public class JSONWithListOfCustomerRoutine
{
    public List<CustomerRoutine> routines;
}

[System.Serializable]
public class JSONWithListOfMetricsObjects
{
    public SummaryMetrics summaryMetrics;
    public List<Metrics> metricsList;
    
}

[System.Serializable]
public class SummaryMetrics
{
    public int totalOrdersNum;
    public int score;
    public int bonus;
    public int successOrder;
    public int perfectCompletedNum;
    public float averageSuccessRate;
    public float averagePerfectRate;
    public int maxSocre;
    public int maxBonus;
    // public float timeLimit;
    public int completedTaskNum;
    public int taskNum;
    public float averageRunningTime;

    public int completion_tokens;
    public int prompt_tokens;
    public int total_tokens;
    public int apiResponseNum;


    public SummaryMetrics(List<Metrics> metricsList)
    {
        totalOrdersNum = metricsList.Sum(m => m.totalOrdersNum);
        score = metricsList.Sum(m => m.score);
        bonus = metricsList.Sum(m => m.bonus);
        successOrder = metricsList.Sum(m => m.successOrder);
        
        maxSocre = metricsList.Sum(m => m.maxSocre);
        maxBonus = metricsList.Sum(m => m.maxBonus);
        taskNum = metricsList.Count;
        completedTaskNum = metricsList.Count(m => m.successOrder == m.totalOrdersNum);
        averageRunningTime = metricsList.Average(m => m.runningTime);

        completion_tokens = metricsList.Sum(m => m.completion_tokens);
        prompt_tokens = metricsList.Sum(m => m.prompt_tokens);
        total_tokens = metricsList.Sum(m => m.total_tokens);
        apiResponseNum = metricsList.Sum(m => m.apiResponseNum);
        perfectCompletedNum = metricsList.Sum(m => m.perfectCompletedNum);

        averageSuccessRate = metricsList.Average(m => m.successRate);
        averagePerfectRate = metricsList.Average(m => m.perfectRate);
    }
}


[System.Serializable]
public class Metrics
{
    public int totalOrdersNum;
    public int score;
    public int bonus;
    public int successOrder;
    public int maxSocre;
    public int maxBonus;
    public float timeLimit;
    public float runningTime;
    public string taskName;

    public int completion_tokens;
    public int prompt_tokens;
    public int total_tokens;
    public int apiResponseNum;

    public int perfectCompletedNum;
    public float successRate;
    public float perfectRate;

    public Metrics(int score, int bonus, int successOrder, int totalOrdersNum, int maxSocre, int maxBonus, float timeLimit, float runningTime, string taskName, int completion_tokens, int prompt_tokens, int total_tokens, int apiResponseNum, int perfectCompletedNum) {
        this.score = score;
        this.bonus = bonus;
        this.successOrder = successOrder;
        this.totalOrdersNum = totalOrdersNum;
        this.maxSocre = maxSocre;
        this.maxBonus = maxBonus;
        this.timeLimit = timeLimit;
        this.runningTime = runningTime;
        this.taskName = taskName;
        this.completion_tokens = completion_tokens;
        this.prompt_tokens = prompt_tokens;
        this.total_tokens = total_tokens;
        this.apiResponseNum = apiResponseNum;
        this.perfectCompletedNum = perfectCompletedNum;
        if (this.totalOrdersNum != 0) {
            this.successRate = this.successOrder * 1.0f / this.totalOrdersNum;
            this.perfectRate = this.perfectCompletedNum * 1.0f / this.totalOrdersNum;
        }
    }

    // public static Metrics Sum(List<Metrics> metricsList)
    // {
    //     int totalCoinNum = metricsList.Sum(m => m.score);
    //     int totalBonus = metricsList.Sum(m => m.bonus);
    //     int totalSuccessOrder = metricsList.Sum(m => m.successOrder);
    //     int totalTotalOrders = metricsList.Sum(m => m.totalOrdersNum);
    //     int totalmaxSocre = metricsList.Sum(m => m.maxSocre);
    //     int totalmaxBonus = metricsList.Sum(m => m.maxBonus);
    //     return new Metrics(totalCoinNum, totalBonus, totalSuccessOrder, totalTotalOrders, totalmaxSocre, totalmaxBonus);
    // }
}

[System.Serializable]
public class HistoryInstruction 
{
    public string instruction;
    public InstructionStatus status;
    public string message;
    public HistoryInstruction(string instruction, InstructionStatus status = InstructionStatus.Success, string message = "")
    {
        this.instruction = instruction;
        this.status = status;
        this.message = message;
    } 
    public string GetWithMes() 
    {
        return status switch
        {
            InstructionStatus.Success => instruction + " (Successfully executed)",
            InstructionStatus.Failed => instruction + " (Failed: " + message + ")",
            InstructionStatus.Running => instruction + " (Executing)",
            _ => "Unknown",
        };
    }
}

[System.Serializable]
public class ItemData
{
    public string item_type;
    public int[] pos;
    public int item_index;
    public ItemDataProperties properties; 

    public Vector3Int Position
    {
        get { return new Vector3Int(pos[0], pos[1], 0); }
    }

}

[System.Serializable]
public class ItemDataProperties
{
    public FoodProperty foodproperty;
     public int plate = -1;
}

[System.Serializable]
public class ItemDetailedData
{
    public int item_total_count;
    public Dictionary<string, int> item_counts_by_type;
    public List<ItemData> items;
}

// [System.Serializable]
// public class ItemTypeCnt
// {
//     public int food;
//     public int plate;
// }

[System.Serializable]
public class SceneData
{
    public ItemDetailedData items;
    public string map;
}

[System.Serializable]
public class OrderData
{
    public int priority;
    public bool wantsBeverage;
    public int seatIdx;
    public string foodType;
    public string diningOption;
    public List<string> flavors;
    public string flavorDescription;
}

[System.Serializable]
public class OrderInfo
{
    public int totalOrdersNum;
    public List<OrderData> orders;
}

[System.Serializable]
public class ActionData
{
    public string actionType;
    public string description;
}

[System.Serializable]
public class TraceData
{
    public string agentName;
    public List<ActionData> actions;
}

[System.Serializable]
public class TraceInfo
{
   public List<TraceData> traces;
}

[System.Serializable]
public class AgentData
{
    public string name;
    public string agentRole;
    public string agentBackground;
    public string agentCharacteristic;
    public string agentStatus;
    public int[] pos;
    public Vector3Int Position
    {
        get { return new Vector3Int(pos[0], pos[1], 0); }
    }
}

[System.Serializable]
public class AgentInfo
{
    public int agentNumber;
    public List<AgentData> agentConfigs;
    public string defaultSettings;
}

[System.Serializable]
public class TaskInfo
{
    public AgentInfo agentInfo;
    public SceneData sceneInfo;
    public OrderInfo orderInfo;
    public TraceInfo traceInfo;
    public float timeLimit;
    public string taskName;
    public List<string> specialEvents;
}

[System.Serializable]
public class SpecialEvent
{
    public string eventName;
    public string description;
    public bool isTemporary;
    public bool isNegative;
    public Action effect;
    public Action onExpire;

    public SpecialEvent(string eventName, string description, bool isTemporary = false, bool isNegative = false, Action effect = null, Action onExpire = null)
    {
        this.eventName = eventName;
        this.description = description;
        this.isTemporary = isTemporary;
        this.isNegative = isNegative;
        this.effect = effect;
        this.onExpire = onExpire;
    }

    public void TriggerEvent()
    {
        effect?.Invoke();
    }

    public void ExpireEvent()
    {
        onExpire?.Invoke();
    }
}

[System.Serializable]
public class CommunicationInfo
{
    public bool HasCommunicated { get; set; }

    public CommunicationInfo()
    {
        HasCommunicated = false;
    }
}

[System.Serializable]
public class SubscribedMessage
{
    public string sender;
    public string receiver;
    public string message;

    private string m_sender;
    private int npcIdx;

    public SubscribedMessage(string sender="", string receiver="", string message="")
    {
        this.sender = sender;
        this.receiver = receiver;
        this.message = message;
        m_sender = this.sender;
    }

    public SubscribedMessage(int npcIdx)
    {
        this.npcIdx = npcIdx;
    }

    public string GetMessage()
    {
        // string senderStr = isOwner ? "You" : m_sender;
        string senderStr = m_sender;
        if (NPCManager.Instance != null && NPCManager.Instance.npcList.Count > npcIdx) {
            senderStr = NPCManager.Instance.npcList[npcIdx].npcInfo.npcName;
        }
        string mes = $@"{{
    sender: {senderStr},
    receiver: {receiver},
    message: {message}
}}";
        return mes;
    }
}

[System.Serializable]
public class PeerToPeerMessage
{
    public int npcIdx;
    public string message;

    public PeerToPeerMessage(int npcIdx, string message = "")
    {
        this.npcIdx = npcIdx;
        this.message = message; 
    }

    public string GetMessage()
    {;
        return message;
    }

}

[System.Serializable]
public class PeerToPeerMessageJson
{
    public string receiver;
    public string message;

    public PeerToPeerMessageJson(string receiver, string message)
    {
        this.receiver = receiver;
        this.message = message;
    }
}

[System.Serializable]
public class ReflectionJson
{
    public string reflection;
    public ReflectionJson(string reflection)
    {
        this.reflection = reflection;
    }
}

[System.Serializable]
public class ManagerInfoJson
{
    public int eventIdx;
    public int recruitIdx;
    public int dismissIdx;
    public int strategyIdx;
    public ManagerInfoJson(int eventIdx, int recruitIdx, int dismissIdx, int strategyIdx) {
        this.eventIdx = eventIdx;
        this.recruitIdx = recruitIdx;
        this.dismissIdx = dismissIdx;
        this.strategyIdx = strategyIdx;
    }
}
[System.Serializable]
public class ChallengeLevelRecord
{
    public int Revenue;
    public int GameLevel;
    public int CurrentOrderFinishNum;
    public int LeftCoinNum;
    public ChallengeLevelRecord(int Revenue, int GameLevel, int CurrentOrderFinishNum, int LeftCoinNum) {
        this.Revenue = Revenue;
        this.GameLevel = GameLevel;
        this.CurrentOrderFinishNum = CurrentOrderFinishNum;
        this.LeftCoinNum = LeftCoinNum;
    }
}

[System.Serializable]
public class NPCInfoSimple
{
    public string Name { get; set; }
    public string Characteristic { get; set; }
    public RestaurantRole Role { get; set; }

    public NPCInfoSimple(string name, string characteristic, RestaurantRole role)
    {
        Name = name;
        Characteristic = characteristic;
        Role = role;
    }
}