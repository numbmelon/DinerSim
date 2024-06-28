using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CoinNumSlot : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;

    private void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

        UpdateText(CoinManager.Instance.CoinNum);
        
        EventHandler.OnCoinNumChanged += UpdateText;
    }

    private void OnDestroy()
    {
        EventHandler.OnCoinNumChanged -= UpdateText;
    }

    private void UpdateText(int newCoinNum)
    {
        textMeshPro.text = newCoinNum.ToString();
    }
}
