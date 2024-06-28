using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pan : Item
{
    public List<FoodProperty> foodList = new();
    private Animator animator;
    public Oven oven;
    
    // TODO: delete this parameter later.
    public GameObject dishPrefab;

    public bool isCooking;
    public bool isReadyToServe;
    public int leftCookingTime;
    private float tikTime;
    public Recipe currentRecipe;
    GameObject progressBar;

    protected override void OnEnable()
    {
        base.OnEnable();
        foodList = new();
        isReadyToServe = false;
        if (progressBar == null) StartCoroutine(CreateProgressBar());
    }

    void OnDisable() {
        StopAllCoroutines();    
    }

    void Awake()
    {
        
        // progressBar.SetActive(false);
        animator = GetComponent<Animator>();
        currentRecipe = null;
        visibleRole = RestaurantRole.Cook;
        // leftCookingTime = 3;
    }

    IEnumerator CreateProgressBar()
    {
        if (progressBar == null) {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 0.65f, 0));
            while (UIManager.Instance == null) yield return null;
            progressBar = UIManager.Instance.CreateProgressBar(screenPos);
        }
    }

    void Update()
    {
        animator.SetBool("isEmpty", foodList.Count == 0);
        if (foodList.Count > 0 && oven.isOpen && currentRecipe != null) {
            animator.SetBool("isCooking", true);
            isCooking = true;
        }
        else {
            animator.SetBool("isCooking", false);
            isCooking = false;
        }

        if (isCooking && leftCookingTime > 0 && currentRecipe != null) {
            if (!TimeManager.Instance.IsPaused()) {
                tikTime += Time.deltaTime;
            }
            if (tikTime >= Settings.secondThreshold) {
                tikTime -= Settings.secondThreshold;
                leftCookingTime--;
                if (leftCookingTime == 0) {
                    isReadyToServe = true;
                }
            }
        }
        if (progressBar != null) {
            if (isCooking && currentRecipe != null) {
                try {
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 0.65f, 0));
                    progressBar.transform.position = screenPos;
                    progressBar.SetActive(true);
                    ProgressBar m_progressBar = progressBar.GetComponent<ProgressBar>();
                    m_progressBar.SetFillAmount(1.0f - leftCookingTime * 1.0f / currentRecipe.cookingTime);
                }
                catch (Exception ex) {
                    Debug.LogWarning("error occurred while showing the progress bar: " + ex.Message);
                }
            }
            else {
                progressBar.SetActive(false);
            }
        }


    }

    public override void PutInto(GameObject item, HistoryInstruction instruction = null)
    {
        Food food = item?.GetComponentInChildren<Food>();
        if (food == null) {
            food = item?.GetComponentInChildren<Plate>()?.food;
            if (food == null && instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "your plate is empty!";
                return;
            }
        }
        PutFoodIntoPan(food, instruction);
    }

    public override void TakeOut(GameObject item=null, GameObject npc=null, string insideItemName=null, HistoryInstruction instruction=null)
    {
        if (item == null || item?.GetComponent<Plate>() == null) {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "you need an empty plate to take food out of pan";
            }
            return;
        }
        Plate plate = item?.GetComponent<Plate>();
        ServeFood(plate, instruction);
    }

    public void PutFoodIntoPan(Food food, HistoryInstruction instruction = null) 
    {   
        if (food == null) {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "can only put the food item into the pan";
            }
            return;
        }
        FoodProperty foodProperty = food.foodProperty;

        if (foodProperty.foodName.EndsWith("_cooked")) {
            foodProperty.foodName = foodProperty.foodName.Replace("_cooked", "");
            foodProperty.isCooked = false;
        }

        if (foodProperty.isCooked) {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "can not put cooked food into the pan";
            }
            return;
        }

        foodList.Add(foodProperty);
        Destroy(food.gameObject);
        Recipe recipe = DishManager.Instance.MatchRecipe(foodList);

        // FIX: maybe exist bugs
        if (recipe != null) {
            leftCookingTime = recipe.cookingTime + Settings.cookingTimeIncrease - Settings.cookingTimeDecreaseToday;
            currentRecipe = recipe;
        }
    }

    public void ServeFood(Plate plate, HistoryInstruction instruction=null)
    {
        if (plate == null || plate.food || !isReadyToServe) {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                if (plate.food) {
                    instruction.message = "the plate is not empty";                   
                }
                else if (!isReadyToServe) {
                    instruction.message = "unable to take food out of pan, no food or food is not cooked done";
                    if (oven.isOpen == false) instruction.message += ", " + oven.itemName + " has not been opened";
                }
            }
            return;
        }

        // TODO: check up the recipe
        GameObject dish = Instantiate(currentRecipe.foodPrefab, plate.transform.position, Quaternion.identity); 

        if (currentRecipe != null) {
            dish.GetComponent<Food>().foodProperty = currentRecipe.result;
            if (DishManager.Instance.AreIngredientsPerfect(foodList, currentRecipe.ingredients)) {
                dish.GetComponent<Food>().isPerfect = true;
            }
            GameObject itemsParent = GameObject.Find("Items");
            if (itemsParent != null)
            {
                dish.transform.SetParent(itemsParent.transform);
            }
        }

        plate.PutFoodIntoPlate(dish.GetComponent<Food>());

        foodList.Clear();
        isReadyToServe = false;
        currentRecipe = null;
    }

    public override bool IsMeetInteractionCondition(Item item)
    {
        // put food into pan
        if (!isCooking && !isReadyToServe) {
            if (item is Food) {
                return true;
            }
            else if (item is Plate) {
                Plate plate = item as Plate;
                if (plate.food != null) {
                    return true;
                }
            }
        }
        // get food from pan into plate
        else if (isReadyToServe) {
            if (item is Plate) {
                Plate plate = item as Plate;
                if (plate.food == null) {
                    return true;
                }
            }
        }        
        return false;
    }

    public override void Interact(Item item = null)
    {
        if (item == null) return;
        if (!isCooking && !isReadyToServe) {
            if (item is Food) {
                PutFoodIntoPan(item as Food);
            }
            else if (item is Plate) {
                Plate plate = item as Plate;
                if (plate.food != null) {
                    PutFoodIntoPan(plate.food);
                }
            }
        }
        else if (isReadyToServe) {
            if (item is Plate) {
                ServeFood(item as Plate);
            }
        }
    }

    public override void InitName()
    {
        itemName = "pan";
    }

    public override string GetMessage()
    {
        string mes = itemName;
        string extraMes = "";
        if (foodList.Count == 0) {
            extraMes = "empty";
        }
        else {
            string foodNameStr = string.Join(",", foodList.Select(fp => fp.foodName));
            string dishMes = currentRecipe == null ? foodNameStr : currentRecipe.dishName.ToString(); 
            if (isReadyToServe) {
                extraMes += "cooking done, with " + currentRecipe.dishName.ToString();
            }
            else if(isCooking) {
                extraMes += "cooking, will be done in " + leftCookingTime.ToString() + " seconds, with " + dishMes;
            }
            else {
                // List<string> foodNameList = new();
                // string foodNameStr;
                // foreach (FoodProperty foodProperty in foodList) {
                //     foodNameList.Add(foodProperty.foodName);
                // }
                // foodNameStr = String.Join(",", foodNameList);
                extraMes = "with " + foodNameStr;
            }
        }

        extraMes += ", on the " + oven.itemName; 

        return mes + " (" + extraMes + ")";
    }

    private void OnDestroy() {
        Destroy(progressBar);
    }
}
