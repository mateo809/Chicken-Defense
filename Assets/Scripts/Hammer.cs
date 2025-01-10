using UnityEngine;
using System.Collections;

public class Hammer : MonoBehaviour
{
    public static Hammer Instance;

    [Header("Hammer Settings")]
    public float AttackRadius = 5f;
    public float Damage = 50f;
    public float AttackCooldown = 2f;

    [SerializeField] private GameObject _hitEffectPrefab;
    [SerializeField] private GameObject _targetEffects;
    [SerializeField] private GameObject _target;
    public GameObject objectToRotate;

    private float _nextAttackTime = 0f;
    private bool _isAttacking = false;
    private Quaternion _originalRotation;
    private float _attackDuration = 0.15f;

    private void Awake()
    {
        if (Instance != null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (objectToRotate != null)
        {
            _originalRotation = objectToRotate.transform.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && Time.time >= _nextAttackTime && !_isAttacking)
        {
            _targetEffects.GetComponent<MeshRenderer>().enabled = false;
            _isAttacking = true;
            Attack();
        }
    }

    private void Update()
    {
        _target.GetComponentInChildren<BoxCollider>().enabled = GetComponent<BoxCollider>().enabled;
    }

    private void Attack()
    {

        _nextAttackTime = Time.time + AttackCooldown;
        StartCoroutine(PerformHammerStrike());
    }

    private IEnumerator PerformHammerStrike()
    {
        Debug.Log("attack hammer");

        Quaternion targetRotation = _originalRotation * Quaternion.Euler(70f, 0f, 0f);
        float elapsedTime = 0f;
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
        if (_hitEffectPrefab != null && _target != null)
        {
            GameObject hitEffect = Instantiate(_hitEffectPrefab, _targetEffects.transform.position, Quaternion.identity);
            Destroy(hitEffect, 0.2f);
        }
        Collider[] hitEnemies = Physics.OverlapSphere(_target.transform.position, AttackRadius);
        foreach (var enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.healthBar.gameObject.SetActive(true);
                    enemy.TakeDamage((int)Damage);
                }
            }
        }
        elapsedTime = 0f;
        while (elapsedTime < _attackDuration)
        {
            if (objectToRotate != null)
            {
                objectToRotate.transform.rotation = Quaternion.Slerp(targetRotation, _originalRotation, elapsedTime / _attackDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
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
        Gizmos.DrawWireSphere(_target.transform.position, AttackRadius);
    }
}
