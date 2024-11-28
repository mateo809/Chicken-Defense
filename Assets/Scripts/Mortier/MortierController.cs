using UnityEngine;

public class MortarController : MonoBehaviour
{
    [Header("Mortar Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public BuildingComponent BuildingComponent;   
    public float launchForceMultiplier = 1f;
    public float detectionRadius = 20f;
    public float rotationSpeed = 5f;

    [Header("Target Settings")]
    public LayerMask targetLayer;
    private Transform currentTarget;
    private float elapsedTime;

    void Start()
    {
        if (BuildingComponent != null)
        {
            detectionRadius = BuildingComponent.Range; 
            launchForceMultiplier = BuildingComponent.Attackspeed; 
        }
    }

    void Update()
    {
        FindClosestTarget();

        elapsedTime += Time.deltaTime;

        if (currentTarget != null)
        {
            Vector3 directionToTarget = (currentTarget.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            if (elapsedTime >= BuildingComponent.Attackspeed)  
            {
                elapsedTime = 0;
                FireProjectile();
            }
        }
    }

    private void FindClosestTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);

        float closestDistance = float.MaxValue;
        Transform closestTarget = null;

        foreach (Collider hitCollider in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = hitCollider.transform;
            }
        }

        currentTarget = closestTarget;
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null || currentTarget == null) return;

        Vector3 targetPosition = currentTarget.position;
        Vector3 flatDirection = new Vector3(targetPosition.x - firePoint.position.x, 0, targetPosition.z - firePoint.position.z);
        float horizontalDistance = flatDirection.magnitude;
        float heightDifference = targetPosition.y - firePoint.position.y;
        float gravity = Mathf.Abs(Physics.gravity.y);
        float angleRad = Mathf.Deg2Rad * 45;

        float velocity = Mathf.Sqrt((gravity * horizontalDistance * horizontalDistance) /
                                    (2 * (horizontalDistance * Mathf.Tan(angleRad) - heightDifference)));

        float minForce = 0.5f;
        float maxForce = 2f;
        float normalizedDistance = Mathf.Clamp01(horizontalDistance / detectionRadius);
        float dynamicMultiplier = Mathf.Lerp(minForce, maxForce, normalizedDistance);

        Vector3 velocityVector = flatDirection.normalized * velocity * Mathf.Cos(angleRad);
        velocityVector.y = velocity * Mathf.Sin(angleRad);

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = true;
            rb.linearVelocity = velocityVector * dynamicMultiplier;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
