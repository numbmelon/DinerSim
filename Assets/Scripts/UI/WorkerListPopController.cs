using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerListPopController : MonoBehaviour
{
    public GameObject workerList;
    public void OpenPop()
    {
        workerList.SetActive(true);
    }

    public void ClosePop()
    {
        workerList.SetActive(false);
    }
}
