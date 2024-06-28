using System;
using System.Collections;
using System.Collections.Generic;
using LMagent.Astar;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]


public class Customer : MonoBehaviour
{
    private Vector2Int currentGridPosition;
    private Vector2Int targetGridPosition;
    private Vector2Int nextGridPosition;

    public float speed;
    private Vector2 movDir;
    private bool isMoving;

    public CustomerStatus Status {
        get { return m_Status; }
        set { m_Status = value; }
    }

    public string orderedDish;

    CustomerStatus m_Status = CustomerStatus.Init;

    private Stack<MovementStep> movementSteps = new();

    private Rigidbody2D rb;
    private Animator anim;
    public SpriteRenderer spriteRenderer;

    public Sprite status_notice;
    public Sprite status_like;
    public Sprite status_waiting_beverage;

    private const float waitingForBeverageMaxTime = 60f;
    private const float waitingForDishMaxTime = 120f;
    private const float waitingForRecommendationMaxTime = 60f;
    public float waitingTik = 0f;
    
    public Seat seat;
    private bool isOrderComplete = false;
    private int rewardCoin = 0;

    public GameObject talkingToWaiter;

    public bool activatedByConfig;
    public OrderData orderData;
    public List<string> flavors;
    public string flavorDescription;

    public int customerId;
    public int arriveTime;
    public int leaveTime;

    public bool perfectCompleted;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentGridPosition = (Vector2Int)GridMapManager.Instance.grid.WorldToCell(transform.position);
        spriteRenderer.enabled = false;
        perfectCompleted = true;
    }

    void Start()
    {
        waitingTik = 0;
    }

    void FixedUpdate()
    {

    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    void Update()
    {
        currentGridPosition = (Vector2Int)GridMapManager.Instance.grid.WorldToCell(transform.position);
        SwitchAnimation();

        if (Status == CustomerStatus.WaitingForBeverage) {
            if (!TimeManager.Instance.IsPaused()) {
                waitingTik += Time.deltaTime;
            }
            
            if (waitingTik > (waitingForBeverageMaxTime - Settings.maxWaitingTimeDecreaseToday)) {
                StartCoroutine(TimeOutLeave(seat));
            }
        }

        if (Status == CustomerStatus.WaitingForDish) {
            if (!TimeManager.Instance.IsPaused()) {
                waitingTik += Time.deltaTime;
            }
            
            if (waitingTik > (waitingForDishMaxTime - Settings.maxWaitingTimeDecreaseToday)) {
                StartCoroutine(TimeOutLeave(seat));
            }
        }

        if (Status == CustomerStatus.WaitingForRecommendation) {
            if (!TimeManager.Instance.IsPaused()) {
                waitingTik += Time.deltaTime;
            }
            
            if (waitingTik > waitingForRecommendationMaxTime - Settings.maxWaitingTimeDecreaseToday) {
                StartCoroutine(TimeOutLeave(seat));
            }
        }
    }

    public IEnumerator AddImmediateMovementRoutine(Vector2Int targetGridPosition)
    {
        movementSteps.Clear();
        this.targetGridPosition = targetGridPosition;
        Astar.Instance.BuildPath(currentGridPosition, targetGridPosition, movementSteps);
        StartCoroutine(Movement());
        yield return null;
    }

    public void FindPath(Vector2Int targetGridPosition)
    {
        movementSteps.Clear();
        this.targetGridPosition = targetGridPosition;
        Astar.Instance.BuildPath(currentGridPosition, targetGridPosition, movementSteps);
    }

    public void SitDown()
    {
        arriveTime = TimeManager.Instance.secondTimer;
        NPCCreator.Instance.customerRoutineList[customerId].arriveTime = arriveTime;
        if (activatedByConfig) {
            if (orderData.foodType != "none") {
                orderedDish = orderData.foodType;
            }
            else {
                orderedDish = "WAITING_FOR_RECOMMENDATION";
            }

            if (orderData.wantsBeverage) {
                Status = CustomerStatus.WaitingForBeverage;
            }
            else {
                if (orderData.foodType == "none") {
                    Status = CustomerStatus.WaitingForRecommendation;
                }
                else {
                    Status = CustomerStatus.WaitingForDish;
                }
            }
        }
        else {
            float chance = UnityEngine.Random.Range(0f, 1f);
            orderedDish = DishManager.Instance.GetLevelRandomDishName();
            if (chance < ChallengeLevelManager.Instance.GetLevelOrderDrinkProbability()) {
                Status = CustomerStatus.WaitingForBeverage;
            }
            else {
                chance = UnityEngine.Random.Range(0f, 1f);
                if (chance < ChallengeLevelManager.Instance.GetLevelRecommendationProbability()) {
                    Status = CustomerStatus.WaitingForRecommendation;
                }
                else {
                    Status = CustomerStatus.WaitingForDish;
                }
            }
        }

    }

    private Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        Vector3 worldPos = GridMapManager.Instance.grid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2f);
    }

    public IEnumerator Movement(Action callback = null)
    {
        while (TimeManager.Instance.IsPaused()) {
            yield return new WaitForFixedUpdate();
        }

        if (movementSteps.Count > 1) {
            nextGridPosition = movementSteps.Pop().gridCoordinate;
            isMoving = true;
        }

        while (movementSteps.Count > 0 || Vector2.Distance(rb.position, (Vector2)GetWorldPosition((Vector3Int)nextGridPosition)) > 0.05f) {
            if (rb == null) yield break;
            movDir =  ((Vector2)GetWorldPosition((Vector3Int)nextGridPosition) - (Vector2)transform.position).normalized;
            Vector2 newPosition = rb.position + movDir * speed * Time.fixedDeltaTime;

            while (TimeManager.Instance.IsPaused()) {
                yield return new WaitForFixedUpdate();
            }
            if (rb != null) rb.MovePosition(newPosition);
            else yield break;
            if (Vector2.Distance(rb.position, (Vector2)GetWorldPosition((Vector3Int)nextGridPosition)) <= 0.05f) {
                if (movementSteps.Count > 0) {
                    nextGridPosition = movementSteps.Pop().gridCoordinate;
                }
            }
            yield return new WaitForFixedUpdate();
        }
        isMoving = false;
        StartCoroutine(SetStopAnimation());

        if (callback != null) {
            callback.Invoke();
        }
    }
    private void SwitchAnimation()
    {   
        // isMoving = transform.position != GetWorldPosition((Vector3Int)targetGridPosition);
        // if (anim.GetComponent<RuntimeAnimatorController>() == null) return;
        anim.SetBool("IsMoving", isMoving);
        if (isMoving) {
            anim.SetBool("Exit", true);
            anim.SetFloat("DirX", movDir.x);
            anim.SetFloat("DirY", movDir.y);
        }
        else {
            anim.SetBool("Exit", false);
        }

        // if (isSitDown == true) {
        //     if (isWaitingForServe) {
        //         spriteRenderer.enabled = true;
        //         spriteRenderer.sprite = status_notice;
        //     }
        // }
        // else {
        //     spriteRenderer.enabled = false;
        // }
        spriteRenderer.enabled = true;
        if (Status == CustomerStatus.WaitingForBeverage) {
            spriteRenderer.sprite = status_waiting_beverage;
        }
        else if (Status == CustomerStatus.WaitingForDish) {
            spriteRenderer.sprite = status_notice;
        }
        else if (isOrderComplete != true) {
            spriteRenderer.enabled = false;
        }
    }

    private IEnumerator SetStopAnimation()
    {
        movDir.x = 0;
        movDir.y = -1;
        // if (anim.GetComponent<RuntimeAnimatorController>() == null) yield break; 
        anim.SetFloat("DirX", movDir.x);
        anim.SetFloat("DirY", movDir.y);

        // TODO: play stop animation
        yield return null;
    }
    public bool CheckFlavor(Food food)
    {
        FoodProperty foodProperty = food.foodProperty;
        if (activatedByConfig) {
            flavors = orderData.flavors;
        }
        // if (flavors == null || flavors.Count == 0) return false;
        if (flavors.Contains("salty") && !foodProperty.salty) {
            return false;
        }
        if (flavors.Contains("sweet") && !foodProperty.sweet) {
            return false;
        }
        if (flavors.Contains("spicy") && !foodProperty.spicy) {
            return false;
        }
        if (flavors.Contains("sour") && !foodProperty.sour) {
            return false;
        }
        return true;

    }

public IEnumerator EatingFood(Plate plate, Food food, Seat seat)
{
    Status = CustomerStatus.Eating;
    float dishValue = DishManager.Instance.GetRecipe(orderedDish).result.coinValue;
    int bonus = 0;
    bool haveFlavorDemand = false;
    bool perfectFlavor = false;

    if (activatedByConfig) {
        flavors = orderData.flavors;
    }
    if (flavors == null || flavors.Count == 0) {
        haveFlavorDemand = false;
    }
    else {
        haveFlavorDemand = true;
        perfectFlavor = CheckFlavor(food);
    }
    
    if (waitingTik <= (waitingForDishMaxTime - Settings.maxWaitingTimeDecreaseToday) / 2f && food.isPerfect && ((!haveFlavorDemand) || (haveFlavorDemand && perfectFlavor))) {
        bonus += (int)(dishValue * 0.2f);
        dishValue = dishValue * 1.2f;
    }
    else {
        perfectCompleted = false;
    }

    if (waitingTik > (waitingForDishMaxTime - Settings.maxWaitingTimeDecreaseToday) / 2f) {
        dishValue -= Settings.penaltyForLateDishes;
    }
    
    rewardCoin += (int)dishValue;
    if (!food.isPerfect || (haveFlavorDemand && !perfectFlavor)) {
        rewardCoin -= 5;
        perfectCompleted = false;
     }
    if (haveFlavorDemand && perfectFlavor) rewardCoin += 5;

    spriteRenderer.enabled = false;

    float leftTime = 5f;

    while (leftTime > 0) {
        if (this == null || gameObject == null || !gameObject.activeInHierarchy) yield break;
        if (!TimeManager.Instance.IsPaused()) {
            leftTime -= Time.deltaTime / Settings.secondThreshold;
        }

        yield return null;
    }

    if (this == null || gameObject == null || !gameObject.activeInHierarchy) yield break;
    spriteRenderer.enabled = true;
    spriteRenderer.sprite = status_like;

    plate.pickable = true;
    if (food.gameObject != null) {
        Destroy(food.gameObject);
    }

    // add coin
    CoinManager.Instance.GetCoins(GetTotalCoin());
    CoinManager.Instance.bonus += bonus;
    CoinManager.Instance.successOrder += 1;
    TaskManager.Instance.currentOrderFinishNum += 1;
    LeaveAndFindPath(seat);
    
    if (this == null || gameObject == null || !gameObject.activeInHierarchy) yield break;
    
    StartCoroutine(Movement(()=>EventOnDestroy()));

    yield return null;
}


    public IEnumerator Drinking(Cup cup) 
    {
        Status = CustomerStatus.Drinking;
        int bonus = 0;

        // TODO: change reward calculation
        if (waitingTik <= (waitingForBeverageMaxTime - Settings.maxWaitingTimeDecreaseToday) / 2f) {
            rewardCoin += 1;
            bonus += 1;
        }
        else {
            perfectCompleted = false;
        }
        rewardCoin += 5 + Settings.extraBeveragerevenue;


        waitingTik = 0;
        
        if (this == null || gameObject == null || !gameObject.activeInHierarchy) yield break;
        spriteRenderer.enabled = false;

        float leftTime = 3f;

        while (leftTime > 0) {
            if (this == null || gameObject == null || !gameObject.activeInHierarchy) yield break;
            if (!TimeManager.Instance.IsPaused()) {
                leftTime -= Time.deltaTime / Settings.secondThreshold;
            }
            yield return null;
        }
        // add coin
        CoinManager.Instance.GetCoins(GetTotalCoin());
        CoinManager.Instance.bonus += bonus;
        rewardCoin = 0;

        if (this == null || gameObject == null || !gameObject.activeInHierarchy) yield break;
        spriteRenderer.enabled = true;

        // cup.pickable = true;
        // cup.isEmpty = true;
        if (cup != null) {
            Destroy(cup.gameObject);
        }
        if (activatedByConfig) {
            if (orderData.foodType == "none") {
                Status = CustomerStatus.WaitingForRecommendation;
            }
            else {
                Status = CustomerStatus.WaitingForDish;
            }
        }
        else {
            float chance = UnityEngine.Random.Range(0f, 1f);
            if (chance < ChallengeLevelManager.Instance.GetLevelRecommendationProbability()) {
                Status = CustomerStatus.WaitingForRecommendation;
            }
            else {
                Status = CustomerStatus.WaitingForDish;
            }
        }
        yield return null;
    } 

    public IEnumerator Talking(GameObject npc, HistoryInstruction historyInstruction)
    {
        Status = CustomerStatus.Talking;

        // if (waitingTik <= waitingForRecommendationMaxTime / 2f) {
        //     rewardCoin += 3;
        // }
        string prompt = Settings.customerWaiterTalkStringBegin;
        NPCInnerVoice npcInnerVoice = npc.GetComponent<NPCInnerVoice>();
        prompt += "Here is the basic information of waiter:\n{\n" + npcInnerVoice.GetBasicAndObservationPrompt(true, false) + "\n}\n";
        prompt += "Here is the item information in the scene:\n{\n" + ItemManager.Instance.GetAllItemMessage(RestaurantRole.Common) + "\n}\n";

        prompt += Settings.customerWaiterTalkStringEnd;
    
        StartCoroutine(LMManager.Instance.CreateAPICall(prompt, gameObject, "talk", historyInstruction));
        yield return null;
    }

    private void EventOnDestroy()
    {
        
        Destroy(gameObject);
        NPCCreator.Instance.npcNum--;
    }

    public int GetTotalCoin()
    {
        return rewardCoin;
    }

    public bool IsSitDown()
    {
        return Status != CustomerStatus.Init;
    }


    public IEnumerator TimeOutLeave(Seat seat)
    {
        spriteRenderer.enabled = false;
        perfectCompleted = false;
        if (TaskManager.Instance != null && TaskManager.Instance.gameMode == GameMode.Challenge) {
            // TODO: change the coin punish later
            CoinManager.Instance.CoinNum -= 10;
        }

        LeaveAndFindPath(seat);
        StartCoroutine(Movement(()=>EventOnDestroy()));

        yield return null;
    }

    public void LeaveAndFindPath(Seat seat)
    {
        seat.npc = null;
        seat.isOccupied = false;

        leaveTime = TimeManager.Instance.secondTimer;
        NPCCreator.Instance.customerRoutineList[customerId].leaveTime = leaveTime;
        NPCCreator.Instance.CustomerFinishOrder(orderData, perfectCompleted);
        // CoinManager.Instance.CoinNum += GetTotalCoin() / 2;

        Status = CustomerStatus.Init;
        FindPath(new(32, 15));
    }
}
