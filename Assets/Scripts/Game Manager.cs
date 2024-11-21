using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private EnemyTypeScriptableObject[] enemyTypes;  

    [SerializeField] private Transform _spawner;
    [SerializeField] private TextMeshProUGUI _wave;
    [SerializeField] private TextMeshProUGUI _coins;
    [SerializeField] private TextMeshProUGUI _timer;

    public float TimeBetweenWaves = 0f;
    private float _countdown;

    [SerializeField] private float spawnDelay = 0.5f;

    public int Coins = 0;
    public int WaveNumber = 0;
    public int enemiesRemaining = 0;

    private bool waveInProgress = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        _countdown = TimeBetweenWaves;
        UpdateUI();
    }

    void Update()
    {
        if (!waveInProgress && enemiesRemaining <= 0)
        {
            _countdown -= Time.deltaTime;

            if (_countdown <= 0)
            {
                StartCoroutine(SpawnWave());
            }
        }
        UpdateUI();
        Debug.Log(enemiesRemaining);
    }

    public IEnumerator SpawnWave()
    {
        waveInProgress = true;
        WaveNumber++;
        _countdown = TimeBetweenWaves;

        int enemiesToSpawn = WaveNumber * 2;
        enemiesRemaining = enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }

        waveInProgress = false;
    }

    public void SpawnEnemy()
    {
        if (enemyTypes.Length > 0)
        {
            int randomIndex = Random.Range(0, enemyTypes.Length);
            EnemyTypeScriptableObject chosenType = enemyTypes[randomIndex];
            GameObject spawnedEnemy = Instantiate(chosenType.enemyPrefab, _spawner.position, _spawner.rotation);
            Enemy enemyScript = spawnedEnemy.GetComponent<Enemy>();
            enemyScript.enemyType = chosenType;
        }
    }

    private void UpdateUI()
    {
        _coins.text = "Coins : " + Coins.ToString();
        _wave.text = "Wave : " + WaveNumber.ToString();
        if (waveInProgress || enemiesRemaining > 0)
        {
            _timer.text = "Timer : In Progress";
        }
        else
        {
            _timer.text = "Timer : " + Mathf.Ceil(_countdown).ToString();
        }
    }
}
