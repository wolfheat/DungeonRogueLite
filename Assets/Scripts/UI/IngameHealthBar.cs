using UnityEngine;

public class IngameHealthBar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    Vector3 startScale;

    private Transform cameraTransform;

    void Start()
    {
        startScale = transform.localScale;  
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        // Make the item face the camera
        Vector3 directionToCamera = cameraTransform.position - transform.position;

        directionToCamera.x = 0;

        // Keep the object upright (ignore Y axis rotation)
        //directionToCamera.y = 0;

        // Apply rotation to face the camera
        transform.forward = directionToCamera;
    }
    public void UpdateHealthBar(int current, int max)
    {
        float percent = (float)current / max;
        if (percent == 1 || percent == 0) {
            spriteRenderer.enabled = false;
        }
        else {
            spriteRenderer.enabled = true;
            Debug.Log("Healthbar percent is "+percent);
            // LImit so almost dead is still showing healthbar
            transform.localScale = new Vector3(Mathf.Max(0.04f,percent),startScale.y,startScale.z);
        }
    }
}
