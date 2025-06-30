using System;
using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    public void ShootArrow(ArrowData data)
    {
        StartCoroutine(Shoot(data));
    }

    private IEnumerator Shoot(ArrowData data)
    {
        Vector3 from = data.StartPosition;
        Vector3 to = data.TargetPosition;
        IBeingHitByArrow arrowTarget = data.TargetCharacter;

        transform.position = from;
        transform.rotation = Quaternion.LookRotation(to-from,Vector3.up);

        float speed = 150f;
        float totDistance = (to - from).magnitude;
        float traveled = 0;
        while (traveled < totDistance) {
            traveled += Time.deltaTime*speed;
            float t = traveled / totDistance;
            transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }
        Debug.Log("Hit Target here");

        if(arrowTarget != null)
            arrowTarget.BeingHitByArrow(data);

        ItemSpawner.Instance.RemoveArrow(this);

    }
}

