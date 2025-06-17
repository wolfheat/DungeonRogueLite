using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class WorldDamage : MonoBehaviour
{
    [SerializeField] private TextMeshPro damageText;
    [SerializeField] private float speed;
    [SerializeField] private float life;
    
    public void StartText(int amt)
    {
        SizeText(amt);

        damageText.text = amt.ToString();
        FacePlayer();
        StartCoroutine(AnimateText());
    }

    private void SizeText(int amt)
    {
        float scaling = amt switch
        {
            < 5 => 0.4f,
            < 10 => 0.6f,
            < 20 => 0.8f,
            < 40 => 1.2f,
            _ => 1.5f // fallback/default case
        };

        transform.localScale = Vector3.one*scaling;
    }

    private void FacePlayer()
    {
        // Make the item face the camera
        Vector3 directionToCamera = -CenterOverPlayer.Instance.transform.forward;
        //Vector3 directionToCamera = Camera.main.transform.position - transform.position;

        //directionToCamera.x = 0;

        // Keep the object upright (ignore Y axis rotation)
        //directionToCamera.y = 0;

        // Apply rotation to face the camera
        transform.forward = -directionToCamera;
    }

    private IEnumerator AnimateText()
    {
        // face player

        // Move text uppwards and remove
        while(life > 0) {
            transform.position += Vector3.up * speed * Time.deltaTime;
            life -= Time.deltaTime;
            yield return null;
        }

        // Life is over
        Destroy(gameObject);
    }
}

