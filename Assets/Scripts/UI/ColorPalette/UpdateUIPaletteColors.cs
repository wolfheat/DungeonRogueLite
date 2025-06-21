using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUIPaletteColors : MonoBehaviour
{
    [SerializeField] private Color UIBackgroundA; 
    [SerializeField] private Color UIBackgroundB; 
    [SerializeField] private Color UIBackgroundC; 

    [SerializeField] private Color UIHeaderTextA; 
    [SerializeField] private Color UIHeaderTextB;
    
    [SerializeField] private Color UIMiddleText; 
    [SerializeField] private Color UIMinorText;

    [SerializeField] private GameObject[] BackgroundsA;
    [SerializeField] private GameObject[] TextsHeadersA;
    [SerializeField] private GameObject[] TextsMiddlesA;
    [SerializeField] private GameObject[] TextsMinorsA;


    [ContextMenu("Set All Colors")]
    public void SetAllColors()
    {
        Debug.Log("Setting all UI colors");

        foreach (GameObject go in BackgroundsA) {
            if(go.TryGetComponent<Image>(out Image image)) {
                Debug.Log("Found Image on "+go.name+" setting it to color A");
                image.color = UIBackgroundA;
            }
        }
    }
}
