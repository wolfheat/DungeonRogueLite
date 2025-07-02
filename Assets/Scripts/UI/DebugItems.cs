
using TMPro;
using UnityEngine;

public class DebugItems : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProText;

    public void SetText(string text)
    {
        textMeshProText.text = text;
    }
}
