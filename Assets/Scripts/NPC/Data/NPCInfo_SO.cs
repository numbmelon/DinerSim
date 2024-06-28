using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCInfo_SO", menuName = "NPC Schedule/NPCInfo")]
public class NPCInfo_SO : ScriptableObject
{
    public string npcName;
    public RestaurantRole npcRole;

    [TextArea] public string npcBackground;
    [TextArea] public string npcCharacteristic;
    [TextArea] public string npcStatus;

    public List<HistoryInstruction> historyInstructions = new();
    public List<string> actionList = new();
    public List<ChatConfig> chatHistory = new();
}
