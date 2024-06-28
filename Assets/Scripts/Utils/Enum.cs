using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    Task, Challenge
}

public enum GridType
{
    Obstacle, CanDropItem, NotDefined
}

public enum Role
{
    System, NPC
}

public enum RestaurantRole
{
    Common, Waiter, Cook
}

public enum InstructionStatus
{
    Success, Failed, Running
}

public enum CustomerStatus
{
    Init,     
    WaitingForBeverage,
    WaitingForDish,
    Drinking,
    Eating,
    WaitingForRecommendation,
    Talking
}

public enum DishType
{
    carrot_cut,
    carrot_cooked,
    eggplant_cooked,
    steak_cooked,
    potato_cooked,
    fried_potato_carrot
}

public enum CooperationStrategy
{
    Decentralized, Centralized, MessagePool, Single
}