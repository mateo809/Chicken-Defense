using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public EnemyTypeScriptableObject enemyType;

    private Tourelle tourelle;

    public int currentHealth;

    private void Start()
    {
        if (enemyType != null)
        {
            currentHealth = enemyType.health;
            transform.name = enemyType.enemyName;  
        }
    }
    public void SetTourelle(Tourelle newTourelle)
    {
        tourelle = newTourelle;  
    }

    public void TakeDamage(int damage)
    {
        if (tourelle != null)
        {
            BuildingScriptableObject buildingScriptable = tourelle.GetComponent<Tourelle>().tourelle;
            if (buildingScriptable != null)
            {
                damage = buildingScriptable.Damage;
            }
        }
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.instance.Score += 5;
        GameManager.instance.Coins += enemyType.Coins;
        GameManager.instance.enemiesRemaining--;
        Destroy(gameObject); 
    }
}
