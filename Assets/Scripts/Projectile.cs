using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f; 
    public int damage = 10;  

    private Transform target;

    public void Seek(Transform _target)
    {
        if (_target == null)
        {
            Debug.LogError("Target passed to Seek is null!");
            Destroy(gameObject); 
            return;
        }
        target = _target;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); 
            return;
        }
        Vector3 direction = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }
        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target);
    }

    void HitTarget()
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject); 
    }
}
