using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
// using System.Threading.Tasks;
public class NPCCreator : Singleton<NPCCreator>
{
    public List<Vector2Int> npcCreationGridPosList;

    public List<GameObject> npcPrefabList;
    public float creationCoolDown = 1f;
    private System.Random random = new System.Random();
    public List<CustomerRoutine> customerRoutineList = new();

    private float timetik;

    public int npcNum;
    public int totNum;

    public List<GameObject> npcList;

    public TextAsset jsonData;
    public OrderInfo orderInfo;

    public int createPosIdx = 0;
    public int totalOrdersNum = -1; 
    public int maxSocre;
    public int maxBonus;
    public int perfectCompletedOrderNum = 0;

    public int currentPriority = 1; 

    private List<int> priorityTotNumList = new ();
    public List<int> priorityCompletedNumList = new();

    void Start()
    {
        npcCreationGridPosList = new List<Vector2Int> {
            new Vector2Int(30, 12),
            new Vector2Int(32, 5),
            new Vector2Int(29, 16),
            new Vector2Int(33, 5)
        };

        npcNum = 0;
        totNum = 0;
    
        EventHandler.AfterCompleteCurrentLevelEvent += ClearCustomer;
        EventHandler.BeginChallengeEvent += ClearCustomerChallengeLevel;
        EventHandler.AfterCurrentChallengeDayEvent += ClearCustomerChallengeLevel;
        // EventHandler.BeforeEnterNextLevelEvent += InitNPCCreator;
        // StartCoroutine(LoadJsonFile());
    }

    public void ClearCustomer()
    {
        GameObject npcParent = GameObject.Find("Customers");
        if (npcParent != null) {
            foreach (Transform child in npcParent.transform)
            {
                Customer customer = child.GetComponent<Customer>();
                if (customer != null) {
                    customer.StopAllCoroutines();
                }
                Destroy(child.gameObject);
            }
        }
        RecordCustomerStartEndTime();
        customerRoutineList = new();
    }

    public void ClearCustomerChallengeLevel()
    {
        GameObject npcParent = GameObject.Find("Customers");
        if (npcParent != null) {
            foreach (Transform child in npcParent.transform)
            {
                Customer customer = child.GetComponent<Customer>();
                if (customer != null) {
                    customer.StopAllCoroutines();
                }
                Destroy(child.gameObject);
            }
        }
        npcNum = 0;
        totNum = 0;
        createPosIdx = 0;
        // maxSocre = 0;
        // maxBonus = 0;
        priorityTotNumList = new();
        priorityCompletedNumList = new();
        timetik = 0;
        currentPriority = 1;
        customerRoutineList = new();
    }



    public void RecordCustomerStartEndTime()
    {
        string customerStartEndTimeFilePath = Path.Combine(LMManager.Instance.currentLogDirectory, "customer_start_end.json");
        JSONWithListOfCustomerRoutine jsonWithListRoutineObjects = new()
        {
            routines = customerRoutineList
        };
        string json = JsonUtility.ToJson(jsonWithListRoutineObjects, true);
        File.WriteAllText(customerStartEndTimeFilePath, json);
    }


    public void InitNPCCreator(OrderInfo orderInfo)
    {
        npcNum = 0;
        totNum = 0;
        createPosIdx = 0;
        maxSocre = 0;
        maxBonus = 0;
        // GameObject npcParent = GameObject.Find("Customers");
        // if (npcParent != null) {
        //     foreach (Transform child in npcParent.transform)
        //     {
        //         Destroy(child.gameObject);
        //     }
        // }
        priorityTotNumList = new();
        priorityCompletedNumList = new();
        timetik = 0;
        currentPriority = 1;
        LoadOrderData(orderInfo);
        // StartCoroutine(LoadJsonFile());
    }

    public void InitNPCCreatorChallenge(OrderInfo orderInfo)
    {
        npcNum = 0;
        totNum = 0;
        createPosIdx = 0;
        maxSocre = 0;
        maxBonus = 0;
        // GameObject npcParent = GameObject.Find("Customers");
        // if (npcParent != null) {
        //     foreach (Transform child in npcParent.transform)
        //     {
        //         Destroy(child.gameObject);
        //     }
        // }
        priorityTotNumList = new();
        priorityCompletedNumList = new();
        timetik = 0;
        // StartCoroutine(LoadJsonFile());
    }

    // public void InitializeOrder(OrderInfo orderInfo)
    // {
    //     return;
    // }

    public void GetCurrentTotNum()
    {
        LMManager.Instance.totalOrdersNum = totalOrdersNum;
        LMManager.Instance.maxBonus = maxBonus;
        LMManager.Instance.maxSocre = maxSocre;
    }

    public void CreateNPC(OrderData orderData)
    {
        Vector2Int spawnPos = npcCreationGridPosList[createPosIdx];
        createPosIdx = (createPosIdx + 1) % npcCreationGridPosList.Count;


        GameObject npcPrefab = npcPrefabList[random.Next(npcPrefabList.Count)];
        GameObject npcInstance = Instantiate(npcPrefab, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
        GameObject npcParent = GameObject.Find("Customers");
        if (npcParent != null)
        {
            npcInstance.transform.SetParent(npcParent.transform);
        }
        npcInstance.GetComponent<Customer>().customerId = totNum;
        customerRoutineList.Add(new CustomerRoutine(-1, -1));

        npcNum += 1;
        totNum += 1;
        npcList.Add(npcInstance);

        OrderManager.Instance.InitCustomer(npcInstance, orderData);
    }

    void Update()
    {
        if (!TimeManager.Instance.IsPaused()) {
            timetik -= Time.deltaTime;
        }

        if (timetik <= 0 && TaskManager.Instance != null) {
            if (TaskManager.Instance.gameMode == GameMode.Task) {
                if (totNum < orderInfo.totalOrdersNum && npcNum < 7) {
                    if (orderInfo.orders[totNum].priority == currentPriority) {
                        timetik = creationCoolDown;
                        CreateNPC(orderInfo.orders[totNum]);
                    }
                }
            }
            else if (TaskManager.Instance.gameMode == GameMode.Challenge && ChallengeLevelManager.Instance != null) {
                if (npcNum < 7 && totNum < ChallengeLevelManager.Instance.GetLevelCustomerNum()) {
                    timetik = creationCoolDown;
                    CreateNPC(ChallengeLevelManager.Instance.GetLevelRandomOrderData());
                }
            }
            // if (npcNum < 5 && totNum < LevelManager.Instance.GetLevelCustomerNum()) {
            //     CreateNPC();
            // }
        }
    }

    public void LoadOrderData(OrderInfo orderInfo)
    {
        if (orderInfo == null) return;
        SceneData sceneData = TaskManager.Instance.taskInfo.sceneInfo;
        Dictionary<string, int> foodCountDict = new Dictionary<string, int>();
        foreach (ItemData item in sceneData.items.items)
        {
            if (item.item_type == "food")
            {
                string foodName = item.properties.foodproperty.foodName;
                if (foodCountDict.ContainsKey(foodName))
                {
                    foodCountDict[foodName]++;
                }
                else
                {
                    foodCountDict[foodName] = 1;
                }
            }
        }
        this.orderInfo = orderInfo;
        totalOrdersNum = orderInfo.totalOrdersNum;
        foreach (OrderData orderData in orderInfo.orders) {
            if (orderData.flavors != null && orderData.flavors.Count > 0) {
                // maxSocre += 5;
                maxBonus += 5;
            }
            if (orderData.foodType == "none") {
                // TODO: change this to serach max value dish
                maxSocre += 34;  // 40 - 3 -3 
                maxBonus += 8;
            }
            else {
                Recipe recipe = DishManager.Instance.GetRecipe(orderData.foodType);
                int coinValue =  recipe.result.coinValue;
                maxSocre += coinValue;
                foreach (FoodProperty foodProperty in recipe.ingredients) {
                    string foodName = foodProperty.foodName;
                    if (foodCountDict.ContainsKey(foodName) && foodCountDict[foodName] > 0)
                    {
                        foodCountDict[foodName]--;
                    }
                    else
                    {
                        maxSocre -= foodProperty.coinValue;
                    }
                }

                maxBonus += (int)(coinValue * 0.2f);
            }
            if (orderData.wantsBeverage) {
                // 5 - 1
                maxSocre += 4;

                maxBonus += 1;
            }

            int priority = orderData.priority;
            while (priorityTotNumList.Count < priority) priorityTotNumList.Add(0);
            priorityTotNumList[priority - 1] += 1;
        }
        maxSocre += maxBonus;
        while (priorityCompletedNumList.Count < priorityTotNumList.Count) {
            priorityCompletedNumList.Add(0);
        }
        GetCurrentTotNum();
    }
    // IEnumerator LoadJsonFile()
    // {
    //     TimeManager.Instance.PauseGame();
    //     jsonData = Resources.Load<TextAsset>("order_info_level1_" + LevelManager.Instance.dataIdx.ToString());
    //     if (jsonData == null) yield return new WaitForFixedUpdate();
    //     if (jsonData != null) {
    //         orderInfo = JsonUtility.FromJson<OrderInfo>(jsonData.text); 
    //         while (orderInfo == null) yield return null;
    //         totalOrdersNum = orderInfo.totalOrdersNum;
    //         foreach (OrderData orderData in orderInfo.orders) {
    //             Recipe recipe = DishManager.Instance.GetRecipe(orderData.foodType);
    //             int coinValue =  recipe.result.coinValue;
    //             maxSocre += coinValue;
    //             if (LevelManager.Instance.GameLevel > 5) {
    //                 foreach (FoodProperty foodProperty in recipe.ingredients) {
    //                     maxSocre -= foodProperty.coinValue;
    //                 }
    //             }
    //             maxBonus += (int)(coinValue * 0.2f);
    //             int priority = orderData.priority;
    //             while (priorityTotNumList.Count < priority) priorityTotNumList.Add(0);
    //             priorityTotNumList[priority - 1] += 1;
    //         }
    //         maxSocre += maxBonus;
    //         while (priorityCompletedNumList.Count < priorityTotNumList.Count) {
    //             priorityCompletedNumList.Add(0);
    //         }
    //     }
    //     GetCurrentTotNum();
    //     TimeManager.Instance.ResumeGame();
    // }

    public void CustomerFinishOrder(OrderData orderData, bool perfectCompleted) {
        if (priorityCompletedNumList.Count == 0 || orderData.priority > priorityCompletedNumList.Count) return;
        priorityCompletedNumList[orderData.priority - 1] += 1;
        if (priorityCompletedNumList[orderData.priority - 1] == priorityTotNumList[orderData.priority - 1]) {
            currentPriority += 1;
        }
        // TaskManager.Instance.currentOrderFinishNum += 1;
        if (perfectCompleted) {
            LMManager.Instance.perfectCompletedNum += 1;
        }
    }

}
