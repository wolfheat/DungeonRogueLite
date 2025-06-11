using UnityEngine;

public class TickBox : MonoBehaviour
{
    [SerializeField] private GameObject active; 
    [SerializeField] private GameObject inActive;

    public void SetActive(bool setToActive)
    {
        active.SetActive(setToActive);
        inActive.SetActive(!setToActive);
    }
}
