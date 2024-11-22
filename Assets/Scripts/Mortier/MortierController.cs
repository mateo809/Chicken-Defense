using UnityEngine;

public class MortarController : MonoBehaviour
{
    [Header("Mortar Settings")]
    public GameObject projectilePrefab;      
    public Transform firePoint;             
    public float firingInterval = 2f;       
    public float rotationSpeed = 5f;        
    public float launchForceMultiplier = 1f; 
    public float detectionRadius = 20f;      

    [Header("Target Settings")]
    public LayerMask targetLayer;           
    private Transform currentTarget;        
    private float elapsedTime;

    void Update()
    {
        FindClosestTarget();

        elapsedTime += Time.deltaTime;

        if (currentTarget != null)
        {
            Vector3 directionToTarget = (currentTarget.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            if (elapsedTime >= firingInterval)
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

        // Calculate the direction to the target
        Vector3 directionToTarget = targetPosition - firePoint.position;

        // Estimate the time to reach the target based on distance and gravity
        float distance = directionToTarget.magnitude;
        float gravity = Physics.gravity.y;
        float launchAngle = Mathf.Deg2Rad * 45; // Keep it fixed or dynamically calculate

        // Basic physics to compute the necessary velocity
        float initialVelocity = Mathf.Sqrt(-gravity * distance * distance / (2 * (directionToTarget.y - Mathf.Tan(launchAngle) * distance)));

        // Normalize direction and apply velocity
        Vector3 velocity = directionToTarget.normalized * initialVelocity;
        velocity.y = initialVelocity * Mathf.Sin(launchAngle); // Adjust the vertical component to counteract gravity

        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = true;  // Enable gravity for the projectile

            // Apply the calculated velocity
            rb.linearVelocity = velocity * launchForceMultiplier;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
