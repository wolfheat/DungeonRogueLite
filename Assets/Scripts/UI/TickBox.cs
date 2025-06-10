using UnityEngine;

public class TickBox : MonoBehaviour
{
    [SerializeField] private GameObject active; 
    [SerializeField] private GameObject inActive;

    public void SetActive(bool setToActive)
    {
        Debug.Log("Setting box active: "+setToActive+" "+name);
        active.SetActive(setToActive);
        inActive.SetActive(!setToActive);
    }
}
