using UnityEngine;

public class ProjectileSystem : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float explosionRadius = 2f;  // Rayon d'explosion
    public float lifetime = 5f;         // Durée de vie avant destruction

    private void Start()
    {
        Destroy(gameObject, lifetime); // Détruire après un certain temps
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Appliquer des effets de collision ici (dégâts, destruction, etc.)
        Explode();
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var collider in colliders)
        {
            // Exemple : appliquer des forces sur des objets touchés
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(500f, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject); // Détruire le projectile après l'explosion
    }
}
