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
    [SerializeField] private GameObject _hitEffectPrefab;
    [SerializeField] private GameObject _hitEffectTarget;
    [SerializeField] private GameObject _target;

    public GameObject objectToRotate;

    private float _nextAttackTime = 0f;
    private bool _isAttacking = false;
    private Quaternion _originalRotation;
    private float _attackDuration = 0.5f;

    public BoxCollider boxCollider;

    private void Awake()
    {
        if (Instance != null)
        { 
            Instance = this;      
        }

    }

    private void Start()
    {
        if (HammerScriptable != null)
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
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(AttackRadius * 2, 8f, AttackRadius * 2);
        }
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
            EnableCollider();
            if (_hitEffectPrefab != null)
            {
                Instantiate(_hitEffectPrefab, _hitEffectTarget.transform.position, Quaternion.identity);
            }
            if (objectToRotate != null)
            {
                StartCoroutine(PerformHammerStrike());
            }
        }
    }

    private void EnableCollider()
    {
        if (boxCollider != null && !boxCollider.enabled)
        {
            boxCollider.enabled = true; 
        }
    }

    private IEnumerator PerformHammerStrike()
    {
        Destroy(_target);
        float elapsedTime = 0f;
        Quaternion targetRotation = _originalRotation * Quaternion.Euler(-90f, 0f, 0f);

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

        // Désactiver le collider après l'attaque
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

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
