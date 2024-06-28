using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fridge : Item
{
    private Animator animator;
    public bool isOpen;
    public GameObject fridgeSpaceUIPos;
    bool isInteracting;
    GameObject targetPlayer;
    public GameObject carrotPrefab;
    public GameObject eggplantPrefab;
    public GameObject steakPrefab;
    public GameObject potatoPrefab;

    void Awake()
    {
        animator = GetComponent<Animator>();
        visibleRole = RestaurantRole.Cook;
    }


    void Update()
    {
        animator.SetBool("isOpen", isOpen);
        if (UIManager.Instance == null) return;
        if (isInteracting) {
            ShowBag();
        }
        else {
            HideBag();
        }
    }


    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.GetComponent<Player>()?.enabled == true && isOpen) {
            isInteracting = true;
            targetPlayer = other.gameObject;
        }
        else {
            isInteracting = false;
        }

        // NPC open it
        if (other.gameObject.GetComponent<NPCCooker>()) {
            isOpen = true;
        }
    }


    public void GiveFood()
    {
        if (isInteracting) {
            if (targetPlayer != null) {
                ItemPickUp itemPickup = targetPlayer.GetComponentInChildren<ItemPickUp>();
                if (itemPickup != null) {
                    if (itemPickup.holdItem == null || (itemPickup.holdItem.GetComponent<Plate>() != null && itemPickup.holdItem.GetComponent<Plate>().food == null)) {
                        GameObject new_food = Instantiate(carrotPrefab, itemPickup.gameObject.transform.position, Quaternion.identity);
                        itemPickup.PickUpTarget(new_food);
                        new_food.transform.SetParent(GameObject.Find("Items").transform);
                    }
                }
            }
        }
    }


    public void GiveCookerFood(GameObject npc, string foodName)
    {
        GameObject foodPrefab = GetPrefabByFoodName(foodName);
        if (npc == null || foodPrefab == null) {
            return;
        }

        ItemPickUp itemPickup = npc.GetComponentInChildren<ItemPickUp>();
        if (itemPickup != null) {
            if (itemPickup.holdItem == null || (itemPickup.holdItem.GetComponent<Plate>() != null && itemPickup.holdItem.GetComponent<Plate>().food == null)) {
                GameObject newFood = Instantiate(foodPrefab, itemPickup.gameObject.transform.position, Quaternion.identity);
                itemPickup.PickUpTarget(newFood);
                newFood.transform.SetParent(GameObject.Find("Items").transform);
                CoinManager.Instance.CoinNum -= foodPrefab.GetComponent<Food>().foodProperty.coinValue + Settings.extraCostForIngredients;
            }
            else if (itemPickup.holdItem != null) {
                ItemDrop itemDrop = npc.GetComponentInChildren<ItemDrop>();
                itemDrop.DropItem();
                GameObject newFood = Instantiate(foodPrefab, itemPickup.gameObject.transform.position, Quaternion.identity);
                itemPickup.PickUpTarget(newFood);
                newFood.transform.SetParent(GameObject.Find("Items").transform);
                CoinManager.Instance.CoinNum -= foodPrefab.GetComponent<Food>().foodProperty.coinValue + Settings.extraCostForIngredients;
            }           
        }

    }


    public GameObject GetPrefabByFoodName(string foodName) 
    {
        foodName = foodName.Trim().ToLower();
        if (foodName == "carrot") {
            return carrotPrefab;
        }    
        else if (foodName == "eggplant") {
            return eggplantPrefab;
        }
        else if (foodName == "steak") {
            return steakPrefab;
        }
        else if (foodName == "potato") {
            return potatoPrefab;
        }
        // TODO: throw error
        return null;
    }


    void ShowBag()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(fridgeSpaceUIPos.transform.position);
        UIManager.Instance.ShowFridgeSpace(screenPos, this.gameObject);
    }


    void HideBag()
    {
        UIManager.Instance.HideFridgeSpace();
    }


    public void OpenFridge()
    {
        isOpen = true;
        animator.SetBool("isOpen", true);
    }


    public void CloseFridge()
    {
        isOpen = false;
        animator.SetBool("isOpen", false);
    }


    public override void Interact(Item item = null)
    {
        isOpen = !isOpen;
    }


    public override void InitName()
    {
        itemName = "fridge";
    }


    public override string GetMessage()
    {
        return base.GetMessage();
    }

    public override void PutInto(GameObject item = null, HistoryInstruction instruction = null)
    {
        if (instruction != null) {
            instruction.status = InstructionStatus.Failed;
            instruction.message = "illegal [PutInto] command for a fridge instace, do you mean '[TakeOut] fridge item'?";
        }
    }

    public override void TakeOut(GameObject item = null, GameObject npc = null, string insideItemName = null, HistoryInstruction instruction = null)
    {
        GiveCookerFood(npc, insideItemName);
    }
}
