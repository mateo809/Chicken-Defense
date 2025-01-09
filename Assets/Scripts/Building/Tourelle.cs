using UnityEngine;

public class Tourelle : MonoBehaviour
{

    private Transform target;
    public Transform partToRotate;

    private Enemy enemy;
    public BuildingScriptableObject tourelle;  
    public BuildingComponent BuildingComponent; 

    private BuildingScriptableObject tourelleCopy;

    public GameObject projectilePrefab;
    //public GameObject ParticuleShoot;
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
            tourelleCopy = Instantiate(tourelle); 
            InitializeBuildingStats(tourelleCopy); 
        }
        else
        {
            Debug.LogWarning("La tourelle n'est pas assignée dans l'inspecteur !");
        }

        InvokeRepeating("UpdateTarget", 0f, 0.25f);
    }
    public void InitializeBuildingStats(BuildingScriptableObject data)
    {
        if (BuildingComponent != null)
        {
            BuildingComponent.InitializeStats(data); 
            attackSpeed = BuildingComponent.Attackspeed;
            Range = BuildingComponent.Range;
        }
        else
        {
            Debug.LogError("Le BuildingComponent est nul !");
        }
    }

    private void Update()
    {
        attackSpeed = BuildingComponent.Attackspeed;
        Range = BuildingComponent.Range;
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
            if (tourelleCopy != null && enemy != null)
            {
                Shoot();
                attackCooldown = attackSpeed;
                int damage = tourelleCopy.Damage;
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
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
       // GameObject particule = Instantiate(ParticuleShoot, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Seek(target);
            if (tourelleCopy != null)
            {
               // Destroy(particule, 1f);
                projectile.damage = tourelleCopy.Damage;
            }
        }
    }
}
