using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Type", menuName = "Enemies/EnemyType")]
public class EnemyType : ScriptableObject
{
    public string enemyName;
    public float speed;
    public int health;
    public int damage;
    public GameObject enemyPrefab;  
}
