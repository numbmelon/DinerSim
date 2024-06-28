using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LMInfo_SO", menuName = "LM/LMInfo")]
public class LMInfo_SO : ScriptableObject
{
    public string llmName;
    public string llmBackendUrl;
}
