using UnityEngine;

public class HealthBarRotation : MonoBehaviour
{
    [Header("Camera & Rotation")]
    public Transform target; 
    public Camera mainCamera; 

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; 
        }
    }

    private void Update()
    {
        Vector3 directionToCamera = mainCamera.transform.position - target.position;
        directionToCamera.y = 0; 
        transform.LookAt(transform.position + directionToCamera); 
    }
}
