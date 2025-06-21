using UnityEngine;


public enum TargetType{Square, Target}

public class TargetSelection : MonoBehaviour
{
    [SerializeField] private GameObject square; 
    [SerializeField] private GameObject target; 


    public void SetSelector(Vector3 pos, bool canAttack)
    {
        square.SetActive(!canAttack);
        target.SetActive(canAttack);

        transform.position = pos;
    }

}
