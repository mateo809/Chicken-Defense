using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public EnemyType enemyType;  

    private int currentHealth;

    private void Start()
    {
        if (enemyType != null)
        {
            currentHealth = enemyType.health;
            transform.name = enemyType.enemyName;  

        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject); 
    }
}
