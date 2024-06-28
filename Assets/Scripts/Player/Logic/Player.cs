using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private ItemPickUp itemPickUp;
    private ItemInteraction itemInteraction;
    private ItemDrop itemDrop;
    private float itemHolderPositionOffsetX;
    private float itemHolderPositionOffsetY;

    public float speed;
    private float inputX;
    private float inputY;

    private Vector2 movementInput;
    private Animator[] animators;
    private bool isMoving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        itemPickUp = GetComponentInChildren<ItemPickUp>();
        itemInteraction = GetComponentInChildren<ItemInteraction>();
        itemDrop = GetComponentInChildren<ItemDrop>();
    }

    private void Update()
    {
        PlayerInput();
        SwitchAnimation();
    }

    private void FixedUpdate()
    {
        Movement();
        // Debug.Log(itemPickUp.transform.position.x + itemHolderPositionOffsetX);
        itemPickUp.transform.position = new(transform.position.x + itemHolderPositionOffsetX, transform.position.y + 0.25f + itemHolderPositionOffsetY, transform.position.z); 
    }

    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        if (inputX != 0 && inputY != 0) {
            inputX *= 0.7f;
            inputY *= 0.7f;
        }
        movementInput = new Vector2(inputX, inputY);

        isMoving = movementInput != Vector2.zero;

        if (isMoving) {
            // change item position that playing is holding
            if (inputX < -0.5f && (Math.Abs(inputY) < 0.5f || inputY > 0)) {
                itemHolderPositionOffsetX = -0.65f;
                itemHolderPositionOffsetY = 0.25f;
            }
            else if (inputX > 0.5f && (Math.Abs(inputY) < 0.5f || inputY > 0)) {
                itemHolderPositionOffsetX = 0.65f;
                itemHolderPositionOffsetY = 0.25f;
            }
            else {
                itemHolderPositionOffsetX = 0;
                itemHolderPositionOffsetY = 0;
            }

            if (inputY > 0 && Mathf.Abs(inputY) > Mathf.Abs(inputX)) {
                itemHolderPositionOffsetY = 0.4f;
            }

            // change layout sorting order
            if (Mathf.Abs(inputX) <= 0.5f && inputY > 0) {
                spriteRenderer.sortingOrder = 99;
            }
            else {
                spriteRenderer.sortingOrder = -1;
            }
        }

        PickUp();
        Interact();
        Drop();
    }

    void Movement()
    {   
        if (!TimeManager.Instance.IsPaused()) {
            rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
        }
    }

    private void SwitchAnimation()
    {
        foreach (var anim in animators)
        {
            anim.SetBool("IsMoving", isMoving);
            if (isMoving)  {
                anim.SetFloat("DirX", inputX);
                anim.SetFloat("DirY", inputY);
            }
            anim.SetBool("IsCarry", itemPickUp.holdItem != null);
        }
    }

    void PickUp() 
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            itemPickUp.PickUpTarget();
        }
    }

    void Interact()
    {
        // Debug.Log("Interact");
        if (Input.GetKeyDown(KeyCode.Space)) {
            itemInteraction.InteractWithItem();
        }     
    }

    void Drop()
    {
        if (Input.GetKeyDown(KeyCode.Q)) {
            itemDrop.DropItem();
        } 
    }
}
