using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCreator : Singleton<AgentCreator>
{
    public GameObject cookPrefab;
    public GameObject waiterPrefab;
    public GameObject commonAgentPrefab;

    private List<string> names = new List<string>
    {
        "Charlie", "David", "Eve", "Frank", "Grace", "Hank", "Ivy", "Jack",
    "Kara", "Leo", "Mona", "Nina", "Oscar", "Paul", "Quinn", "Rose", "Sam", "Tina",
    "Uma", "Vince", "Wendy", "Xander", "Yara", "Zane", "Amber", "Bruno", "Clara", "Derek",
    "Elaine", "Finn", "Gina", "Harry", "Isla", "Jake", "Kelsey", "Liam", "Mila", "Nolan"
    };

    private List<string> characteristics = new List<string>
    {
        "good at cooking", "bad at cooking", "friendly", "rude", "quick learner", "slow learner",
        "hardworking", "lazy", "good communicator", "poor communicator", "detail-oriented", 
        "careless", "team player", "selfish", "creative", "unimaginative", "efficient", 
        "inefficient", "organized", "disorganized"
    };

    public NPCInfoSimple GetRandomNPC()
    {
        if (names.Count == 0 || characteristics.Count == 0)
        {
            Debug.LogError("No more names or characteristics available.");
            return null;
        }

        int nameIndex = Random.Range(0, names.Count);
        int characteristicIndex = Random.Range(0, characteristics.Count);

        string selectedName = names[nameIndex];
        string selectedCharacteristic = characteristics[characteristicIndex];
        RestaurantRole role = (RestaurantRole)Random.Range(1, System.Enum.GetValues(typeof(RestaurantRole)).Length);


        names.RemoveAt(nameIndex);
        // characteristics.RemoveAt(characteristicIndex);

        return new NPCInfoSimple(selectedName, selectedCharacteristic, role);
    }
    // Start is called before the first frame update
    void Start()
    {
        // EventHandler.BeforeEnterNextLevelEvent += ResetAgents;
        // CreateAgents();
    }

    void OnEnable()
    {
        // EventHandler.BeforeEnterNextLevelEvent += ResetAgents;
    }
    void DisEnable()
    {
        // EventHandler.BeforeEnterNextLevelEvent -= ResetAgents;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DefaultTwoAgents()
    {
        Vector3 cookPosition = GridMapManager.Instance.grid.CellToWorld(new Vector3Int(0, -9));
        GameObject cookcInstance = Instantiate(cookPrefab, new Vector3(cookPosition.x, cookPosition.y, 0), Quaternion.identity);
        cookcInstance.transform.SetParent(GameObject.Find("NPCs").transform);

        Vector3 waiterPosition = GridMapManager.Instance.grid.CellToWorld(new Vector3Int(-6, 1));
        GameObject waiterInstance = Instantiate(waiterPrefab, new Vector3(waiterPosition.x, waiterPosition.y, 0), Quaternion.identity);
        waiterInstance.transform.SetParent(GameObject.Find("NPCs").transform);
    }

    void DefaultThreeAgents()
    {
        DefaultTwoAgents();
        Vector3 cookPosition1 = GridMapManager.Instance.grid.CellToWorld(new Vector3Int(2, -10));
        GameObject cookcInstance1 = Instantiate(cookPrefab, new Vector3(cookPosition1.x, cookPosition1.y, 0), Quaternion.identity);
        cookcInstance1.GetComponent<NPCInnerVoice>().npcInfo.npcName = "Sam";
        cookcInstance1.transform.SetParent(GameObject.Find("NPCs").transform);
    }

    void DefaultFiveAgents()
    {
        DefaultThreeAgents();
        Vector3 waiterPosition1 = GridMapManager.Instance.grid.CellToWorld(new Vector3Int(-5, 2));
        GameObject waiterInstance1 = Instantiate(waiterPrefab, new Vector3(waiterPosition1.x, waiterPosition1.y, 0), Quaternion.identity);
        waiterInstance1.transform.SetParent(GameObject.Find("NPCs").transform);
        waiterInstance1.GetComponent<NPCInnerVoice>().npcInfo.npcName = "David";
        Vector3 cookPosition2 = GridMapManager.Instance.grid.CellToWorld(new Vector3Int(3, -8));
        GameObject cookcInstance2 = Instantiate(cookPrefab, new Vector3(cookPosition2.x, cookPosition2.y, 0), Quaternion.identity);
        cookcInstance2.GetComponent<NPCInnerVoice>().npcInfo.npcName = "Alice";
        cookcInstance2.transform.SetParent(GameObject.Find("NPCs").transform);
    }

    void DefaultSevenAgents()
    {
        DefaultFiveAgents();
        Vector3 waiterPosition2 = GridMapManager.Instance.grid.CellToWorld(new Vector3Int(-4, 3));
        GameObject waiterInstance2 = Instantiate(waiterPrefab, new Vector3(waiterPosition2.x, waiterPosition2.y, 0), Quaternion.identity);
        waiterInstance2.transform.SetParent(GameObject.Find("NPCs").transform);
        waiterInstance2.GetComponent<NPCInnerVoice>().npcInfo.npcName = "Tom";

        Vector3 cookPosition3 = GridMapManager.Instance.grid.CellToWorld(new Vector3Int(3, -10));
        GameObject cookcInstance3 = Instantiate(cookPrefab, new Vector3(cookPosition3.x, cookPosition3.y, 0), Quaternion.identity);
        cookcInstance3.transform.SetParent(GameObject.Find("NPCs").transform);
        cookcInstance3.GetComponent<NPCInnerVoice>().npcInfo.npcName = "Jerry";
    }

    void CreateAgents(string defaultSettings = "default_two_agents")
    {
        // int agentNum = Settings.agentNumPerLevelList[TaskManager.Instance.taskLevel - 1];
        if (defaultSettings == "default_two_agents") {
            DefaultTwoAgents();
        }
        if (defaultSettings == "default_three_agents") {
            DefaultThreeAgents();
        }
        if (defaultSettings == "default_five_agents") {
            DefaultFiveAgents();
        }
        if (defaultSettings == "default_seven_agents") {
            DefaultSevenAgents();
        }

        if (defaultSettings == "default_single_agent") {
            Vector3 cookPosition = GridMapManager.Instance.grid.CellToWorld(new Vector3Int(0, -9));
            GameObject cookcInstance = Instantiate(commonAgentPrefab, new Vector3(cookPosition.x, cookPosition.y, 0), Quaternion.identity);
            cookcInstance.transform.SetParent(GameObject.Find("NPCs").transform);
        }
    }

    public void ResetAgents(AgentInfo agentInfo)
    {
        GameObject npcParent = GameObject.Find("NPCs");
        foreach (Transform npc in npcParent.transform) {
            Destroy(npc.gameObject);
        }
        CreateAgents(agentInfo.defaultSettings);
    }

    public void ChallengeModeResetAgents()
    {
        GameObject npcParent = GameObject.Find("NPCs");
        foreach (Transform npc in npcParent.transform) {
            Destroy(npc.gameObject);
        }
        CreateDefaultTwoAgents();
    }

    public void CreateDefaultTwoAgents()
    {
        Vector3 cookPosition = GridMapManager.Instance.grid.CellToWorld(new Vector3Int(0, -9));
        GameObject cookcInstance = Instantiate(cookPrefab, new Vector3(cookPosition.x, cookPosition.y, 0), Quaternion.identity);
        cookcInstance.transform.SetParent(GameObject.Find("NPCs").transform);        

        Vector3 waiterPosition = GridMapManager.Instance.grid.CellToWorld(new Vector3Int(-6, 1));
        GameObject waiterInstance = Instantiate(waiterPrefab, new Vector3(waiterPosition.x, waiterPosition.y, 0), Quaternion.identity);
        waiterInstance.transform.SetParent(GameObject.Find("NPCs").transform);
    }

    public void AddAgent(NPCInfoSimple npcInfoSimple)
    {
        TimeManager.Instance.PauseGame();
        if (npcInfoSimple.Role == RestaurantRole.Waiter) {
            Vector3 waiterPosition = GridMapManager.Instance.grid.CellToWorld(new Vector3Int(-6, 1));
            GameObject waiterInstance = Instantiate(waiterPrefab, new Vector3(waiterPosition.x, waiterPosition.y, 0), Quaternion.identity);
            waiterInstance.transform.SetParent(GameObject.Find("NPCs").transform);
            waiterInstance.GetComponent<NPCInnerVoice>().npcInfo.npcName = npcInfoSimple.Name;
            waiterInstance.GetComponent<NPCInnerVoice>().npcInfo.npcCharacteristic = npcInfoSimple.Characteristic;   
        }
        else {
            Vector3 cookPosition = GridMapManager.Instance.grid.CellToWorld(new Vector3Int(0, -9));
            GameObject cookcInstance = Instantiate(cookPrefab, new Vector3(cookPosition.x, cookPosition.y, 0), Quaternion.identity);
            cookcInstance.transform.SetParent(GameObject.Find("NPCs").transform);    
            cookcInstance.GetComponent<NPCInnerVoice>().npcInfo.npcName = npcInfoSimple.Name;
            cookcInstance.GetComponent<NPCInnerVoice>().npcInfo.npcCharacteristic = npcInfoSimple.Characteristic;   
        }

        CoinManager.Instance.CoinNum -= 150;
        TimeManager.Instance.ResumeGame();
        
    }
}
