using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public EnemyTypeScriptableObject enemyType;

    [SerializeField] private float _speed;
    private Enemy enemy;

    private Transform _target;
    private int _waypointindex = 0;

    [SerializeField] private GameObject objectToDestroy;

    void Start()
    {
        if (enemyType != null)
        {
            _speed = enemyType.speed;
        }
        _target = Waypoint.points[0];
    }

    void Update()
    {
        if (enemyType != null && enemyType.enemyName == "Cochon")
        {
            enemy = GetComponent<Enemy>();

            if (enemy != null && enemy.currentHealth <= enemyType.health / 2)
            {
                _speed = enemyType.speed * 1.5f;
                if (objectToDestroy != null)
                {
                    Destroy(objectToDestroy);
                    objectToDestroy = null;
                }
            }
        }

        Vector3 dir = _target.position - transform.position;
        transform.Translate(dir.normalized * _speed * Time.deltaTime, Space.World);

        if (dir != Vector3.zero) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); 
        }

        if (Vector3.Distance(transform.position, _target.position) <= 0.5f)
        {
            NextWaypoint();
        }
    }

    public void NextWaypoint()
    {
        if (_waypointindex >= Waypoint.points.Length - 1)
        {
            GameManager.instance.enemiesRemaining--;
            Destroy(gameObject);
            return;
        }

        _waypointindex++;
        _target = Waypoint.points[_waypointindex];
    }
}
