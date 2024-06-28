using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InstructionExecutor : Singleton<InstructionExecutor>
{
    public InstructionParser instructionParser = new();
    Dictionary<GameObject, int> cmdRemainingDict = new();

    // Start is called before the first frame update
    void Start()
    {
        EventHandler.AfterCompleteCurrentLevelEvent += StopAllRunningCoroutines;
        EventHandler.BeginChallengeEvent += StopAllRunningCoroutines;
        EventHandler.AfterCurrentChallengeDayEvent += StopAllRunningCoroutines;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopAllRunningCoroutines()
    {
        // TODO: fix this
        // this.StopAllCoroutines();
        // cmdRemainingDict = new();
    }

    public IEnumerator ExecuteTalkCmd(string cmd, GameObject customer, HistoryInstruction instruction = null)
    {
        string order = instructionParser.ParseOrder(cmd);
        if (order != null) {
            if (DishManager.Instance.GetRecipe(order) != null) {
                customer.GetComponent<Customer>().orderedDish = order;
            }
            else {
                customer.GetComponent<Customer>().orderedDish = DishManager.Instance.GetRandomDishName();
            }
            CmdComplete(customer.GetComponent<Customer>().talkingToWaiter);
            customer.GetComponent<Customer>().Status = CustomerStatus.WaitingForDish;
            if (instruction != null) instruction.status = InstructionStatus.Success;
        }
        else {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "customer failed to return a legal order, using a random dish instead.";
            }
        }
        
        yield return null;
    }
    
    public IEnumerator ExecuteCmd(string cmd, GameObject npc)
    {
        if (npc == null) yield break;
        string newStatus = instructionParser.ParseStatus(cmd);
        if (newStatus != null) {
            npc.GetComponent<NPCInnerVoice>().npcInfo.npcStatus = newStatus;
        }


        if (NPCManager.Instance != null && NPCManager.Instance.cooperationStrategy == CooperationStrategy.MessagePool) {
            string messagepoolStr = instructionParser.ParseMessagePool(cmd);
            if (messagepoolStr != null) {
                Debug.Log(messagepoolStr);
                if (!messagepoolStr.Contains("<messagepool>")) {
                    try {
                        SubscribedMessage subscribedMessage = JsonUtility.FromJson<SubscribedMessage>(messagepoolStr);
                        int npcIdx = npc.GetComponent<NPCInnerVoice>().npcIdx;
                        NPCManager.Instance.messagePool[npcIdx].receiver = subscribedMessage.receiver;
                        NPCManager.Instance.messagePool[npcIdx].message = subscribedMessage.message;
                    }
                    catch (Exception ex) {
                        Debug.LogWarning("error occurred while parsing the logs" + ex.Message);
                    }
                }
            }
        }
        else if (NPCManager.Instance != null && NPCManager.Instance.cooperationStrategy == CooperationStrategy.Centralized) {
            string suggestion = instructionParser.ParseSuggestion(cmd);
            if (suggestion != null) {
                Debug.Log(suggestion);
                int npcIdx = npc.GetComponent<NPCInnerVoice>().npcIdx;
                NPCManager.Instance.npcManagerSuggestions[npcIdx] = suggestion;
            }
        }
        else if (NPCManager.Instance != null && NPCManager.Instance.cooperationStrategy == CooperationStrategy.Decentralized) {
            List<string> messageList = instructionParser.ParseMessage(cmd);
            if (messageList != null) {
                int npcIdx = npc.GetComponent<NPCInnerVoice>().npcIdx;
                foreach (string message in messageList) {
                    try {
                        PeerToPeerMessageJson peerToPeerMessage = JsonUtility.FromJson<PeerToPeerMessageJson>(message);
                        int receiverIdx = NPCManager.Instance.GetNPCIdxByName(peerToPeerMessage.receiver);
                        string senderMessage = peerToPeerMessage.message;
                        NPCManager.Instance.AddNPCDecentralizedMes(npcIdx, receiverIdx, senderMessage);
                    }
                    catch (Exception ex) {
                        Debug.LogWarning("error occurred while parsing the logs" + ex.Message);
                    }
                }
            }
        }

 
        List<string> actions = instructionParser.ParseAction(cmd);

        if (npc == null) yield break;

        if (cmdRemainingDict.GetValueOrDefault(npc, -1) == -1) {
            if (npc == null) yield break;
            cmdRemainingDict.Add(npc, actions.Count);
        }
        else {
            cmdRemainingDict[npc] = actions.Count;
        }
        
        int cnt = cmdRemainingDict[npc];
        foreach (var _action in actions) {
            ExecuteSingleAction(_action, npc);
            while (npc != null && cmdRemainingDict[npc] == cnt) {
                yield return new WaitForFixedUpdate();
                if (npc == null) yield break;
            }
            if (npc == null) yield break;
            if(npc.GetComponent<NPCInnerVoice>().npcInfo.historyInstructions.Count>0 && npc.GetComponent<NPCInnerVoice>().npcInfo.historyInstructions[^1].status == InstructionStatus.Failed) {
                cmdRemainingDict[npc] = 0;
                npc.GetComponent<NPCInnerVoice>().npcInfo.npcStatus += "\nThis status might be wrong, since the last instruction(s) you generate went wrong:\n" + npc.GetComponent<NPCInnerVoice>().npcInfo.historyInstructions[^1].GetWithMes();
                break;
            }
            cnt = cmdRemainingDict[npc];
        }
        npc.GetComponent<NPCInnerVoice>().isOccupied = false;
        yield return null;
    }

    public void ExecuteSingleAction(string cmd, GameObject npc) 
    {
        string[] parts = cmd.Split(' ');
        string action = parts[0].ToLower();
        HistoryInstruction instruction = new (cmd, InstructionStatus.Running);
        npc.GetComponent<NPCInnerVoice>().npcInfo.historyInstructions.Add(instruction);
        if (npc.GetComponent<NPCInnerVoice>().npcInfo.historyInstructions.Count > Settings.maxHistoryNum) {
            npc.GetComponent<NPCInnerVoice>().npcInfo.historyInstructions.RemoveAt(0);
        }

        if (action.Equals("[pickup]")) {
            if (parts.Length < 2) {
                CmdSynatxError(npc, instruction);
                return;
            }
            ExecutePickUp(instruction, npc, parts[1].Trim());
        }
        else if (action.Equals("[putinto]")) {
            if (parts.Length < 2) {
                CmdSynatxError(npc, instruction);
                return;
            }
            ExecutePutInto(instruction, npc, parts[1].Trim());
        }
        else if (action.Equals("[takeout]")) {
            if (parts.Length < 2) {
                CmdSynatxError(npc, instruction);
                return;
            }
            if (parts.Count() > 2) {
                ExecuteTakeOut(instruction, npc, parts[1].Trim(), parts[2].Trim());
            }
            else {
                ExecuteTakeOut(instruction, npc, parts[1].Trim());
            }
        }
        else if (action.Equals("[cut]")) {
            if (parts.Length < 2) {
                CmdSynatxError(npc, instruction);
                return;
            }
            ExecuteCut(instruction, npc, parts[1].Trim());
        }
        else if (action.Equals("[season]")) {
            if (parts.Length < 3) {
                CmdSynatxError(npc, instruction);
                return;
            }
            ExecuteSeason(instruction, npc, parts[1].Trim(), parts[2].Trim());
        }
        else if (action.Equals("[drop]")) {
            // TODO: delete this action later
            ExecuteDrop(instruction, npc);
        }
        else if (action.Equals("[open]")) {
            if (parts.Length < 2) {
                CmdSynatxError(npc, instruction);
                return;
            }
            ExecuteOpen(instruction, npc, parts[1].Trim());
        }
        else if(action.Equals("[close]")) {
            if (parts.Length < 2) {
                CmdSynatxError(npc, instruction);
                return;
            }
            ExecuteClose(instruction, npc, parts[1].Trim());
        }
        else if(action.Equals("[talkat]")) {
            if (parts.Length < 2) {
                CmdSynatxError(npc, instruction);
                return;
            }
            ExecuteTalkAt(instruction, npc, parts[1].Trim());
        }
        else if (action.Equals("[donothing]")) {
            ExecuteDoNothing(instruction, npc);
        }
        else if (action.Equals("[moveto]")) {
            if (parts.Length < 2) {
                CmdSynatxError(npc, instruction);
                return;
            }
            ExecuteMoveTo(instruction, npc, parts[1].Trim());
        }
        else {
            instruction.status = InstructionStatus.Failed;
            instruction.message = "unknown action " + parts[0];
            CmdComplete(npc);
        }
    }

    void CmdSynatxError(GameObject npc, HistoryInstruction instruction)
    {
        instruction.status = InstructionStatus.Failed;
        instruction.message = "syntax error";
        CmdComplete(npc);
    }

    public void ExecuteMoveTo(HistoryInstruction instruction, GameObject npc, string itemName)
    {
        GameObject item = ItemManager.Instance.itemDict.GetValueOrDefault(itemName, null);
        if (item == null) {
            instruction.message = "item " + itemName + " not exists";
            instruction.status = InstructionStatus.Failed;
            CmdComplete(npc);
            return;
        }
        if (IsItemDetectedByNpc(npc, item)) {
            CmdComplete(npc, instruction);
        }
        else {
            MoveToItem(npc, item, () => CmdComplete(npc, instruction));
        }
    }

    public void ExecuteDrop(HistoryInstruction instruction, GameObject npc)
    {
        GameObject holditem = npc.GetComponentInChildren<ItemPickUp>().holdItem;
        if (holditem == null) {
            instruction.message = "You're not holding any items.";
            instruction.status = InstructionStatus.Failed;
            CmdComplete(npc);
            return;
        }
        npc.GetComponentInChildren<ItemDrop>()?.DropItem();
        instruction.status = InstructionStatus.Success;
        CmdComplete(npc);

    }
    public void ExecutePickUp(HistoryInstruction instruction, GameObject npc, string itemName)
    {
        GameObject item = ItemManager.Instance.itemDict.GetValueOrDefault(itemName, null);
        if (item == null || !item.GetComponent<Item>().pickable) {
            instruction.message = item == null ? "item " + itemName + " not exists" : "item " + itemName + " is unpickable";
            if (item != null) {
                if (item.GetComponent<PlateStack>() != null || item.GetComponent<CoffeeMachine>() != null || item.GetComponent<Pan>() != null|| item.GetComponent<OrderPickupArea>() != null) {
                    instruction.message += ", do you mean [TakeOut] " + itemName + "?";
                }
            }
            instruction.status = InstructionStatus.Failed;
            CmdComplete(npc);
            return;
        }
        else if (item.GetComponent<Item>().ispicked) {
            instruction.message = "item has been a picked status, check the scene infomation to find more details.";
            instruction.status = InstructionStatus.Failed;
            CmdComplete(npc);
            return;
        }


        if (IsItemDetectedByNpc(npc, item)) {
            PickUp(npc, item, instruction);
        }
        else {
            MoveToItem(npc, item, () => PickUp(npc, item, instruction));
        }
    }

    public void ExecutePutInto(HistoryInstruction instruction, GameObject npc, string itemName)
    {
        GameObject item = ItemManager.Instance.itemDict.GetValueOrDefault(itemName, null);
        GameObject holditem = npc.GetComponentInChildren<ItemPickUp>().holdItem;
        if (item == null || holditem == null) {
            instruction.message = item == null ? "item " + itemName + " not exists" : "You're not holding any items.";
            instruction.status = InstructionStatus.Failed;
            CmdComplete(npc);
            return;
        }

        if (IsItemDetectedByNpc(npc, item)) {
            PutInto(holditem, item, npc, instruction);
        }
        else {
            MoveToItem(npc, item, () => PutInto(holditem, item, npc, instruction));
        }
    }

    public void ExecuteTakeOut(HistoryInstruction instruction, GameObject npc, string itemName, string insideItemName = null) 
    {
        GameObject item = ItemManager.Instance.itemDict.GetValueOrDefault(itemName, null);
        GameObject holditem = npc.GetComponentInChildren<ItemPickUp>().holdItem;
        if (item == null) {
            instruction.message = "item " + itemName + " not exists";
            instruction.status = InstructionStatus.Failed;
            CmdComplete(npc);
            return;
        }

        if (IsItemDetectedByNpc(npc, item)) {
            TakeOut(holditem, item, npc, insideItemName, instruction);
        }
        else {
            MoveToItem(npc, item, () => TakeOut(holditem, item, npc, insideItemName, instruction));
        }
    }

    public void ExecuteCut(HistoryInstruction instruction, GameObject npc, string itemName)
    {
        GameObject item = ItemManager.Instance.itemDict.GetValueOrDefault(itemName, null);
        GameObject holditem = npc.GetComponentInChildren<ItemPickUp>().holdItem;
        if (item == null) {
            instruction.message = "item " + itemName + " not exists";
            instruction.status = InstructionStatus.Failed;
            CmdComplete(npc);
            return;
        }
        if (IsItemDetectedByNpc(npc, item)) {
            // Debug.Log(22222222);
            Cut(item, npc, instruction, () => CmdComplete(npc, instruction));
        }
        else {
            MoveToItem(npc, item, () => Cut(item, npc, instruction, () => CmdComplete(npc, instruction)));
        }
    }

    public void ExecuteSeason(HistoryInstruction instruction, GameObject npc, string itemName, string flavor)
    {
        GameObject item = ItemManager.Instance.itemDict.GetValueOrDefault(itemName, null);
        GameObject holditem = npc.GetComponentInChildren<ItemPickUp>().holdItem;
        Food food = null;
        if (item == null) {
            instruction.message = "item " + itemName + " not exists";
            instruction.status = InstructionStatus.Failed;
            CmdComplete(npc);
            return;
        }
        if (holditem == null) {
            instruction.message = "You're not holding any items.";
            instruction.status = InstructionStatus.Failed;
            CmdComplete(npc);
            return;
        }
        else if (holditem.GetComponentInChildren<Plate>() != null) {
            if (holditem.GetComponentInChildren<Plate>().food == null) {
                instruction.message = "You're holding an empty plate, no food for seasoning";
                instruction.status = InstructionStatus.Failed;
                CmdComplete(npc);
                return;
            }
            else {
                food = holditem.GetComponentInChildren<Plate>().food;
            }
        }
        if (food == null) {
            food = holditem.GetComponentInChildren<Food>();
        }
        if (IsItemDetectedByNpc(npc, item)) {
            Season(food, item, npc, flavor, instruction);
        }
        else {
            MoveToItem(npc, item, () => Season(food, item, npc, flavor, instruction));
        }
    }

    public void ExecuteDoNothing(HistoryInstruction instruction, GameObject npc) 
    {
        instruction.status = InstructionStatus.Success;
        CmdComplete(npc);
    }

    public void ExecuteOpen(HistoryInstruction instruction, GameObject npc, string itemName) 
    {
        GameObject item = ItemManager.Instance.itemDict.GetValueOrDefault(itemName, null);
        if (item == null) {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "item " + itemName + " not exists";
            }
            CmdComplete(npc);
            return;
        }

        if (IsItemDetectedByNpc(npc, item)) {
            Open(item, npc, instruction);
        }
        else {
            MoveToItem(npc, item, () => Open(item, npc, instruction));
        }
    }

    public void ExecuteClose(HistoryInstruction instruction, GameObject npc, string itemName)
    {
        GameObject item = ItemManager.Instance.itemDict.GetValueOrDefault(itemName, null);
        if (item == null) {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "item " + itemName + " not exists";
            }
            CmdComplete(npc);
            return;
        }

        if (IsItemDetectedByNpc(npc, item)) {
            Close(item, npc, instruction);
        }
        else {
            MoveToItem(npc, item, () => Close(item, npc, instruction));
        }
    }

    public void ExecuteTalkAt(HistoryInstruction instruction, GameObject npc, string itemName)
    {
        GameObject item = ItemManager.Instance.itemDict.GetValueOrDefault(itemName, null);
        if (item == null) {
            instruction.message = "item " + itemName + " not exists";
            instruction.status = InstructionStatus.Failed;
            CmdComplete(npc);
            return;
        }

        if (IsItemDetectedByNpc(npc, item)) {
            Talk(item, npc, instruction);
            // CmdComplete(npc);
        }
        else {
            MoveToItem(npc, item, () => Talk(item, npc, instruction));
        }
        

    }

    public bool IsItemDetectedByNpc(GameObject npc, GameObject item)
    {
        GameObject itemHolder = npc.GetComponentInChildren<ItemPickUp>().gameObject;
        RaycastHit2D[] hits = Physics2D.RaycastAll(itemHolder.transform.position + new Vector3(0, 0, 2), new Vector3(0, 0, -1), 10);
        
        foreach (RaycastHit2D hit in hits) {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject == item) {
                return true;
            }
        }
        return false;
    }


    public void MoveToItem(GameObject npc, GameObject item, Action action)
    {
        Vector3Int targetGridPos = GridMapManager.Instance.GetReachablePosition(GridMapManager.Instance.grid.WorldToCell(item.transform.position));

        npc.GetComponent<NPC>().FindPath((Vector2Int)targetGridPos);
        StartCoroutine(npc.GetComponent<NPC>().Movement(action));
    }

    void PickUp(GameObject npc, GameObject item, HistoryInstruction instruction = null) 
    {
        npc.GetComponentInChildren<ItemPickUp>().PickUpTarget(item);
        // if (item.GetComponent<Item>().ispicked) {
        //     instruction.message = "item has been picked by other agents";
        //     instruction.status = InstructionStatus.Failed;
        //     CmdComplete(npc);
        //     return;
        // }
        if (instruction != null) instruction.status = InstructionStatus.Success;
        CmdComplete(npc);
    }

    void PutInto(GameObject holditem, GameObject item, GameObject npc, HistoryInstruction instruction = null) 
    {
        if (holditem == null) {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "You're not holding any items.";
            }
            CmdComplete(npc);
            return;
        }

        // TODO: simplify or reconstruct [Pickup] logics

        if (!((item.GetComponent<Pan>() != null || item.GetComponent<ChoppingBoard>() != null) && holditem.GetComponentInChildren<Plate>() != null)) {
            npc.GetComponentInChildren<ItemPickUp>().holdItem = null;
        }
        item.GetComponent<Item>().PutInto(holditem, instruction);
        if (instruction != null && instruction.status == InstructionStatus.Running) {
            instruction.status = InstructionStatus.Success;
        }
        else if (instruction != null && !((item.GetComponent<Pan>() != null || item.GetComponent<ChoppingBoard>() != null) && holditem.GetComponentInChildren<Plate>() != null)) {
            npc.GetComponentInChildren<ItemPickUp>().holdItem = holditem; // pick up again if failed to [PutInto]
        }
        CmdComplete(npc);
    }

    void TakeOut(GameObject holdItem, GameObject item, GameObject npc, string insideItemName, HistoryInstruction instruction = null) 
    {
        item.GetComponent<Item>().TakeOut(holdItem, npc, insideItemName, instruction);
         if (instruction != null) {
            if (instruction.status == InstructionStatus.Running) {
                instruction.status = InstructionStatus.Success;
            }
         }
        CmdComplete(npc);
    }

    void Season(Food food, GameObject item, GameObject npc, string flavor, HistoryInstruction instruction = null) 
    {
        item.GetComponent<Item>().Season(food, flavor, instruction);
         if (instruction != null) {
            if (instruction.status == InstructionStatus.Running) {
                instruction.status = InstructionStatus.Success;
            }
         }
        CmdComplete(npc);
    }

    void Cut(GameObject item, GameObject npc, HistoryInstruction instruction, Action action) {
        StartCoroutine(item.GetComponent<ChoppingBoard>()?.Cut(instruction, action));
    }

    void Open(GameObject item, GameObject npc, HistoryInstruction instruction = null)
    {
        item.GetComponent<Item>().Open();
        if (instruction != null) instruction.status = InstructionStatus.Success;
        CmdComplete(npc);
    }

    void Close(GameObject item, GameObject npc, HistoryInstruction instruction = null)
    {
        item.GetComponent<Item>().Close();
        if (instruction != null) instruction.status = InstructionStatus.Success;
        CmdComplete(npc);
    }

    void Talk(GameObject item, GameObject npc, HistoryInstruction instruction = null) {
        if (item.GetComponent<Desk>() != null) {
            item.GetComponent<Desk>().isTalking = true;
            item.GetComponent<Desk>().talkingToWaiter = npc;
        }
        else {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "no such seats";
            }
        }
    }

    public void CmdComplete(GameObject npc, HistoryInstruction instruction = null) 
    {
        if (cmdRemainingDict.GetValueOrDefault(npc, -1) == -1) {
            return;
        }
        cmdRemainingDict[npc]--;

        // if (cmdRemainingDict[npc] <= 0) {
        //     npc.GetComponent<NPCInnerVoice>().isOccupied = false;
        // }
         if (instruction != null) {
            if (instruction.status == InstructionStatus.Running) {
                instruction.status = InstructionStatus.Success;
            }
         }
    }
}
