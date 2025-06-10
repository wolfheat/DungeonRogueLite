using UnityEngine;

public class CenterOverPlayer : MonoBehaviour
{
	public static CenterOverPlayer Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

    public void Center(Vector3 pos) => transform.position = new Vector3(pos.x, transform.position.y, pos.z);

}
