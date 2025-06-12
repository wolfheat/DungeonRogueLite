using System;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class SpecialInfo : MonoBehaviour
{

    private TextMeshProUGUI infoText;

    public static SpecialInfo Instance { get; private set; }

    private string totalInfo = "INFO";

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        infoText = GetComponent<TextMeshProUGUI>();
        Instance = this;
    }

    public void ShowInfo(string info)
    {        
        totalInfo += "\n" + info;
        infoText.text = totalInfo;
    }

}
