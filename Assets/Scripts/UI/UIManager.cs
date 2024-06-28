using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public GameObject fridgeSpace;
    public GameObject randomEventBoard;
    public GameObject levelBoard;
    private GameObject targetFridge;
    public GameObject slot;
    public GameObject progressBar;

    public GameObject canvas;

    void Start()
    {
        // ShowLevelBoard();
    }

    public GameObject CreateProgressBar(Vector3 pos)
    {
        GameObject new_progressBar = Instantiate(progressBar, Vector3.zero, Quaternion.identity, canvas.transform);
        new_progressBar.transform.position = pos;
        return new_progressBar;
    }

    public void ShowFridgeSpace(Vector3 newPos, GameObject _gameobejct)
    {
        fridgeSpace.SetActive(true);
        fridgeSpace.transform.position = newPos;
        targetFridge = _gameobejct;
        // slot.GetComponent<Button>().onClick.AddListener(TestClick);
        slot.GetComponent<Button>().onClick.AddListener(targetFridge.GetComponent<Fridge>().GiveFood);
    }

    public void HideFridgeSpace()
    {
        slot.GetComponent<Button>().onClick.RemoveAllListeners();
        fridgeSpace.SetActive(false);

    }

    public void HideRandomEventBoard()
    {
        randomEventBoard.SetActive(false);
    }

    public void HideLevelBoard()
    {
        TimeManager.Instance.ResumeGame();
        levelBoard.SetActive(false);
    }

    public void ShowLevelBoard()
    {
        levelBoard.SetActive(true);
    }


}
