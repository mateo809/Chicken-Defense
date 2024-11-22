using UnityEngine;

public class Tourelle : MonoBehaviour
{
    private Transform target;
    public Transform partToRotate;

    private Enemy enemy;
    public BuildingScriptableObject tourelle;

    public GameObject projectilePrefab; 
    public Transform firePoint; 

    public string enemyTag = "Enemy"; 

    public float Range = 15f; 
    public float TurnSpeed = 20f; 
    private float attackCooldown = 0f; 
    private float attackSpeed; 

    public object buildingScriptable { get; private set; }

    private void Start()
    {
        if (tourelle != null)
        {
            attackSpeed = tourelle.Attackspeed;
            Range = tourelle.Range;
        }
        else
        {
            Debug.LogWarning("La tourelle n'est pas assignée dans l'inspecteur !");
        }
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * TurnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        if (attackCooldown <= 0f)
        {
            if (tourelle != null && enemy != null)
            {
                Shoot();
                attackCooldown = attackSpeed; 
                int damage = tourelle.Damage;
                enemy.TakeDamage(damage);
            }
        }
        else
        {
            attackCooldown -= Time.deltaTime;  
        }
    }

    void UpdateTarget()
    {
        GameObject[] ennemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemyObj in ennemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemyObj.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemyObj;
            }
        }

        if (nearestEnemy != null && shortestDistance <= Range)
        {
            target = nearestEnemy.transform;
            enemy = nearestEnemy.GetComponent<Enemy>();
            enemy.SetTourelle(this);  
        }
        else
        {
            target = null;  
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Range);
    }

    void Shoot()
    {
        if (target == null || projectilePrefab == null || firePoint == null)
        {
            return;
        }

        // Instancier le projectile et l'orienter vers la cible
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Seek(target);  
            if (tourelle != null)
            {
                projectile.damage = tourelle.Damage;  
            }
        }
    }
}
