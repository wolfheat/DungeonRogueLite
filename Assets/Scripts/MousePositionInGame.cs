using UnityEngine;

public class MousePositionInGame : MonoBehaviour
{
	[SerializeField] private TargetSelection tileShower; 

	public static MousePositionInGame Instance { get; private set; }
	private void Awake()
	{
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

    private void Update()
    {
		// Figure out if enemy is reachable and attackable from this position
		bool canAttackFromHere = PlayerInteract.Instance.IsAttackable();

        tileShower.SetSelector(new Vector3(Mathf.RoundToInt(transform.position.x), 0.05f, Mathf.RoundToInt(transform.position.z)), canAttackFromHere);
    }
    //private void Update() => tileShower.transform.position = new Vector3((int)(transform.position.x-0.5f), 0.05f, (int)(transform.position.z + 0.5f));

}
