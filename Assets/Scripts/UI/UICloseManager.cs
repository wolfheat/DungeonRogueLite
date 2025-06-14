using System;
using System.Collections;
using UnityEngine;

public class UICloseManager : MonoBehaviour
{
    [SerializeField] private GameObject[] CloseAtStart; 
    [SerializeField] private GameObject[] OpenAtStart; 
    [SerializeField] private GameObject[] ToggleAtStart; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject panel in OpenAtStart) 
            panel.SetActive(true);

        StartCoroutine(WaitAndClose());

    }

    private IEnumerator WaitAndClose()
    {
        yield return null;
        foreach (GameObject panel in CloseAtStart) 
            panel.SetActive(false);
    }
}
