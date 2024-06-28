using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMagent.Astar;
using System;

public class NPC : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private ItemPickUp itemPickUp;
    private ItemInteraction itemInteraction;
    private ItemDrop itemDrop;
    private float itemHolderPositionOffsetX;
    private float itemHolderPositionOffsetY;

    public float speed;
    private Vector2 movDir;
    private bool isMoving;

    private Stack<MovementStep> movementSteps = new();

    private Vector2Int currentGridPosition;
    private Vector2Int targetGridPosition;
    private Vector2Int nextGridPosition;

    private Animator[] animators;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        itemPickUp = GetComponentInChildren<ItemPickUp>();
        itemInteraction = GetComponentInChildren<ItemInteraction>();
        itemDrop = GetComponentInChildren<ItemDrop>();
    }
    
    void Start()
    {
        currentGridPosition = (Vector2Int)GridMapManager.Instance.grid.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize / 2f, currentGridPosition.y + Settings.gridCellSize / 2f, 0);
        // StartCoroutine(AddImmediateMovementRoutine(new Vector2Int(4, -8)));
    }

    void OnEnable()
    {
        StartCoroutine(AddTransformToCameraSwitch());
    }

    void OnDisable()
    {
        // CameraSwitcher.Instance?.RemoveParent(transform);
        StopAllCoroutines();
    }

    IEnumerator AddTransformToCameraSwitch()
    {
        while (CameraSwitcher.Instance == null) yield return null;
        CameraSwitcher.Instance.AddParent(transform);
    }


    void Update()
    {
        currentGridPosition = (Vector2Int)GridMapManager.Instance.grid.WorldToCell(transform.position);

        if (isMoving) {
            if (movDir.x < -0.5f && (Math.Abs(movDir.y) < 0.5f || movDir.y > 0)) {
                itemHolderPositionOffsetX = -0.65f;
                itemHolderPositionOffsetY = 0.25f;
            }
            else if (movDir.x > 0.5f && (Math.Abs(movDir.y) < 0.5f || movDir.y > 0)) {
                itemHolderPositionOffsetX = 0.65f;
                itemHolderPositionOffsetY = 0.25f;
            }
            else {
                itemHolderPositionOffsetX = 0;
                itemHolderPositionOffsetY = 0;
            }

            if (movDir.y > 0 && Mathf.Abs(movDir.y) > Mathf.Abs(movDir.x)) {
                itemHolderPositionOffsetY = 0.4f;
            }
        }

            if (Mathf.Abs(movDir.x) <= 0.5f && movDir.y > 0) {
                spriteRenderer.sortingOrder = 99;
            }
            else {
                spriteRenderer.sortingOrder = -1;
            }

        SwitchAnimation();
    }

    private void FixedUpdate()
    {
        itemPickUp.transform.position = new(transform.position.x + itemHolderPositionOffsetX, transform.position.y + 0.25f + itemHolderPositionOffsetY, transform.position.z); 
    }

    private Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        Vector3 worldPos = GridMapManager.Instance.grid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2f);
    }

    private void SwitchAnimation()
    {
        foreach (var anim in animators)
        {
            anim.SetBool("IsMoving", isMoving);
            if (isMoving)  {
                anim.SetFloat("DirX", movDir.x);
                anim.SetFloat("DirY", movDir.y);
            }
            anim.SetBool("IsCarry", itemPickUp.holdItem != null);
        }
    }

    private IEnumerator SetStopAnimation()
    {
        movDir.x = 0;
        movDir.y = -1;
        itemHolderPositionOffsetX = 0;
        itemHolderPositionOffsetY = 0;


        // if (anim.GetComponent<RuntimeAnimatorController>() == null) yield break; 
        foreach (var anim in animators) {
            anim.SetFloat("DirX", movDir.x);
            anim.SetFloat("DirY", movDir.y);
        }

        // TODO: play stop animation
        yield return null;
    }

    public void FindPath(Vector2Int targetGridPosition)
    {
        movementSteps.Clear();
        this.targetGridPosition = targetGridPosition;
        Astar.Instance.BuildPath(currentGridPosition, targetGridPosition, movementSteps);
    }

    public IEnumerator Movement(Action callback = null)
    {
        while (TimeManager.Instance.IsPaused()) {
            yield return new WaitForFixedUpdate();
        }

        if (movementSteps.Count > 1) {
            nextGridPosition = movementSteps.Pop().gridCoordinate;
            // Debug.Log(nextGridPosition);
            isMoving = true;
        }

        while (movementSteps.Count > 0 || Vector2.Distance(rb.position, (Vector2)GetWorldPosition((Vector3Int)nextGridPosition)) > 0.05f) {
            // Debug.Log(nextGridPosition);
            movDir =  ((Vector2)GetWorldPosition((Vector3Int)nextGridPosition) - (Vector2)transform.position).normalized;
            Vector2 newPosition = rb.position + movDir * speed * Time.fixedDeltaTime;
            
            while (TimeManager.Instance.IsPaused()) {
                yield return new WaitForFixedUpdate();
            }

            rb.MovePosition(newPosition);
            if (Vector2.Distance(rb.position, (Vector2)GetWorldPosition((Vector3Int)nextGridPosition)) <= 0.05f) {
                rb.MovePosition((Vector2)GetWorldPosition((Vector3Int)nextGridPosition));
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

    public IEnumerator AddImmediateMovementRoutine(Vector2Int targetGridPosition)
    {
        movementSteps.Clear();
        this.targetGridPosition = targetGridPosition;
        currentGridPosition = (Vector2Int)GridMapManager.Instance.grid.WorldToCell(transform.position);
        // Debug.Log(currentGridPosition + "1111");
        Astar.Instance.BuildPath(currentGridPosition, targetGridPosition, movementSteps);
        StartCoroutine(Movement());
        yield return null;
    }
}
