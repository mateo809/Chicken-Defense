using UnityEngine;

public class ProjectileMortier : MonoBehaviour
{
    public float explosionRadius = 2f;
    public float lifetime = 5f;
    public int damage;

    public BuildingComponent BuildingComponent;

    private Rigidbody rb;

    private void Start()
    {
        Destroy(gameObject, lifetime); 

        if(BuildingComponent != null)
        {
            damage = BuildingComponent.Damage;
        }
    }

    private void Update()
    {
        damage = BuildingComponent.Damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var collider in colliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject); 
    }
}
