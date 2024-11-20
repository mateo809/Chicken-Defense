using UnityEngine;

public class Tourelle : MonoBehaviour
{
    private Transform target;
    public Transform partToRotate;
    public float Range = 15f;
    public float TurnSpeed = 20f;

    public string enemyTag = "Enemy";

    private void Start()
    {
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
        partToRotate.rotation = Quaternion.Euler(0f,rotation.y,0f);
    }

    void UpdateTarget()
    {
        GameObject[] ennemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in ennemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if(distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= Range) 
        { 
            target = nearestEnemy.transform;
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
}
