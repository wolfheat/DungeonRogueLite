using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EnemyActionManager : MonoBehaviour
{

	public static EnemyActionManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}
    private void OnEnable() => TickManager.TickGame += Tick;
    private void OnDisable() => TickManager.TickGame -= Tick;


    private void Tick()
    {
		Debug.Log("Handle all enemies actions");

		// Have a list of all enemies
		// Order them after distance from player

		List<EnemyController> enemies = transform.GetComponentsInChildren<EnemyController>().OrderBy(x => x.PlayerDistance).ToList();

		Debug.Log("The enemies are ordered");
		StringBuilder sb = new StringBuilder("Enemies: ");
		foreach (EnemyController enemy in enemies) {
			sb.Append(enemy.PlayerDistance+",");
		}
		Debug.Log(sb.ToString());

		Debug.Log("NOW TICK ALL ENEMIES");
		foreach (EnemyController enemy in enemies) {
			enemy.Tick();
		}
		Debug.Log("ENEMIES Ticks Completed");
		TickManager.Instance.EndEnemyTicks();
    }

}
