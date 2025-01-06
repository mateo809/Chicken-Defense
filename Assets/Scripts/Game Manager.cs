using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private GameObject _warningPanel;

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

    [SerializeField] private GameObject Start1;
    [SerializeField] private GameObject Start2;
    [SerializeField] private GameObject Start3;

    [SerializeField] private GameObject Panel;

    public Transform LifeSlider;

    private int starsEarned = 0;

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
        Win();
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
            StartCoroutine(WarningPanel());
            yield return new WaitForSeconds(2f);
        }

        int enemiesToSpawn = WaveNumber * 5;
        enemiesRemaining = enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            float ik = ((int)(WaveNumber / 5)) * 0.25f;
            SpawnEnemy(ik);
            yield return new WaitForSeconds(spawnDelay);
        }

        waveInProgress = false;
    }


    private IEnumerator WarningPanel()
    {
        _warningPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _warningPanel.gameObject.SetActive(false);
    }

    public void SpawnEnemy(float IncrementStat)
    {      
        if (enemyTypes.Length > 0)
        {
            int randomIndex = Random.Range(0, enemyTypes.Length);
            EnemyTypeScriptableObject chosenType = enemyTypes[randomIndex];
            GameObject spawnedEnemy = Instantiate(chosenType.enemyPrefab, _spawner.position, _spawner.rotation);
            EnemyComponent enemyComponent = spawnedEnemy.GetComponent<EnemyComponent>();
            if (enemyComponent != null)
            {
                enemyComponent.InitializeStats(chosenType);
                IncreaseEnemyStats(IncrementStat, enemyComponent);
            }

            Enemy enemyScript = spawnedEnemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.enemyComponent = enemyComponent;
            }
        }
    }
    private void UpdateUI()
    {
        if (enemiesRemaining <= 0)
        {
            enemiesRemaining = 0;
        }
        if (Coins > 0)
        {
            _coins.text = Coins.ToString();
        }

        _wave.text = "Wave : " + WaveNumber.ToString() + "/10";
        _enemyRemaining.text = enemiesRemaining.ToString();
        _scorePlayer.text = Score.ToString();

        if (waveInProgress || enemiesRemaining > 0)
        {
            _waveFeedback.gameObject.SetActive(false);
            _timer.text = "In Progress";
        }
        else
        {
            if (WaveNumber == 5)
                StartCoroutine(WarningPanel());
            _waveFeedback.gameObject.SetActive(true);
            _timer.text = Mathf.Ceil(_countdown).ToString();
        }
    }


    public Material GetRangePreviewMaterial(bool isValidPlacement)
    {
        return isValidPlacement ? GreenPreview : RedPreview;
    }

    private void IncreaseEnemyStats(float percentage, EnemyComponent enemyComponent)
    {
        enemyComponent.Health +=(enemyComponent.Health * (1 + percentage));
        enemyComponent.Damage = (enemyComponent.Damage * (1 + percentage));
        enemyComponent.Speed *= (1 + percentage);

        Debug.Log("Active enemy stats increased by " + (percentage * 100) + "% for wave " + WaveNumber);
    }


    public void SpawnAI()
    {
        if (IAObjectPrefab != null)
        {
            Instantiate(IAObjectPrefab, _spawner.position, Quaternion.identity);
            Debug.Log("IA spawn!");
        }
    }

    public void Win()
    {
        if (WaveNumber == 2 && !waveInProgress && enemiesRemaining == 0)
        {
            Panel.gameObject.SetActive(true);
            Time.timeScale = 0f;
            if (LifeSlider.GetComponent<Slider>().value <= 100)
            {
                Start1.gameObject.SetActive(true);
                starsEarned = 1;
            }
            if (LifeSlider.GetComponent<Slider>().value <= 200)
            {
                Start1.gameObject.SetActive(true);
                Start2.gameObject.SetActive(true);
                starsEarned = 2;
            }
            if (LifeSlider.GetComponent<Slider>().value >= 250)
            {
                Start1.gameObject.SetActive(true);
                Start2.gameObject.SetActive(true);
                Start3.gameObject.SetActive(true);
                starsEarned = 3;
            }
            PlayerPrefs.SetInt("LevelStars", starsEarned);
            PlayerPrefs.Save();
        }
    }
}
