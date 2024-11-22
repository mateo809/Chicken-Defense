using UnityEngine;

public class MorterSystem: MonoBehaviour
{
    [Header("Mortar Settings")]
    public Transform target;                // Cible du mortier
    public GameObject projectilePrefab;     // Préfabriqué du projectile
    public Transform firePoint;             // Point de tir
    public float firingInterval = 2f;       // Intervalle entre les tirs
    public float rotationSpeed = 5f;        // Vitesse d'orientation
    public float launchForceMultiplier = 1f; // Multiplicateur de force de tir

    private float elapsedTime;

    void Update()
    {
        if (target == null) return;

        elapsedTime += Time.deltaTime;

        // Orientation vers la cible
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Tirer si prêt
        if (elapsedTime >= firingInterval)
        {
            elapsedTime = 0;
            FireProjectile();
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null || target == null) return;

        // Calcul balistique
        Vector3 targetPosition = target.position;
        Vector3 firePointPosition = firePoint.position;

        float gravity = Physics.gravity.y;
        float heightDifference = targetPosition.y - firePointPosition.y;
        Vector3 flatTarget = new Vector3(targetPosition.x, firePointPosition.y, targetPosition.z);
        float distance = Vector3.Distance(flatTarget, firePointPosition);

        float launchAngle = Mathf.Deg2Rad * 45; // Angle optimal pour le tir
        float initialVelocity = Mathf.Sqrt(-gravity * distance * distance / (2 * (heightDifference - Mathf.Tan(launchAngle) * distance)));

        // Calcul de la direction
        Vector3 velocity = (flatTarget - firePointPosition).normalized * initialVelocity;
        velocity.y = initialVelocity * Mathf.Sin(launchAngle);

        // Instanciation du projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = velocity * launchForceMultiplier;
        }
    }
}
