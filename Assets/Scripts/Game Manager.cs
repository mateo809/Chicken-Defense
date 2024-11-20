using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform _spawner;
    [SerializeField] TextMeshProUGUI _wave;
    [SerializeField] TextMeshProUGUI _coins;
    [SerializeField] TextMeshProUGUI _timer;

    public float TimeBetweenWaves = 10f; 

    private float _countdown;

    [SerializeField] private float spawnDelay = 0.5f;

    public int Coins = 0;
    public int WaveNumber = 0;
    private int enemiesRemaining = 0;

    private bool waveInProgress = false;

    public List<GameObject> EnemyPrefab = new List<GameObject>();

    void Start()
    {
        _countdown = TimeBetweenWaves;
        UpdateUI();
    }

    void Update()
    {
        if (!waveInProgress)
        {
            _countdown -= Time.deltaTime;
            if (_countdown <= 0 && enemiesRemaining <= 0)
            {
                StartCoroutine(SpawnWave());
            }
        }

        UpdateUI();
    }

    public IEnumerator SpawnWave()
    {
        waveInProgress = true; 
        WaveNumber++;
        _countdown = TimeBetweenWaves; 

        int enemiesToSpawn = WaveNumber + 3; 
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
        if (EnemyPrefab.Count > 0)
        {
            int indexSpawn = WaveNumber;
            GameObject randomEnemy = EnemyPrefab[indexSpawn];
            GameObject spawnedEnemy = Instantiate(randomEnemy, _spawner.position, _spawner.rotation);
            spawnedEnemy.transform.parent = _spawner;
            enemiesRemaining++;
        }
    }

    public void HandleEnemyDeath()
    {
        enemiesRemaining--;
        if (enemiesRemaining <= 0 && waveInProgress)
        {
            waveInProgress = false;
            _countdown = TimeBetweenWaves; 
        }
    }

    private void UpdateUI()
    {
        _coins.text = "Coins : " + Coins.ToString();
        _wave.text = "Wave : " + WaveNumber.ToString();
        _timer.text = "Timer : " + (waveInProgress ? "In Progress" : Mathf.Ceil(_countdown).ToString());
    }
}
