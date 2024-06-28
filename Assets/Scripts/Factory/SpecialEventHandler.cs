using System.Collections.Generic;
using System;
using UnityEngine;
using System.Text;

public static class SpecialEventHandler
{
    // public SpecialEvent(string eventName, string description, bool isTemporary = false, bool isNegative = false, Action effect = null, Action onExpire = null)
    // {
    //     this.eventName = eventName;
    //     this.description = description;
    //     this.isTemporary = isTemporary;
    //     this.isNegative = isNegative;
    //     this.effect = effect;
    //     this.onExpire = onExpire;
    // }
    public static readonly SpecialEvent GainCoin50Immediately = new(
        "Gain Coin",
        "Gain 50 coins immediately.",
        true, // isTemporary
        false, // isNegative
        () => CoinManager.Instance.CoinNum += 50
    );

    public static readonly SpecialEvent LoseCoin50Immediately = new(
        "Lose Coin",
        "Lose 50 coins immediately.",
        true, // isTemporary
        true, // isNegative
        () => CoinManager.Instance.CoinNum -= 50
    );

    public static readonly SpecialEvent FoodIngredientPriceIncrease = new (
        "Food Ingredient Price Increase",
        "Today's food ingredient costs increase by 2 coin.",
        true, // isTemporary
        true, // isNegative
        () => Settings.extraCostForIngredients += 2,
        () => Settings.extraCostForIngredients -= 2
    );

public static readonly SpecialEvent PermanentFoodIngredientPriceIncrease = new(
    "Permanent Food Ingredient Price Increase",
    "Food ingredient costs increase by 1 coin permanently.",
    false, // isTemporary
    true, // isNegative
    () => Settings.extraCostForIngredients += 1
);

public static readonly SpecialEvent ApplianceWearOut = new(
    "Appliance Wear Out",
    "Cooking time increases by 2 seconds permanently.",
    false, // isTemporary
    true, // isNegative
    () => Settings.cookingTimeIncrease += 2
);

public static readonly SpecialEvent DailyWageDecrease = new(
    "Daily Wage Decrease",
    "Employee daily wage decreases by 10 coins for today.",
    true, // isTemporary
    false, // isNegative
    () => Settings.dailyWageDecrease += 10,
    () => Settings.dailyWageDecrease -= 10
);

public static readonly SpecialEvent DailyWageIncrease = new(
    "Daily Wage Increase",
    "Employee daily wage increases by 10 coins for today.",
    true, // isTemporary
    true, // isNegative
    () => Settings.dailyWageIncrease += 10,
    () => Settings.dailyWageIncrease -= 10
);

public static readonly SpecialEvent PermanentWageIncrease = new(
    "Permanent Wage Increase",
    "Employee wage increases by 10 coins permanently.",
    false, // isTemporary
    true, // isNegative
    () => Settings.permanentWageIncrease += 10
);

public static readonly SpecialEvent PermanentRentIncrease = new(
    "Permanent Rent Increase",
    "Restaurant rent increases by 20 coins permanently.",
    false, // isTemporary
    true, // isNegative
    () => Settings.permanentRentIncrease += 20
);

public static readonly SpecialEvent NoWageAndRentToday = new(
    "No Wage and Rent Today",
    "No employee wages and restaurant rent for today.",
    true, // isTemporary
    false, // isNegative
    () => { Settings.dailyWage = -99999; Settings.dailyRent = -99999; },
    () => { Settings.dailyWage = Settings.originalDailyWage; Settings.dailyRent = Settings.originalDailyRent; }
);

public static readonly SpecialEvent ReducedCookingTimeToday = new(
    "Reduced Cooking Time Today",
    "All dish cooking times are reduced by 2 seconds for today.",
    true, // isTemporary
    false, // isNegative
    () => Settings.cookingTimeDecreaseToday += 2,
    () => Settings.cookingTimeDecreaseToday -= 2
);

public static readonly SpecialEvent ExtraBonusToday = new(
    "Extra Bonus Today",
    "Earn an extra 2 coins for each bonus today.",
    true, // isTemporary
    false, // isNegative
    () => Settings.extraBonusToday += 2,
    () => Settings.extraBonusToday -= 2
);

public static readonly SpecialEvent ImpatientCustomersToday = new(
    "Impatient Customers Today",
    "Maximum waiting time for customers decreases by 20 seconds for today.",
    true, // isTemporary
    true, // isNegative
    () => Settings.maxWaitingTimeDecreaseToday += 20,
    () => Settings.maxWaitingTimeDecreaseToday -= 20
);

public static readonly SpecialEvent NoBeveragesToday = new(
    "No Beverages Today",
    "Customers do not order beverages today.",
    true, // isTemporary
    false, // isNegative
    () => Settings.noBeveragesToday = true,
    () => Settings.noBeveragesToday = false
);

public static readonly SpecialEvent RecommendDishesToday = new(
    "Recommend Dishes Today",
    "All customers need dish recommendations today.",
    true, // isTemporary
    true, // isNegative
    () => Settings.recommendDishesToday = true,
    () => Settings.recommendDishesToday = false
);


public static readonly SpecialEvent LazyEmployeesForADay = new(
    "Lazy Employees",
    "Employees become lazy today. Action intervals increase by 2 seconds.",
    true, // isTemporary
    true, // isNegative
    () => { Settings.extraActionTik += 2; },
    () => { Settings.extraActionTik -= 2; }
);

public static readonly SpecialEvent PermanentIncreasedBeverageCost = new(
    "Increased Beverage Cost",
    "Beverage costs increase. Each drink costs an additional 2 coins permanently.",
    false, // isTemporary
    true, // isNegative
    () => { Settings.extraBeverageCost += 2; }
);

public static readonly SpecialEvent DecreasedCustomerWaitTime = new(
    "Decreased Customer Wait Time",
    "Maximum wait time for customers permanently decreases by 10 seconds.",
    false, // isTemporary
    true, // isNegative
    () => { Settings.maxWaitingTimeDecreaseToday += 10; }
);

public static readonly SpecialEvent ReducedCoinsForLateDishes = new(
    "Reduced Coins for Late Dishes",
    "If dishes are not served quickly today, received coins decrease by 5.",
    true, // isTemporary
    true, // isNegative
    () => { Settings.penaltyForLateDishes += 5; },
    () => { Settings.penaltyForLateDishes -= 5; }
);
    

public static readonly SpecialEvent CustomerPatienceIncreaseToday = new (
    "Customer Patience Increase",
    "Today customers are more patient, waiting time increased by 20s.",
    true, // isTemporary
    false, // isNegative
    () => Settings.maxWaitingTimeDecreaseToday -= 20,
    () => Settings.maxWaitingTimeDecreaseToday += 20
);

public static readonly SpecialEvent BeveragePriceIncreasePermanent = new (
    "Beverage Price Increase",
    "Beverage prices permanently increase by 1 coin.",
    false, // isTemporary
    false, // isNegative
    () => {Settings.extraBeveragerevenue += 1;}
);

public static readonly SpecialEvent RentDecreasePermanent = new (
    "Rent Decrease",
    "Rent permanently decreases by 5 coins per day.",
    false, // isTemporary
    false, // isNegative
    () => {Settings.originalDailyRent -= 5; Settings.dailyRent -= 5;}
);

public static readonly SpecialEvent EmployeeWageDecreasePermanent = new (
    "Employee Wage Decrease",
    "Employee wages permanently decrease by 5 coins per day.",
    false, // isTemporary
    false, // isNegative
    () => {Settings.originalDailyWage -= 5; Settings.dailyWage -= 5;}
);

    public static List<SpecialEvent> GetAllEvents()
    {
        return new List<SpecialEvent> {
        GainCoin50Immediately, LoseCoin50Immediately, FoodIngredientPriceIncrease,
        PermanentFoodIngredientPriceIncrease, ApplianceWearOut, DailyWageDecrease, 
        PermanentWageIncrease, NoWageAndRentToday, ReducedCookingTimeToday, 
        ExtraBonusToday, ImpatientCustomersToday, NoBeveragesToday, 
        RecommendDishesToday, LazyEmployeesForADay, PermanentIncreasedBeverageCost,
        DecreasedCustomerWaitTime, ReducedCoinsForLateDishes, CustomerPatienceIncreaseToday,
        BeveragePriceIncreasePermanent, RentDecreasePermanent, EmployeeWageDecreasePermanent
         /*, other events */ };
    }
}

public class SpecialEventPool
{
    private List<SpecialEvent> eventPool = new List<SpecialEvent>();

    public void AddEvent(SpecialEvent specialEvent)
    {
        eventPool.Add(specialEvent);
        specialEvent.TriggerEvent();
    }

    public void EndOfDay()
    {
        List<SpecialEvent> expiredEvents = new List<SpecialEvent>();

        foreach (var specialEvent in eventPool)
        {
            if (specialEvent.isTemporary)
            {
                specialEvent.ExpireEvent();
                expiredEvents.Add(specialEvent);
            }
        }

        // Remove expired events from the pool
        foreach (var expiredEvent in expiredEvents)
        {
            eventPool.Remove(expiredEvent);
        }
    }

   public List<SpecialEvent> GetEventPool()
    {
        return eventPool;
    }

    public List<SpecialEvent> GetAvailableNonNegativeEvents()
    {
        List<SpecialEvent> availableEvents = new List<SpecialEvent>();
        List<SpecialEvent> allEvents = SpecialEventHandler.GetAllEvents();

        foreach (var specialEvent in allEvents)
        {
            if (!specialEvent.isNegative && !eventPool.Contains(specialEvent))
            {
                availableEvents.Add(specialEvent);
            }
        }

        return availableEvents;
    }

    public List<SpecialEvent> GetAvailableNegativeEvents()
    {
        List<SpecialEvent> availableEvents = new List<SpecialEvent>();
        List<SpecialEvent> allEvents = SpecialEventHandler.GetAllEvents();

        foreach (var specialEvent in allEvents)
        {
            if (specialEvent.isNegative && !eventPool.Contains(specialEvent))
            {
                availableEvents.Add(specialEvent);
            }
        }

        return availableEvents;
    }

        public List<SpecialEvent> GetRandomNonNegativeEvents(int count)
        {
            List<SpecialEvent> availableEvents = GetAvailableNonNegativeEvents();
            List<SpecialEvent> selectedEvents = new List<SpecialEvent>();

            System.Random rand = new System.Random();
            while (selectedEvents.Count < count && availableEvents.Count > 0)
            {
                int index = rand.Next(availableEvents.Count);
                selectedEvents.Add(availableEvents[index]);
                availableEvents.RemoveAt(index);
            }

            return selectedEvents;
        }

        public SpecialEvent GetRandomNegativeEvent()
        {
            List<SpecialEvent> availableEvents = GetAvailableNegativeEvents();
            if (availableEvents.Count == 0) return null;

            System.Random rand = new System.Random();
            int index = rand.Next(availableEvents.Count);
            Debug.Log(availableEvents[index].description);
            return availableEvents[index];
        }

        public string GetCurrentEventPoolString()
        {
            StringBuilder eventPoolDescription = new StringBuilder();
            foreach (var specialEvent in eventPool)
            {
                eventPoolDescription.AppendLine($"{specialEvent.eventName}: {specialEvent.description}");
            }
            return eventPoolDescription.ToString();
        }
}