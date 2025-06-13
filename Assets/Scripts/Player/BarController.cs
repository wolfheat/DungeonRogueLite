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
        Debug.Log("percent for "+name+" is "+percent+" max = "+MaxValue);
        float sizeDeltaNew = -Barwidth * (1 - percent);
        
        Debug.Log("Barwidth = " + Barwidth);
        Debug.Log("-Barwidth * (1 - percent) = " + sizeDeltaNew);
        //text.text = (percent*100).ToString("F2")+"/"+MaxValue;
        text.text = value + "/" + MaxValue;
        bar.sizeDelta = new Vector2(-Barwidth * (1 - percent), 0);
        //bar.sizeDelta = new Vector2(-Barwidth * (1 - value), 0);
        Debug.Log("Bar = "+bar.sizeDelta.x);
    }
}
