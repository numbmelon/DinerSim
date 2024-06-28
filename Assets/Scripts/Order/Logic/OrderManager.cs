using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OrderManager : Singleton<OrderManager>
{
    public List<Seat> seatList;
    private System.Random random = new System.Random();
    public List<string> customerComplaints = new();

    protected override void Awake()
    {
        base.Awake();

    }
    
    void Start()
    {
        EventHandler.AfterCompleteCurrentLevelEvent += ClearMessage;
        EventHandler.BeginChallengeEvent += ClearMessage;
        EventHandler.AfterCurrentChallengeDayEvent += ClearMessage;
    }


    void Update()
    {
        foreach (Seat seat in seatList) {
            CheckSeatService(seat);
        }
    }

    public string GetCustomerComplaint()
    {
        string mes = string.Join("\n", customerComplaints);
        if (mes == "") mes = "null";
        return mes;
    }

    void ClearMessage()
    {
        customerComplaints = new();
        foreach (Seat seat in seatList) {
            seat.isOccupied = false;
            seat.npc = null;
            seat.complaint = "";
        }
    }

    public void InitCustomer(GameObject npc, OrderData orderData = null)
    {
        if (orderData != null) {
            npc.GetComponent<Customer>().activatedByConfig = true;
            npc.GetComponent<Customer>().orderData = orderData;
            // Debug.Log(orderData.flavorDescription);
        }
        // random select a empty chair
        List<int> availableSeatIndexList = new List<int>();
        for(int i = 0; i < seatList.Count; i++)
        {
            if (!seatList[i].isOccupied) 
            {
                availableSeatIndexList.Add(i);
            }
        }

        if (availableSeatIndexList.Count > 0)
        {
            int chairIndex = -1;
            if (orderData != null && availableSeatIndexList.Contains(orderData.seatIdx)) {
                chairIndex = orderData.seatIdx;
            }
            else {
                chairIndex = availableSeatIndexList[random.Next(availableSeatIndexList.Count)];
            } 
            seatList[chairIndex].isOccupied = true; 
            seatList[chairIndex].npc = npc;

            npc.GetComponent<Customer>().seat = seatList[chairIndex];

            // npc move to chairs
            npc.GetComponent<Customer>().FindPath(seatList[chairIndex].chairGridPos);
            StartCoroutine(npc.GetComponent<Customer>().Movement(()=>npc.GetComponent<Customer>().SitDown()));
        }
    }


    public void CheckSeatService(Seat seat)
    {
        if (seat.npc == null) {
            if (seat.desk.isTalking) {
                seat.desk.isTalking = false;
            
                // Customer customer = seat.npc.GetComponent<Customer>();
                // customer.talkingToWaiter = seat.desk.talkingToWaiter;
                // if (customer.talkingToWaiter)
                InstructionExecutor.Instance.CmdComplete(seat.desk.talkingToWaiter);

                if (seat.desk.instruction != null) {
                    seat.desk.instruction.status = InstructionStatus.Failed;
                    seat.desk.instruction.message = "there is no customer to talk!";
                }
            }
            return;
        }
        CheckRecommendationService(seat);
        if (seat.desk.item == null) return;
        CheckBeverageService(seat);
        CheckDishService(seat);
    }

    public void CheckRecommendationService(Seat seat)
    {
        // if isTakling is true, that means here is a singal to begin a dialogue
        if (seat.desk.isTalking == false) return;
        
        seat.desk.isTalking = false;
        Customer customer = seat.npc.GetComponent<Customer>();
        customer.talkingToWaiter = seat.desk.talkingToWaiter;

        if (customer.Status != CustomerStatus.WaitingForRecommendation) {
            InstructionExecutor.Instance.CmdComplete(customer.talkingToWaiter);
            seat.desk.instruction.status = InstructionStatus.Failed;
            seat.desk.instruction.message = "you could only [TalkAt] the customer when he/she need recommendation.";
            return;
        }
        
        StartCoroutine(customer.Talking(customer.talkingToWaiter, seat.desk.instruction));
    }


    public void CheckBeverageService(Seat seat)
    {
        Customer customer = seat.npc.GetComponent<Customer>();
        if (customer.Status != CustomerStatus.WaitingForBeverage) return;

        GameObject item = seat.desk.item;
        Cup cup = item.GetComponent<Cup>();
        if (cup != null) {
            StartCoroutine(customer.Drinking(cup));
        }
    }


    public void CheckDishService(Seat seat)
    {
        Customer customer = seat.npc.GetComponent<Customer>();
        if (customer.Status != CustomerStatus.WaitingForDish) return;

        GameObject item = seat.desk.item;
        Plate plate = item.GetComponent<Plate>();
        FoodProperty orderedDishProperty = DishManager.Instance.GetRecipe(customer.orderedDish).result;
        if (plate != null) {
            Food food = plate.food;
            if (food != null && orderedDishProperty.EqualBaseProperty(food.foodProperty)) {
                ClearSeatComplaint(seat);
                plate.OccupiedByCustomer();
                StartCoroutine(customer.EatingFood(plate, food, seat));
            }
            else {
                if (food == null) {
                AddSeatComplaint(seat, seat.desk.itemName + ": Why did you give me an empty plate?");
                } 
                else if (!orderedDishProperty.EqualBaseProperty(food.foodProperty)) {
                AddSeatComplaint(seat, seat.desk.itemName + ": You're serving me the wrong dish!");
                }
            }
        }
        else {
            Food food = item.GetComponent<Food>();
            if (food != null) {
                AddSeatComplaint(seat, seat.desk.itemName + ": Why don't you put the dishes on a plate?");
            }
            else {
                ClearSeatComplaint(seat);
            }
        }
    }

    private void ClearSeatComplaint(Seat seat)
    {
        if (seat.complaint != "") {
            customerComplaints.Remove(seat.complaint);
            seat.complaint = "";
        }
    }

    private void AddSeatComplaint(Seat seat, string mes)
    {
        ClearSeatComplaint(seat);
        seat.complaint = mes;
        customerComplaints.Add(mes);
    }



    public string GetAllOrderMessage()
    {
        List<string> mesList = new();
        string mes;
        for (int i = 0; i < seatList.Count; i++) {
            if (seatList[i].npc == null) continue;
            Customer customer = seatList[i].npc.GetComponent<Customer>();
            if (customer.Status == CustomerStatus.WaitingForBeverage || customer.Status == CustomerStatus.WaitingForDish || customer.Status == CustomerStatus.WaitingForRecommendation) {
                string orderMes = seatList[i].desk.itemName;
                if (customer.Status == CustomerStatus.WaitingForBeverage) {
                    orderMes += ", order a cup of coffee, already waiting for " + ((int)customer.waitingTik).ToString() + " seconds.";
                }
                if (customer.Status == CustomerStatus.WaitingForDish) {
                    orderMes += ", order " + customer.orderedDish + ", already waiting for " + ((int)customer.waitingTik).ToString() + " seconds.";
                    if ((customer.flavors != null) || (customer.activatedByConfig && customer.orderData?.flavors != null)) {
                        string flavorDescription = (customer.flavorDescription == "" || customer.flavorDescription == null) ? customer.orderData.flavorDescription : customer.flavorDescription;
                        if (flavorDescription != "") orderMes += "flavor description: " + flavorDescription;
                    }
                }
                if (customer.Status == CustomerStatus.WaitingForRecommendation) {
                    orderMes += ", waiting to talk with the waiter to get recommendations for ordering, already waiting for " + ((int)customer.waitingTik).ToString() + " seconds.";
                }
                mesList.Add(orderMes);
            }
        }
        mes = String.Join("\n", mesList.OrderBy(x=>x).ToList());
        if (mes == "") mes = "null";
        return mes;
    }

    public string GetAllOrderedDishNameAndCount()
    {
        // Dictionary<string, int> dishCount = new Dictionary<string, int>();

        // for (int i = 0; i < seatList.Count; i++) {
        //     if (seatList[i].npc == null) continue;
        //     Customer customer = seatList[i].npc.GetComponent<Customer>();
        //     if (customer.Status == CustomerStatus.WaitingForDish) {
        //         if (dishCount.ContainsKey(customer.orderedDish)) {
        //             dishCount[customer.orderedDish]++;
        //         } else {
        //             dishCount.Add(customer.orderedDish, 1);
        //         }
        //     }
        // }

        // List<string> mesList = new List<string>();
        // foreach (var dish in dishCount.OrderBy(x => x.Key)) {
        //     mesList.Add($"{dish.Key}: {dish.Value}");
        // }

        // return String.Join("\n", mesList);
        List<string> mesList = new();
        string mes;
        for (int i = 0; i < seatList.Count; i++) {
            if (seatList[i].npc == null) continue;
            string orderMes = seatList[i].desk.itemName;
            Customer customer = seatList[i].npc.GetComponent<Customer>();
            if (customer.Status == CustomerStatus.WaitingForDish) {
                orderMes += ", order " + customer.orderedDish + ", already waiting for " + ((int)customer.waitingTik).ToString() + " seconds.";
                if ((customer.flavors != null) || (customer.activatedByConfig && customer.orderData?.flavors != null)) {
                    string flavorDescription = (customer.flavorDescription == "" || customer.flavorDescription == null) ? customer.orderData.flavorDescription : customer.flavorDescription;
                    if (flavorDescription != "") orderMes += "flavor description: " + flavorDescription;
                }
                mesList.Add(orderMes);
            }
        }   
        mes = String.Join("\n", mesList.OrderBy(x=>x).ToList());
        if (mes == "") mes = "null";
        return mes;
    }
}
