using UnityEngine;
using System.Collections;

public class Hammer : MonoBehaviour
{
    public static Hammer Instance;
    [Header("Hammer Settings")]
    public float AttackRadius = 5f;
    public float Damage = 50f;
    public float AttackCooldown = 2f;
    public BuildingScriptableObject HammerScriptable;
    public BuildingComponent BuildingComponent;
    [SerializeField] private GameObject _hitEffectPrefab;
    [SerializeField] private GameObject _hitEffectTarget;
    [SerializeField] private GameObject _target;
    [SerializeField] private GameObject _targetEffects;

    public GameObject objectToRotate;

    private float _nextAttackTime = 0f;
    private bool _isAttacking = false;
    private Quaternion _originalRotation;
    private float _attackDuration = 0.5f;

    public BoxCollider boxCollider;
    public Vector3 ColliderOffset = new Vector3(0, 5, 5f);

    private void Awake()
    {
        if (Instance != null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Initialiser les paramètres à partir de BuildingComponent si disponible
        if (BuildingComponent != null)
        {
            AttackRadius = BuildingComponent.Range; // Par exemple, si BuildingComponent définit l'attaque de rayon
            AttackCooldown = BuildingComponent.Attackspeed; // Attente entre les attaques
            Damage = BuildingComponent.Damage; // Dommages de l'attaque
        }
        else if (HammerScriptable != null) // Au cas où BuildingComponent serait null, utiliser le scriptable object
        {
            AttackRadius = HammerScriptable.Range;
            AttackCooldown = HammerScriptable.Attackspeed;
            Damage = HammerScriptable.Damage;
        }

        if (objectToRotate != null)
        {
            _originalRotation = objectToRotate.transform.rotation;
        }

        boxCollider = gameObject.GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(1, 1f, AttackRadius);
        boxCollider.center = ColliderOffset;
    }

    private void Update()
    {
        AttackRadius = BuildingComponent.Range; // Par exemple, si BuildingComponent définit l'attaque de rayon
        AttackCooldown = BuildingComponent.Attackspeed; // Attente entre les attaques
        Damage = BuildingComponent.Damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !_isAttacking && Time.time >= _nextAttackTime)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (!_isAttacking && Time.time >= _nextAttackTime)
        {
            _isAttacking = true;
            _nextAttackTime = Time.time + AttackCooldown;
            if (_hitEffectPrefab != null)
            {
                Instantiate(_hitEffectPrefab, _targetEffects.transform.position, Quaternion.identity);
            }
            if (objectToRotate != null)
            {
                StartCoroutine(PerformHammerStrike());
            }
        }
    }

    private IEnumerator PerformHammerStrike()
    {
        Destroy(_target);
        float elapsedTime = 0f;
        Quaternion targetRotation = _originalRotation * Quaternion.Euler(70f, 0f, 0f);

        while (elapsedTime < _attackDuration)
        {
            if (objectToRotate != null)
            {
                objectToRotate.transform.rotation = Quaternion.Slerp(_originalRotation, targetRotation, elapsedTime / _attackDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (objectToRotate != null)
        {
            objectToRotate.transform.rotation = targetRotation;
        }

        // Vérifier les ennemis dans le rayon de l'attaque
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, AttackRadius);
        foreach (var enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage((int)Damage);
                }
            }
        }

        yield return new WaitForSeconds(0.2f);

        if (objectToRotate != null)
        {
            objectToRotate.transform.rotation = _originalRotation;
        }

        _isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRadius);
    }
}
