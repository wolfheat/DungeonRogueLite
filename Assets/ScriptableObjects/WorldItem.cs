using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ItemData data;
    public ItemData Data => data;

    public void SetData(ItemData newData)
    {
        data = newData;
        spriteRenderer.sprite = data.Picture;
    }

    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        if (data != null)
            SetData(data);
    }
    //  
    //private void Update()
    //{
    //    // Make the item face the camera
    //    Vector3 directionToCamera = cameraTransform.position - transform.position;
    //
    //    // Keep the object upright (ignore Y axis rotation)
    //    //directionToCamera.y = 0;
    //
    //    // Apply rotation to face the camera
    //    transform.forward = directionToCamera;
    //}

}

