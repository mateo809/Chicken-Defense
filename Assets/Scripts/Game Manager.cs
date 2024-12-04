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
    [SerializeField] private TextMeshProUGUI _enemyRemaining;
    [SerializeField] private TextMeshProUGUI _scorePlayer;

    public RectTransform _shopButton;

    public TextMeshProUGUI _InstructionRota;
    public TextMeshProUGUI _InstructionMissile;

    public GameObject BuildPanel;
    public GameObject HeroPanel;
    public GameObject Gold;
    public GameObject IAObjectPrefab;  
    public GameObject _button;
    [SerializeField] private GameObject _waveFeedback;

    public float TimeBetweenWaves = 0f;
    private float _countdown;
    public float aiSpawnInterval = 45f; 
    private float aiSpawnTimer = 0f;
    [SerializeField] private float spawnDelay = 0.5f;

    public int Coins = 0;
    public int WaveNumber = 0;
    public int enemiesRemaining = 0;
    public int Score = 0;

    public Material RedPreview;
    public Material GreenPreview;

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
                Score += 50;
                StartCoroutine(SpawnWave());
            }
        }
        aiSpawnTimer += Time.deltaTime;
        if (aiSpawnTimer >= aiSpawnInterval)
        {
            aiSpawnTimer = 0f;
            SpawnAI(); 
        }

        UpdateUI();
        Debug.Log(enemiesRemaining);
    }

    public IEnumerator SpawnWave()
    {
        waveInProgress = true;
        WaveNumber++;
        _countdown = TimeBetweenWaves;

        if (WaveNumber % 5 == 0)
        {
            IncreaseEnemyStats(0.25f);
        }

        int enemiesToSpawn = WaveNumber * 10;
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
        if(enemiesRemaining <= 0)
        {
            enemiesRemaining = 0;
        }
        _coins.text = Coins.ToString();
        _wave.text = "Wave : " + WaveNumber.ToString();
        _enemyRemaining.text = enemiesRemaining.ToString();
        _scorePlayer.text = Score.ToString();
        if (waveInProgress || enemiesRemaining > 0)
        {
            _waveFeedback.gameObject.SetActive(false);
            _timer.text = "In Progress";
        }
        else
        {
            _waveFeedback.gameObject.SetActive(true);
            _timer.text = Mathf.Ceil(_countdown).ToString();
        }
    }

    public Material GetRangePreviewMaterial(bool isValidPlacement)
    {
        return isValidPlacement ? GreenPreview : RedPreview;
    }

    private void IncreaseEnemyStats(float percentage)
    {
        foreach (var enemyType in enemyTypes)
        {
            enemyType.health = Mathf.RoundToInt(enemyType.health * (1 + percentage));
            enemyType.damage = Mathf.RoundToInt(enemyType.damage * (1 + percentage));
            enemyType.speed *= (1 + percentage);
        }
        Debug.Log("Enemy stats increased by " + (percentage * 100) + "% for wave " + WaveNumber);
    }

    public void SpawnAI()
    {
        if (IAObjectPrefab == null)
        {
            Instantiate(IAObjectPrefab, _spawner.position, Quaternion.identity);
            Debug.Log("IA spawn!");
        }
        else
        {
            return;
        }
    }
}
