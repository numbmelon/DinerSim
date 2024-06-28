using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    private ItemPickUp itemPickUp;
    private GameObject targetItem;
    void Awake()
    {
        itemPickUp = GetComponent<ItemPickUp>();
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        Item item = other.GetComponent<Item>();
        // TODO: choose one from many items 
        if (item != null && item.interactable) {
            Item holdItem = null;
            if (itemPickUp?.holdItem != null) {
                holdItem = itemPickUp?.holdItem.GetComponent<Item>();
            }
            if (item.IsMeetInteractionCondition(holdItem)) {
                if (targetItem == null || targetItem.GetComponent<Item>().interactPriority < item.interactPriority) {
                    targetItem = item.gameObject;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        Item item = other.GetComponent<Item>();
        if (item != null && item.gameObject == targetItem) {
            targetItem = null;
        }
    }

    public void InteractWithItem()
    {
        if (targetItem == null) return;
        if (itemPickUp != null) {
            // check type of items that player is holding 
            Item holdItem = null;
            if (itemPickUp?.holdItem != null) {
                holdItem = itemPickUp.holdItem.GetComponent<Item>();
            }
            targetItem.GetComponent<Item>().Interact(holdItem);
        }
    }
}
