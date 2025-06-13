using UnityEngine;
using TMPro;

public class BarController : MonoBehaviour
{
    [SerializeField] RectTransform bar;
    [SerializeField] TextMeshProUGUI text;
    
    public float Barwidth;
    private void OnEnable()
    {
        Barwidth = GetComponent<RectTransform>().rect.size.x;
        Debug.Log("StartWidth BAr = "+Barwidth);
    }

    private void Start()
    {
        //oxygenController.OxygenUpdated += SetBar;
    }


    public void SetBar(int value, float MaxValue)
    {
        float percent = value / (float)MaxValue;
        float sizeDeltaNew = -Barwidth * (1 - percent);
        text.text = value + "/" + MaxValue;
        bar.sizeDelta = new Vector2(-Barwidth * (1 - percent), 0);
    }
}
