using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class ItemPickUp : MonoBehaviour
{
    public GameObject holdItem;
    private GameObject targetItem;


    void Update()
    {
        if (holdItem != null) {
            holdItem.transform.position = transform.position;
        }
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        Item item = other.GetComponent<Item>();
        // TODO: choose one from many items 
        if (item != null && item.pickable && item.ispicked == false) {
            targetItem = item.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        Item item = other.GetComponent<Item>();
        if (item != null && item.gameObject == targetItem) {
            targetItem = null;
        }
    }

    public void PickUpTarget(GameObject targetItem = null) 
    {
        if (targetItem == null) targetItem = this.targetItem;
        if (targetItem == null || targetItem.GetComponent<Item>() == null || targetItem.GetComponent<Item>().ispicked || targetItem.GetComponent<Item>().pickable == false) return;

        Item item = targetItem.GetComponent<Item>();

        if (item.ispicked) return;

        if (item != null && item.physicalParent != null) {
            item.physicalParent.GetComponent<Item>()?.ItemBePickedUp();
        }


        if (holdItem != null) {
            Plate plate = holdItem.GetComponent<Plate>();
            Food food = targetItem.GetComponent<Food>();
            if (plate != null && food != null && plate.food == null) {
                plate.PutFoodIntoPlate(food);
                this.targetItem = null;
                return;
            }

            plate = targetItem.GetComponent<Plate>();
            food = holdItem.GetComponent<Food>(); 
            if (plate != null && food != null && plate.food == null) {
                plate.PutFoodIntoPlate(food);
            }

            // swap position
            holdItem.GetComponent<Item>().ispicked = false;
            holdItem.transform.position = targetItem.transform.position;
        }
        holdItem = targetItem;
        holdItem.transform.position = transform.position;   
        holdItem.GetComponent<Item>().ispicked = true;

        this.targetItem = null;     
    }

    public void ExchangeItem(GameObject itemObject) 
    {
        Item item = itemObject.GetComponentInChildren<Item>();

        if (holdItem != null) {
            holdItem.GetComponent<Item>().ispicked = false;
            holdItem.transform.position = itemObject.transform.position;
        }
        holdItem = itemObject;
        if (item != null) {
            holdItem.transform.position = transform.position;
            holdItem.GetComponent<Item>().ispicked = true;
        }

        targetItem = null;
    } 
}
