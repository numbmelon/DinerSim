using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    // time settings
    public const float secondThreshold = 1f; // the smaller, the faster the game is.
    public const int secondHold = 59;
    public const int singleDaySeconds = 80;

    public const float gridCellSize = 1;

    // prompt settings
    public const string otherAgentStatusSectionTitle = "******[Other Agents' Status]******\n";
    public const string characterSettingTitle = "[Character Setting]";
    public const string characterSettingSectionTitle = "******[Character Setting]******\n";
    public const string characterStatusTitle = "[Character Status]";
    public const string characterStatusSectionTitle = "******[Character Status]******\n";
    public const string characterHistoryTitle = "[Your Previous Actions]";
    public const string itemAtHandTitle = "[Item You Hold]";
    public const string itemAtHandSectionTitle = "******[Item You Hold]******\n";
    public const string characterHistorySectionTitle = "******[Your Previous Actions]******\nThis section lists all the actions performed along with their corresponding execution statuses:\n";
    public const string recipeSectionTitle = "******[Recipe]******\n";
    public const string seatsRequireServiceTitle = "[Seats that require service]";
    public const string seatsRequireServiceSectionTitle = "******[Seats that require service]******\n";
    public const string ordersTitle = "[Current Orders]";
    public const string ordersSectionTitle = "******[Current Orders]******\n";
    public const string customerComplaintSectionTitle = "******[Customer Complaints]******\n";
    public const string managerSuggestionTitle = "[Manager Suggestion]";
    public const string managerSuggestionSectionTitle = "******[Manager Suggestion]******\n";
    public const string actionsAvailableTitle = "[Available Actions]";
    public const string actionsAvailableSectionTitle = "******[Available Actions]******\n";
    public const string messagePoolSectionTitle = "******[Message Pool]******\n";
    public const string peerToPeerMessageSectionTitle = "******[Peer-To-Peer Messages]******\n";
    public const string cookActionsAvailable = @"******[Available Actions]******
[MoveTo] ITEM_NAME // move to the item named ITEM_NAME (Usually not needed, as other instructions include navigation functionality)
[Pickup] ITEM_NAME // pick up a pickable item named ITEM_NAME
[PutInto] ITEM_NAME // put the item you are holding into the item named ITEM_NAME (e.g. pan)
[TakeOut] ITEM_NAME // take something out of item named ITEM_NAME (e.g. pan of cooked carrot, plate_stack)
[TakeOut] ITEM_NAME OBJECT_NAME // take item named OBJECT_NAME out of item named ITEM_NAME (e.g. take carrot out of fridge using ""[TakeOut] fridge carrot"" command)
[Cut] CHOPPING_BOARD_NAME // cut food if there is food on the chopping_board named CHOPPING_BOARD_NAME
[Season] SEASONING_STATION_NAME SPECIFIED_FLAVOR // add the specified flavor (replace 'SPECIFIED_FLAVOR' to the word 'spicy', 'salty', 'sour' or 'sweet') at the seasoning_station named SEASONING_STATION_NAME
[Open] ITEM_NAME // open item named ITEM_NAME (e.g. oven)
[Close] ITEM_NAME // close item named ITEM_NAME (e.g. oven)
[Drop] // drop the item you are holding (if you have it)
[DoNothing] // do nothing, just wait 
";


    public const string singleAgentActionsAvailable = @"******[Available Actions]******
[Pickup] item_name // pick up a pickable item named item_name
[PutInto] item_name // put the item you are holding into the item named item_name (e.g. pan, desk)
[TakeOut] item_name // take something out of item named item_name (e.g. pan of cooked carrot, plate_stack, order_pickup_area)
[TakeOut] item_name object_name // take item named object_name out of item named item_name (e.g. take carrot out of fridge using ""[TakeOut] fridge carrot"" command)
[Cut] chopping_board // cut food in the chopping_board
[Open] item // open item named item (e.g. oven)
[Close] item // close item named item (e.g. oven)
[Drop] // drop the item you are holding (if you have it)
[DoNothing] // do nothing, just wait 
";
    public const string waiterActionsAvailable = @"******[Available Actions]******
[MoveTo] ITEM_NAME // move to the item named ITEM_NAME (Usually not needed, as other instructions include navigation functionality)
[Pickup] ITEM_NAME // pick up a pickable item named ITEM_NAME
[PutInto] ITEM_NAME // put the item you are holding into item named ITEM_NAME (e.g. desk)
[TakeOut] ITEM_NAME // take something out of the item named ITEM_NAME (e.g. order_pickup_area)
[DoNothing] // do nothing, just wait (e.g. if all 'order_pickup_area' is empty)
";
    public const string itemInformationTitle = "[Items in the Scene]";
    public const string itemInformationSectionTitle = "******[Items in the Scene]******\n";

    public const string npcCookTakeActionStringBegin = @"******Task******
Your goal is to maximize the coin income. You can earn different values of coins when you complete dishes and the waiter successfully delivers them to customers. Extra tips are awarded for completing dishes within a certain time frame. However, coins are consumed for picking up raw materials from the fridge.

#### Dish Coin Values and Raw Material Costs:
  - **potato_cooked**: 10 coins | (potato, 3 coins)
  - **carrot_cut**: 10 coins | (carrot, 3 coins)
  - **carrot_cooked**: 15 coins | (carrot, 3 coins)
  - **eggplant_cooked**: 20 coins | (eggplant, 5 coins)
  - **steak_cooked**: 30 coins | (steak, 8 coins)
  - **fried_potato_carrot**: 40 coins | (potato, 3 coins), (carrot, 3 coins)

#### Flavor Requests:
- If a customer requests a specific flavor, serving the correct flavor adds +5 coins.
- Serving an incorrect flavor results in -5 coins.

#### Additional Rules: 
1. It is necessary to have a plate to [TakeOut] cooked food from the pan.
2. Observe the cooking time of the pan in the [Items in the Scene] section, remember to take out your cooked food.
3. Remember to open the oven. 
4. You could get infinite food from fridge using [TakeOut] command. (e.g. [TakeOut] fridge carrot)
5. You could get infinite plate from plate_stack using [TakeOut] command. (e.g. [TakeOut] plate_stack)
6. Observe [Item You Hold] to see if you are holding an item.
7. View [Your Previous Actions] section to prevent duplicate actions.
8. Seasoning the dish if the order is accompanied by a 'flavour description' message , and determine which flavour(s) is/are to be blended.
9. Dishes should be served on a plate before they are served to the customer.
10. A pan with carrot_cooked (potato_cooked) in it can still add potato (carrot) to cook fried_potato_carrot.
11. A pan can only cook one dish at a time. Remember to communicate effectively and coordinate with your partner to allocate kitchen tools properly and avoid conflicts

#### Instructions:
Based on all the information above, generate specific instructions that satisfy:
1. At least 1 piece of instruction formatted as "" <action> YOUR_ACTION_HERE </action> "" to take action. See available actions in [Available Actions] section.
2. (Optional) 1 piece of instruction formatted as "" <changestatus> YOUR_STATUS_HERE </changestatus> "" to change your status in [Character Status] section.
";
public const string npcCookTakeActionStringExamples = @"#### Examples: 
Here are 7 examples:
\\\
Example 1: 
<action>[Pickup] carrot</action>
<changestatus>ready to put the carrot in the pan</changestatus> 

Explanation:
this example uses the [Pickup] command to pick up the item named carrot, and change its status to the future plan: put carrot into pan.
\\\

\\\ 
Example 2:
<action>[PutInto] pan</action>
<action>[Open] oven</action>
<changestatus>waiting for cooking to complete</changestatus>

Explanation:
in this example, the [PutInto] command is used to put the item at hand into the pan, and the [Open] command is used to open the corresponding oven to start cooking. Then it changes status to waiting for cooking to complete.
\\\

\\\
Example 3:
<action>[TakeOut] fridge carrot</action>
<changestatus>trying to get a carrot from fridge</changestatus>

Explanation:
this example uses the [TakeOut] command to get a carrot from fridge.
\\\

\\\
Example 4:

turn 1:
<action>[Pickup] plate</action>
<changestatus> waiting for cooking to complete </changestatus>

turn 2:
<action>[DoNothing]</action>
<changestatus> waiting for cooking to complete </changestatus>

turn 3: 
... 
turn 4:
...

turn 5:
<action>[TakeOut] pan</action>
<changestatus> ready to serve the cooked carrot</changestatus>

turn 6:
<action>[PutInto] order_pickup_area</action>
<changestatus> ready to pick up a carrot and cook it again </changestatus>

Explanation:
This example uses the [Pickup] command to pick up a plate and do nothing for several rounds to wait for cooking to finish. Then use the [TakeOut] command to remove the cooked carrot from the pan. Finally, use the [PutInto] command to put the dish in the order_pickup_area. It is necessary to serve the cooked food on a plate. Finally, change your status.
\\\

\\\
Example 5:
<action>[Takeout] fridge potato</action>
<action>[Open] oven_1</action>
<action>[PutInto] pan_1</action>
<action>[Takeout] fridge potato</action>
<action>[Open] oven</action>
<action>[PutInto] pan</action>
<changestatus>waiting for potatoes to cook done in pan and pan_1 </changestatus>

Explanation:
This is an example of a chef cooking with two pans at the same time, which can significantly increase efficiency.
\\\

\\\
Example 6:
<action>[Takeout] fridge carrot</action>
<action>[PutInto] chopping_board</action>
<action>[Cut] chopping_board</action>
<changestatus>cut the carrot</changestatus>

Explanation:
This is a example of using a chopping_board, using [PutInto] command to put the food on the chopping_board and then using the [Cut] command for the chopping_board.
\\\

\\\
Example 7:
...... (cooking)
<action>[Takeout] pan</action>
<action>[Season] seasoning_station sour</action>

Explanation:
This is a example of using a seasoning_station to flavor.
\\\
";
public const string npcCookTakeActionStringEnd = @"
Now let's think step by step, explain what you should do with 1 or 2 sentences, and then generate your instructions following the information above. The instructions that you generate will be executed one at a time, so do not generate too many instructions, especially if it is something that needs to wait:
";

    public const string npcWaiterTakeActionStringBegin = @"******Task******
Your goal is to maximise the coin income. You can earn different values of coins for sending different dishes or beverages to customers, extra tips for sending dishes within a certain time frame.

### Dish Coin Values:
- **potato_cooked**: 10 coins 
- **carrot_cut**: 10 coins 
- **carrot_cooked**: 15 coins 
- **eggplant_cooked**: 20 coins 
- **steak_cooked**: 30 coins 
- **fried_potato_carrot**: 40 coins 

### Beverage Value:
- Completing a customer's coffee request earns 5 coins. A cup of coffee only costs 1 coin.
  
### Time Constraints:
- Completing a dish within half the maximum waiting time and with the correct flavor (if requested) rewards an additional 20% of the dish's value as a tip.
- Maximum waiting time for a beverage: 60 seconds.
- Maximum waiting time for a dish: 120 seconds.

#### Additional Rules: 
1. Using ""[TakeOut] coffee_machine"" Command to get cup that filled with coffee from coffee_machine.
2. Prioritize serving customers who have been waiting for a long time.
3. You could get infinite plate from plate_stack using [TakeOut] command. (e.g. [TakeOut] plate_stack)
4. Dishes should be served on a plate before they are served to the customer.
5. Customers who order a beverage will only order food after receiving the beverage. If the beverage is not served on time, the customer will leave.

### Instructions:
Based on all the information above, generate specific instructions that satisfy:
1. At least 1 piece of instruction formatted as "" <action> YOUR_ACTION_HERE </action> "" to take action. See available actions in [Available Actions] section.
2. (Optional) 1 piece of instruction formatted as "" <changestatus> YOUR_STATUS_HERE </changestatus> "" to change your status in [Character Status] section.
";

public const string npcWaiterTakeActionStringExamples = @"#### Examples:
Here are 4 examples:
\\\
Example 1: 
<action>[TakeOut] order_pickup_area</action>
<changestatus> ready to serve customer </changestatus> 

Explanation:
This example uses the [TakeOut] command to take out dish from order_pickup_area, and change the status as the future plan: ready to serve the customer.
\\\

\\\
Example 2: 
<action>[DoNothing]</action>
<changestatus> waiting for new order </changestatus> 

Explanation:
Do nothing if there is no order.
\\\

\\\
Example 3: 
<action>[PutInto] desk</action>
<changestatus> ready to take out another dish </changestatus> 

Explanation:
This example using [PutInto] command to put dish that you are holding into desk, and change the status as the future plan: ready to take out another dish.
\\\

\\\
Example 4: 
<action>[TakeOut] coffee_machine</action>
<changestatus> ready to serve customer a cup of coffee</changestatus> 

Explanation:
This example uses the [TakeOut] command get a cup of coffee from coffee_machine, and change the status as the future plan: ready to serve the customer a cup of coffee.
\\\
";

public const string npcWaiterTakeActionStringEnd = @"
Now, Let's think step by step, explain what you should do with 1 or 2 sentence now, and then generate your instruction following the information above:
";

    public const string singleAgentTakeActionString = @"******Task******
Based on all the information above, generate specific instructions that satisfy:
1. At least 1 piece of instruction formatted as "" <action> YOUR_ACTION_HERE </action> "" to take action. See available actions in [Available Actions] section.
2. (Optional) 1 piece of instruction formatted as "" <changestatus> YOUR_STATUS_HERE </changestatus> "" to change your status in [Character Status] section.

Here are 6 examples:
\\\
Example 1: 
<action>[Pickup] carrot</action>
<changestatus>ready to put the carrot in the pan</changestatus> 

Explanation:
this example uses the [Pickup] command to pick up the item named carrot, and change its status to the future plan: put carrot into pan.
\\\

\\\ 
Example 2:
<action>[PutInto] pan</action>
<action>[Open] oven</action>
<changestatus>waiting for cooking to complete</changestatus>

Explanation:
in this example, the [PutInto] command is used to put the item at hand into the pan, and the [Open] command is used to open the oven to start cooking. Then it changes status to waiting for cooking to complete.
\\\

\\\
Example 3:
<action>[TakeOut] fridge carrot</action>
<changestatus>trying to get a carrot from fridge</changestatus>

Explanation:
this example uses the [TakeOut] command to get a carrot from fridge.
\\\

\\\
Example 4:

turn 1:
<action>[Pickup] plate</action>
<changestatus> waiting for cooking to complete </changestatus>

turn 2:
<action>[DoNothing]</action>
<changestatus> waiting for cooking to complete </changestatus>

turn 3: 
... 
turn 4:
...

turn 5:
<action>[TakeOut] pan</action>
<changestatus> ready to serve the cooked carrot</changestatus>

turn 6:
<action>[PutInto] order_pickup_area</action>
<changestatus> ready to pick up a carrot and cook it again </changestatus>

Explanation:
This example uses the [Pickup] command to pick up a plate and do nothing for several rounds to wait for cooking to finish. Then use the [TakeOut] command to remove the cooked carrot from the pan. Finally, use the [PutInto] command to put the dish in the order_pickup_area. It is necessary to serve the cooked food on a plate. Finally, change your status.
\\\

\\\
Example 5:
<action>[Pickup] plate</action>
<action>[Takeout] pan</action>
<changestatus> take cooked carrot from pan by picking up a plate first and ready to serve the cooked carrot</changestatus>

Explanation:
This is an important example because it shows that you need a plate to take the cooked food out of the pan.
\\\

\\\
Example 6:
<action>[PutInto] desk</action>
<changestatus> ready to take out another dish </changestatus> 

Explanation:
this example using [PutInto] command to put dish that you are holding into desk, and change the status as the future plan: ready to take out another dish.

\\\

\\\
#### Additional Rules: 
1. it is necessary to have a plate to [TakeOut] cooked food from the pan.
2. one carrot\eggplant\steak is enough to cook.
3. observe the cooking time of the pan in the [Items in the Scene] section, remember to take out your cooked food.
4. remember to open the oven. 
5. you could get infinite food from fridge using [TakeOut] command. (e.g. [TakeOut] fridge carrot)
6. you could get infinite plate from plate_stack using [TakeOut] command. (e.g. [TakeOut] plate_stack)
7. you could get infinite cup that filled with coffee from coffee_machine using [TakeOut] command. (e.g. [TakeOut] coffee_machine)
8. observe [Item You Hold] to see whether you need pick up a plate first.
9. view [Your Previous Actions] section to prevent duplicate actions.
10. Prioritize serving customers who have been waiting for a long time.
\\\


Now, think and generate your instruction:
";
    public const string customerWaiterTalkStringBegin = @"Simulate a dialogue between a waiter and a customer in a restaurant, ending with the customer confirming their order. The waiter is responsible for presenting the menu and making recommendations, and the customer needs to choose a dish based on their preferences. Here is the menu for the day:
  Dish Name   | Cost Coin
potato_cooked | 10
carrot_cut | 10    
carrot_cooked | 15   
eggplant_cooked | 20 
steak_cooked | 30  
fried_potato_carrot | 40

As a customer, you should order according to your needs and preferences.
As a waiter, you should recommend dishes according to maximising the restaurant's business benefits, such as the price of the dish and the available ingredients.

";
    public const string customerWaiterTalkStringEnd = @"
For ease of extracting the result, please output the customer's order using a command after the dialogue is complete. Use the format ""<order>xxx</order>"", replacing ""xxx"" with the name of the dish from the menu.
For example:
<order>carrot_cooked</order>

Here is a sample dialogue:
\\\
Waiter: Hello, what would you like to order?
Customer: I feel like having some beef, do you have it here?
Waiter: Of course, you can order a steak.
Customer: Alright, I'll have the steak then.

<order>steak_cooked</order>
\\\

Please note:
1. Only one dish can be ordered.
2. Remember to generate a command outputting the customer's desired dish after the dialogue is completed.

Now, please generate a dialogue and the result of the order:
";

    public const string messagePoolPromptingStr = @"IMPORTRANT FOR COOPERATION: To change your content in the message pool, you should use the following JSON-like format:
<messagepool>
{
""sender"": ""YOUR_NAME"",
""receiver"": ""RECEIVER_NAME"",
""message"": ""MESSAGE""
}
</messagepool>

The receiver can be a specific agent name or a group, such as ""all"", ""cook"", or ""waiter"". Since cooperation with others is crucial, you are encouraged to send messages frequently to let other agents know your demands or status. If the [Message Pool] section is empty, you must provide content using the format above.
";

    public const string decentralizedPromptingStr = @"IMPORTRANT FOR COOPERATION: **one or multiple** instructions to send messages to other agents. You should generate each instruction using the following JSON-like format:
<message>
{
""receiver"": ""RECEIVER_NAME"",
""message"": ""MESSAGE""
}
</message>

The receiver should be a specific agent name. Since cooperation with others is crucial, you are encouraged to send messages frequently to let other agents know your demands or status. If the [Peer-To-Peer Messages] section is empty, you must provide content using the format above.
";

    public const string recipeStr = @"cook potato_cooked
{
material:  potato x 1
steps: put a potato into a pan -> open the oven and wait, take out food of pan using a plate
}


cook eggplant_cooked
{
material:  eggplant x 1
steps: put a eggplant into a pan -> open the oven and wait, take out food of pan using a plate
}


cook steak_cooked
{
material:  steak x 1
steps: put a steak into a pan -> open the oven and wait, take out food of pan using a plate
}


cook carrot_cooked
{
material:  carrot x 1
steps: cut carrot at a chopping board -> put the sliced carrot into a pan -> open the oven and wait, take out food of pan using a plate
}


cook carrot_cut
{
material:  carrot x 1
steps: cut carrot at a chopping board -> put the sliced carrot into a plate
Note: Since it's not cooked, you will get a food item named 'carrot (cut)', not 'carrot_cut'. Using '[Pickup] carrot' to get the sliced carrot is fine, as the content inside the brackets indicates the state of the item. 
}


cook fried_potato_carrot
{
material:  carrot x 1, potato x 1
steps: cut carrot at a chopping board -> put the sliced carrot and a potato into a pan -> open the oven and wait, take out food of pan using a plate
}
";
    // customer
    public const float waitingForBeverageMaxTime = 120f;
    public const float waitingForDishMaxTime = 120f;
    public const float waitingForRecommendationMaxTime = 120f;

    // manager
    public const float managerGuidanceCd = 20f;
    // public const string managerGuideCookString = "You are the manager of the restaurant, and in order to ensure the restaurant operates smoothly, you need to provide suggestions for the actions of the waiter and cook based on the current situation of the restaurant. Now, please combine the information below to offer suggestions for the cook's actions.";
    // public const string managerGuideWaiterString = "You are the manager of the restaurant, and in order to ensure the restaurant operates smoothly, you need to provide suggestions for the actions of the waiter and cook based on the current situation of the restaurant. Now, please combine the information below to offer suggestions for the waiter's actions.";

    public const string managerGuideStartString = @"You are the manager of a 2D game restaurant. Your goal is to maximize the coin income. You need to ensure effective cooperation and communication among all agents to achieve this goal. Below are the important details regarding the tasks and coin values:

#### Coin Income and Costs
You can earn different values of coins for completing different dishes for customers, with extra tips for completing dishes within a certain time frame. Coins are also consumed for picking up raw materials from the fridge and serving beverages.

#### Dish Coin Values and Raw Material Costs:
  - **potato_cooked**: 10 coins | (potato, 3 coins)
  - **carrot_cut**: 10 coins | (carrot, 3 coins)
  - **carrot_cooked**: 15 coins | (carrot, 3 coins)
  - **eggplant_cooked**: 20 coins | (eggplant, 5 coins)
  - **steak_cooked**: 30 coins | (steak, 8 coins)
  - **fried_potato_carrot**: 40 coins | (potato, 3 coins), (carrot, 3 coins)

#### Beverage Values and Raw Material Costs:
- **coffee**: 5 coins | (None, 1 coins)

#### Flavor Requests:
- If a customer requests a specific flavor, serving the correct flavor adds +5 coins.
- Serving an incorrect flavor results in -5 coins.
  
#### Time Constraints:
- Maximum waiting time for a beverage: 60 seconds.
- Maximum waiting time for a dish: 120 seconds.

### Manager's Role:
As the manager, you need to oversee the entire operation and provide one-on-one guidance to each agent. Ensure that all agents are working efficiently, and address any issues that arise to prevent conflicts. Communication and coordination are key to maximizing coin income.

Remember, a pan can only cook one dish at a time. Make sure your agents are aware of this to prevent conflicts and ensure smooth operation.

### Previous Agent Suggestions:
To help you understand the current situation, here are the last suggestions you provided to each agent:
";
    public const string managerGuideCookTaskBegin = @"******Task******
As the manager, you need to fully understand all the information and guide the cook accordingly.
### Additional Rules: 
1. It is necessary to have a plate to [TakeOut] cooked food from the pan.
2. Observe the cooking time of the pan in the [Items in the Scene] section, remember to take out your cooked food.
3. Remember to open the oven. 
4. You could get infinite food from fridge using [TakeOut] command. (e.g. [TakeOut] fridge carrot)
5. You could get infinite plate from plate_stack using [TakeOut] command. (e.g. [TakeOut] plate_stack)
6. Observe [Item You Hold] to see if you are holding an item.
7. View [Your Previous Actions] section to prevent duplicate actions.
8. Seasoning the dish if the order is accompanied by a 'flavour description' message , and determine which flavour(s) is/are to be blended.
9. Dishes should be served on a plate before they are served to the customer.
10. A pan with carrot_cooked (potato_cooked) in it can still add potato (carrot) to cook fried_potato_carrot.
11. A pan can only cook one dish at a time. Remember to communicate effectively and coordinate with your partner to allocate kitchen tools properly and avoid conflicts.

### Instructions:
Based on all the information above, generate specific instructions that satisfy:
1. At least 1 piece of instruction formatted as "" <action> YOUR_ACTION_HERE </action> "" to take action. See available actions in [Available Actions] section.
2. (Optional) 1 piece of instruction formatted as ""<changestatus> YOUR_STATUS_HERE </changestatus>"" to change your status in [Character Status] section.   
3. (Optional) 1 piece of instruction formatted as ""<suggestion> YOUR_SUGGESTION_HERE </suggestion>"" to provide guidance. This is helpful for you to record what you have done.
";

public const string managerGuideWaiterTaskBegin = @"******Task******
As the manager, you need to fully understand all the information and guide the waiter accordingly.
### Additional Rules: 
1. Using ""[TakeOut] coffee_machine"" Command to get a cup of coffee from coffee_machine.
2. Prioritize serving customers who have been waiting for a long time.
3. You could get infinite plate from plate_stack using [TakeOut] command. (e.g. [TakeOut] plate_stack)
4. Dishes should be served on a plate before they are served to the customer.
5. Customers who order a beverage will only order food after receiving the beverage. If the beverage is not served on time, the customer will leave.

### Instructions:
Based on all the information above, generate specific instructions that satisfy:
1. At least 1 piece of instruction formatted as "" <action> YOUR_ACTION_HERE </action> "" to take action. See available actions in [Available Actions] section.
2. (Optional) 1 piece of instruction formatted as "" <changestatus> YOUR_STATUS_HERE </changestatus> "" to change your status in [Character Status] section.   
3. (Optional) 1 piece of instruction formatted as ""<suggestion> YOUR_SUGGESTION_HERE </suggestion>"" to provide guidance. This is helpful for you to record what you have done.
";

public const string managerChallengeLevelBegin = @"You are the manager of a 2D game restaurant. Your goal is to sustain the restaurant operation, prevent bankruptcy, and achieve higher revenue. Effective cooperation and communication among all agents are crucial to achieve this goal. Below are the important details regarding the tasks and coin values:

#### Coin Income and Costs:
You can earn different values of coins for completing different dishes for customers, with extra tips for completing dishes within a certain time frame. Coins are also consumed for picking up raw materials from the fridge and serving beverages.

#### Dish Coin Values and Raw Material Costs:
- **potato_cooked**: 10 coins | (potato, 3 coins)
- **carrot_cut**: 10 coins | (carrot, 3 coins)
- **carrot_cooked**: 15 coins | (carrot, 3 coins)
- **eggplant_cooked**: 20 coins | (eggplant, 5 coins)
- **steak_cooked**: 30 coins | (steak, 8 coins)
- **fried_potato_carrot**: 40 coins | (potato, 3 coins), (carrot, 3 coins)

#### Beverage Values and Raw Material Costs:
- **coffee**: 5 coins | (None, 1 coin)

#### Flavor Requests:
- If a customer requests a specific flavor, serving the correct flavor adds +5 coins.
- Serving an incorrect flavor results in -5 coins.

#### Time Constraints:
- Maximum waiting time for a beverage: 60 seconds.
- Maximum waiting time for a dish: 120 seconds.

At the end of each day, you will pay 20 coins for rent and 20 coins per employee as wages. These amounts may change due to certain events.

### Manager's Role:
The manager's main responsibilities include recruiting/dismissing employees, managing agent cooperation strategies, and conducting reflections with agents at the end of each day to improve their efficiency.";



    // item settings
    public const float cuttingFoodTime = 2f;

    // game settings
    public const int maxHistoryNum = 30;

    // %%%%% delete later
    public const int levelNum = 5;
    public static readonly int[] levelGoals = {100, 250, 500, 750, 1000};
    // public static readonly float[] levelTimeLimit = {5, 5, 5, 5, 5};
    public static readonly float[] levelTimeLimit = {120, 150, 180, 180, 180};
    public static readonly int[] levelCustomerNum = {7, 10, 15};
    public static readonly int[] levelDishNum = {1, 2, 3, 4};
    public static readonly float[] levelOrderDrinkProbability = {0f, 0.2f, 0.4f, 0.5f, 0.5f};
    public static readonly float[] levelRecommendationProbability = {0f, 0f, 0.3f, 0.4f, 0.5f};
    // %%%%% 
    public static readonly int[] taskNumPerLevelList = {10, 10, 10, 15, 15, 15, 20, 20, 20, 25}; 

    public static readonly int[] agentNumPerLevelList = {2, 2, 2, 2, 3, 3, 3, 5, 7, 7}; 

    public static readonly string[] dishPerLevelList = {
        "potato_cooked",
        "potato_cooked, eggplant_cooked",
        "potato_cooked, carrot_cut, carrot_cooked",
        "potato_cooked, carrot_cut, carrot_cooked",
        "potato_cooked, carrot_cut, carrot_cooked, fried_potato_carrot",
        "potato_cooked, carrot_cut, carrot_cooked, fried_potato_carrot, eggplant_cooked, steak_cooked"
    };
    public static readonly string[][] dishPerChallengeLevelList = {
        new string[] { "potato_cooked", "carrot_cut", "eggplant_cooked"},
        new string[] { "potato_cooked", "carrot_cut", "carrot_cooked","eggplant_cooked"},
        new string[] { "potato_cooked", "carrot_cut", "carrot_cooked", "fried_potato_carrot", "eggplant_cooked" },
        new string[] { "potato_cooked", "carrot_cut", "carrot_cooked", "fried_potato_carrot", "eggplant_cooked", "steak_cooked" }
    };


    // special events
    public static int extraCostForIngredients = 0;
    public static int cookingTimeIncrease = 0;
    public static int dailyWageDecrease = 0;
    public static int dailyWageIncrease = 0;
    public static int permanentWageIncrease = 0;
    public static int permanentRentIncrease = 0;
    public static int originalDailyWage = 20; 
    public static int originalDailyRent = 20;  
    public static int dailyWage = originalDailyWage;
    public static int dailyRent = originalDailyRent;
    public static int cookingTimeDecreaseToday = 0;
    public static int extraBonusToday = 0;
    public static int maxWaitingTimeDecreaseToday = 0;
    public static bool noBeveragesToday = false;
    public static bool recommendDishesToday = false;

    public static int extraActionTik = 0;
    public static int extraBeverageCost = 0;
    public static int extraBeveragerevenue = 0;
    public static int penaltyForLateDishes = 0;

}
