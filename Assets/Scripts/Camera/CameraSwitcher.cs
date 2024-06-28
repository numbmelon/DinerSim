using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : Singleton<CameraSwitcher>
{
    public Transform cameraTransform; 
    public List<Transform> parents;

    public int currentParentId = 0;
    private bool isFreeMode = false;
    private Vector3 lastMousePosition;

    void Start()
    {
        EventHandler.AfterCompleteCurrentLevelEvent += InitCamera;
        EventHandler.AfterCurrentChallengeDayEvent += InitCamera;
        
    }

    void Update()
    {
        if (parents.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SwitchParent(currentParentId - 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SwitchParent(currentParentId + 1);
        }

        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            if (delta.magnitude > 1)
            {
                isFreeMode = true;
            }

            if (isFreeMode)
            {
                Vector3 cameraMove = new Vector3(-delta.x, -delta.y, 0) * Time.deltaTime * 8; // camera move speed
                cameraTransform.Translate(cameraMove, Space.World);
            }

            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastMousePosition = Vector3.zero;
        }
    }

    void SwitchParent(int idx)
    {
        isFreeMode = false;
        currentParentId = (idx + parents.Count) % parents.Count;
        cameraTransform.SetParent(parents[currentParentId], false);
        cameraTransform.localPosition = Vector3.zero;
        cameraTransform.localRotation = Quaternion.identity;
        cameraTransform.position += new Vector3(0, 0, -10);
    }

    public void InitCamera()
    {
        parents.Clear();
        currentParentId = 0;
        cameraTransform.SetParent(null);
        cameraTransform.localPosition = Vector3.zero;
        cameraTransform.localRotation = Quaternion.identity;
        cameraTransform.position += new Vector3(0, 0, -10);
        isFreeMode = false;
    }

    public void AddParent(Transform parent)
    {
        if (!parents.Contains(parent))
        {
            parents.Add(parent);
            if (parents.Count == 1)
            {
                SwitchParent(0);
            }
        }
    }

    public void RemoveParent(Transform parent)
    {
        if (parents.Contains(parent))
        {
            bool isCurrentParent = cameraTransform.parent == parent;

            parents.Remove(parent);

            if (isCurrentParent)
            {
                if (parents.Count > 0)
                {
                    SwitchParent(0);
                }
                else
                {
                    cameraTransform.SetParent(null);
                }
            }

            if (currentParentId >= parents.Count)
            {
                currentParentId = 0;
            }
        }
    }
}
