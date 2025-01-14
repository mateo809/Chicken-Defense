using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public EnemyComponent enemyComponent; 
    private Tourelle tourelle;
    public float currentHealth;

    [Header("Health Bar Settings")]
    public Slider healthBar;

    private void Start()
    {
        if (enemyComponent != null)
        {
            // enemyComponent.InitializeStats(enemyComponent.enemyData);

            currentHealth = enemyComponent.Health;
            transform.name = enemyComponent.EnemyName;
        }
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
            healthBar.maxValue = enemyComponent.Health;
            healthBar.value = currentHealth;
        }
    }

    public void SetTourelle(Tourelle newTourelle)
    {
        tourelle = newTourelle;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
            return;

        if (tourelle != null)
        {
            BuildingComponent buildingComponent = tourelle.GetComponent<BuildingComponent>();
            if (buildingComponent != null)
            {
                healthBar.gameObject.SetActive(true);
                damage = buildingComponent.Damage;
            }
        }

        currentHealth -= damage;

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        StartCoroutine(SpawnGold());
        GameManager.instance.Score += 5;
        GameManager.instance.Coins += enemyComponent.Coins;
        GameManager.instance.ShowSecondFeedback("+" + enemyComponent.Coins);
        GameManager.instance.enemiesRemaining--;
        Destroy(gameObject);
    }

    public IEnumerator SpawnGold()
    {
        GameObject go = Instantiate(GameManager.instance.Gold, transform.position, transform.rotation);
        Destroy(go, 2f);
        yield return null;
    }
}
