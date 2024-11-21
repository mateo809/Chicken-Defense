using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    public EnemyTypeScriptableObject enemyType;

    [SerializeField] private float _speed;
    Enemy enemy;

    private Transform _target;
    private int _waypointindex = 0;

    void Start()
    {
        if(enemyType != null)
        {
            _speed = enemyType.speed;
        }
        _target = Waypoint.points[0];
    }

    void Update()
    {
        Vector3 dir = _target.position - transform.position;
        transform.Translate(dir.normalized * _speed * Time.deltaTime, Space.World);

        if(Vector3.Distance(transform.position, _target.position) <= 0.5)
        {
            NextWaypoint();
        }
    }

    public void NextWaypoint()
    {
        if (_waypointindex >= Waypoint.points.Length -1)
        {
            GameManager.instance.enemiesRemaining--;
            Destroy(gameObject);
            return;
        }
        _waypointindex++;
        _target = Waypoint.points[_waypointindex];
    }
}
