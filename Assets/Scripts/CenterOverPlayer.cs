using System;
using System.Collections;
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

    public void ResetToPosition(Vector3 pos)
	{
		Debug.Log("Centering over player looking forward");
		transform.position = pos;
        transform.rotation = Quaternion.LookRotation(Vector3.forward,Vector3.up);
	}

    public void Center(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetRotation(Vector3 forward)
    {
		// Tween to new position?
		StartCoroutine(TweenToRotation(forward));

        //transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    private IEnumerator TweenToRotation(Vector3 forward)
    {
		Quaternion startRotation = transform.rotation;
		Quaternion endRotation   = Quaternion.LookRotation(forward,Vector3.up);
		float tweenTime = 0.08f;
		float timer = 0f;
		while(timer < tweenTime) {
			timer += Time.deltaTime;
			float t = timer / tweenTime;
			transform.rotation = Quaternion.Lerp(startRotation,endRotation,t);
			yield return null;
		}
		transform.rotation = endRotation;
    }

    //public void Center(Vector3 pos) => transform.position = new Vector3(pos.x, transform.position.y, pos.z);

}
