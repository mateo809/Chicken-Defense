using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Enemy Settings")]
    [SerializeField] private EnemyTypeScriptableObject[] enemyTypes;
    [SerializeField] private Transform _spawner;
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _wave;
    [SerializeField] private TextMeshProUGUI _coins;
    [SerializeField] private TextMeshProUGUI _timer;
    [SerializeField] private TextMeshProUGUI _enemyRemaining;
    [SerializeField] private TextMeshProUGUI _scorePlayer;

    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI SecondFeedbackText;
    public TextMeshProUGUI _InstructionRota;
    public TextMeshProUGUI _InstructionMissile;
    [Header("UI Panels")]
    [SerializeField] private GameObject Start1;
    [SerializeField] private GameObject Start2;
    [SerializeField] private GameObject Start3;
    [SerializeField] private GameObject Panel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _buttonReady;
    [Header("Gameplay Settings")]
    [SerializeField] private Button speedToggleButton; 
    public RectTransform _shopButton;
    public GameObject BuildPanel;
    public GameObject HeroPanel;
    public GameObject Gold;
    public GameObject IAObjectPrefab;
    public GameObject _button;
    public GameObject _tutoPanel;
    public GameObject _colonel;
    public GameObject insufficientFundsIndicator;
    [SerializeField] private GameObject _waveFeedback;
    [SerializeField] private GameObject _warningPanel;

    private bool isFastMode = false;
    private bool waveInProgress = false;
    private bool gameStarted = false;
    public bool shopOpen = false;
    [Header("Wave Settings")]
    public float TimeBetweenWaves = 0f;
    private float _countdown;
    public float aiSpawnInterval = 45f;
    private float aiSpawnTimer = 0f;
    public int enemiesRemaining = 0;
    public int WaveNumber = 0;
    [Header("Player Stats")]
    public int Coins = 0;
    public int Score = 0;
    private int starsEarned = 0;
    [Header("Materials")]
    public Material RedPreview;
    public Material GreenPreview;
    [Header("Other References")]
    public Transform LifeSlider;
    public AudioSource audioSource;
    public AudioSource DestroySound;
    public AudioClip DestroyButtonClicked;
    public AudioClip buttonClickSound;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Menu.instance.PauseGame();
            _pausePanel.SetActive(true);
        }

        if (!gameStarted) return;

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

    public void StartGame() 
    {
        gameStarted = true;
        _buttonReady.gameObject.SetActive(false);
    }

    public IEnumerator SpawnWave()
    {
        waveInProgress = true;
        WaveNumber++;
        _countdown = TimeBetweenWaves;

        if (WaveNumber % 5 == 0)
        {
            StartCoroutine(WarningPanel());
            yield return new WaitForSeconds(1f);
        }

        int enemiesToSpawn = WaveNumber * 5;
        enemiesRemaining = enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            float ik = ((int)(WaveNumber / 5)) * 0.25f;
            var enemy = SpawnEnemy(ik);
            yield return new WaitForSeconds(enemy.SpawnDelay);
        }

        waveInProgress = false;
    }


    private IEnumerator WarningPanel()
    {
        _warningPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        _warningPanel.gameObject.SetActive(false);
    }

    public EnemyTypeScriptableObject SpawnEnemy(float IncrementStat)
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

            return chosenType;
        }

        return null;
    }
    public void UpdateUI()
    {
        if (enemiesRemaining <= 0)
        {
            enemiesRemaining = 0;
        }
        _coins.text = Coins.ToString();
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
        if (WaveNumber == 10 && !waveInProgress && enemiesRemaining <= 0)
        {
            Time.timeScale = 0f;
            Panel.gameObject.SetActive(true);
            waveInProgress = true;

            float lifeValue = LifeSlider.GetComponent<Slider>().value;

            if (lifeValue <= 100)
            {
                shopOpen = true;
                Start1.gameObject.SetActive(true);
                starsEarned = 1;
            }
            else if (lifeValue > 100 && lifeValue <= 200)
            {
                shopOpen = true;
                Start1.gameObject.SetActive(true);
                Start2.gameObject.SetActive(true);
                starsEarned = 2;
            }
            else if (lifeValue > 200)
            {
                shopOpen = true;
                Start1.gameObject.SetActive(true);
                Start2.gameObject.SetActive(true);
                Start3.gameObject.SetActive(true);
                starsEarned = 3;
            }

            PlayerPrefs.SetInt("LevelStars", starsEarned);
            PlayerPrefs.Save();
            UpdateTotalStars();
        }
    }



    public void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.gameObject.SetActive(true);

            Invoke(nameof(HideFeedback), 1f);
        }
    }

    private void HideFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }

    public void ShowSecondFeedback(string message)
    {
        if (SecondFeedbackText != null)
        {
            SecondFeedbackText.text = message;
            SecondFeedbackText.gameObject.SetActive(true);

            Invoke(nameof(HideSecondFeedback), 0.5f);
        }
    }

    private void HideSecondFeedback()
    {
        if (SecondFeedbackText != null)
        {
            SecondFeedbackText.gameObject.SetActive(false);
        }
    }

    public void UpdateTotalStars()
    {
        int totalStars = PlayerPrefs.GetInt("TotalStars", 0);
        totalStars += starsEarned;
        PlayerPrefs.SetInt("TotalStars", totalStars); 
        PlayerPrefs.Save(); 
        Debug.Log("TotalStars updated to: " + totalStars);
    }

    public void ToggleSpeedMode()
    {
        isFastMode = !isFastMode;

        Time.timeScale = isFastMode ? 2.0f : 1.0f; 
    }

    public void Openshop()
    {
        shopOpen = true;
    }

    public void Closeshop()
    {
        shopOpen = false;
    }


    private IEnumerator DisableAfterDelay(GameObject target, float delay)
    {
        Debug.Log("Désactivation commencée, attente de " + delay + " secondes.");
        Time.timeScale = 1.0f;
        yield return new WaitForSeconds(delay);
        Debug.Log("Désactivation de l'objet : " + target.name);
        target.SetActive(false);
    }

    public void DeleteObjectPrviewNoMoney()
    {
        StartCoroutine(DisableAfterDelay(insufficientFundsIndicator, 0.75f));
    }

}
