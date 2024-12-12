using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    public EnemyTypeScriptableObject enemyData;

    public string EnemyName;
    public float Speed;
    public float Health;
    public float Damage;
    public int Coins;
    public GameObject EnemyPrefab;

    public void InitializeStats(EnemyTypeScriptableObject data)
    {
        EnemyName = data.enemyName;
        Speed = data.speed;
        Health = data.health;
        Damage = data.damage;
        Coins = data.Coins;
        EnemyPrefab = data.enemyPrefab;

        Debug.Log($"EnemyInstanceData initialized: {EnemyName}, Speed: {Speed}, Health: {Health}, Damage: {Damage}, Coins: {Coins}");
    }
}
