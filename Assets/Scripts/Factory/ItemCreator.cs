using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Newtonsoft.Json;

public class ItemCreator : Singleton<ItemCreator>
{
    public GameObject PlatePrefab;
    public GameObject CarrotPrefab;
    public GameObject CookedCarrotPrefab;
    public GameObject EggplantPrefab;
    public GameObject CookedEggplantPrefab;
    public GameObject SteakPrefab;
    public GameObject PotatoPrefab;
    public GameObject PanPrefab;
    public TextAsset jsonData;

    
    SceneData sceneData;

    void Start()
    {
        // StartCoroutine(LoadJsonFile());
        // EventHandler.BeforeEnterNextLevelEvent += LoadNextData;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    // void LoadNextData()
    // {
    //     StartCoroutine(LoadJsonFile());
    // }

    public void InitializeScene(SceneData sceneData)
    {
        // Debug.Log("Map Name: " + sceneData.map);
        foreach (ItemData item in sceneData.items.items)
        {
            // if (item.properties != null)
            // {
            //     if (item.properties.foodproperty != null)
            //     {
            //         Debug.Log("Food Name: " + item.properties.foodproperty.foodName);
            //     }
            //     if (item.properties.plate != 0)
            //     {
            //         Debug.Log("Plate: " + item.properties.plate);
            //     }
            // }
            CreateItem(item);
        }
    }

    private void CreateItem(ItemData item)
    {
        if (item.item_type == "food") {
            CreateFood(item);
        }
        else if (item.item_type == "plate") {
            CreatePlate(item);
        }
        else if (item.item_type == "pan") {
            CreatePan(item);
        }
    }

    private void CreateFood(ItemData item)
    {
        GameObject prefab = null;
        switch (item.properties.foodproperty.foodName)
        {
            case "carrot":
                prefab = item.properties.foodproperty.isCooked ? CookedCarrotPrefab : CarrotPrefab;
                break;
            case "eggplant":
                prefab = item.properties.foodproperty.isCooked ? CookedEggplantPrefab : EggplantPrefab;
                break;
            case "steak":
                prefab = SteakPrefab;
                break;
            case "potato":
                prefab = PotatoPrefab;
                break;
        }

        if (prefab != null)
        {
            Vector3 position = GridMapManager.Instance.grid.CellToWorld(item.Position);
            GameObject obj = Instantiate(prefab, new Vector3(position.x + Settings.gridCellSize / 2, position.y + Settings.gridCellSize / 2, 0), Quaternion.identity);
            GameObject itemsParent = GameObject.Find("Items");
            if (itemsParent != null)
            {
                obj.transform.SetParent(itemsParent.transform);
            }
        }
    }

    private void CreatePlate(ItemData item)
    {
        // Debug.Log("plate");
        Vector3 positionGlobal = GridMapManager.Instance.grid.CellToWorld(item.Position);
        positionGlobal = new Vector3(positionGlobal.x + Settings.gridCellSize / 2f, positionGlobal.y + Settings.gridCellSize / 2f, 0);

        GameObject plateInstance = Instantiate(PlatePrefab, positionGlobal, Quaternion.identity);
        GameObject itemsParent = GameObject.Find("Items");
        if (itemsParent != null)
        {
            plateInstance.transform.SetParent(itemsParent.transform);
        }
    }

    private void CreatePan(ItemData item)
    {
        // Debug.Log("plate");
        Vector3 positionGlobal = GridMapManager.Instance.grid.CellToWorld(item.Position);
        positionGlobal = new Vector3(positionGlobal.x + Settings.gridCellSize / 2f, positionGlobal.y + Settings.gridCellSize / 2f, 0);

        GameObject panInstance = Instantiate(PanPrefab, positionGlobal, Quaternion.identity);
        GameObject itemsParent = GameObject.Find("Items");
        if (itemsParent != null)
        {
            panInstance.transform.SetParent(itemsParent.transform);
        }
    }

    // IEnumerator LoadJsonFile()
    // {
    //     TimeManager.Instance.PauseGame();
    //     jsonData = Resources.Load<TextAsset>("scene_info_level1_" + LevelManager.Instance.dataIdx.ToString());
    //     if (jsonData == null) yield return new WaitForFixedUpdate();
    //     if (jsonData != null) {
    //         sceneData = JsonUtility.FromJson<SceneData>(jsonData.text); 
    //         while(sceneData == null) yield return null;
    //         InitializeScene(sceneData);
    //     } 
    //     TimeManager.Instance.ResumeGame();
    // }
}
