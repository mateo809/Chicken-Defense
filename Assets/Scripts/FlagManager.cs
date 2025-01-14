using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlagManager : MonoBehaviour
{
    public static FlagManager instance;
    [Header("Scene Management")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private GameObject _nextFlag;
    [SerializeField] private GameObject _actualFlag;
    [SerializeField] private GameObject _finalFlag;
    [SerializeField] private string levelKey = "LevelStars";
    [SerializeField] private int requiredStarsForNextFlag = 2;
    [SerializeField] private GameObject _path;
    [SerializeField] private GameObject _finalpath;
    [SerializeField] private GameObject _arrow;
    [SerializeField] private GameObject _newtFlagArrow;
    [SerializeField] private GameObject _panelReset;
    [SerializeField] private Animator _animation;
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _starActualFlag;
    [SerializeField] private GameObject _secondStar;
    [SerializeField] private GameObject _levelEffect;

    [SerializeField] private TextMeshProUGUI _starText;

    private void Awake()
    {
        Time.timeScale = 1.0f;
        if (!PlayerPrefs.HasKey("FirstLaunch"))
        {
            Debug.Log("Première exécution : Réinitialisation des PlayerPrefs.");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("FirstLaunch", 1);
            PlayerPrefs.SetInt("TotalStars", 0);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Le jeu a déjà été lancé auparavant.");
        }


        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        int totalStars = PlayerPrefs.GetInt("TotalStars", 0);
        Debug.Log($"Total des étoiles récupérées : {totalStars}");

        if (_nextFlag != null)
        {
            if (totalStars >= requiredStarsForNextFlag)
            {
                _starActualFlag.gameObject.SetActive(true);
                _path.gameObject.SetActive(true);
                _actualFlag.gameObject.SetActive(true);
                _nextFlag.gameObject.SetActive(true);
                _actualFlag.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                _actualFlag.GetComponent<BoxCollider>().enabled = false;
                _nextFlag.GetComponent<BoxCollider>().enabled = true;
                _arrow.gameObject.SetActive(false);
            }
            else
            {
                _nextFlag.SetActive(false);
            }
            if (totalStars >= 4)
            {
                _newtFlagArrow.gameObject.SetActive(false);
                _secondStar.gameObject.SetActive(true);
                _finalpath.gameObject.SetActive(true);
                _finalFlag.gameObject.SetActive(true);
                _secondStar.gameObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.CompareTag("Flag"))
                {
                    StartCoroutine(FadeScene());
                }
                if (hit.collider.gameObject.CompareTag("Flag1"))
                {
                    StartCoroutine(FadeSecondScene());
                }
                if (hit.collider.gameObject.CompareTag("Flag2"))
                {
                    StartCoroutine(FadeFinalScene());  
                }
            }
        }

        if (_panelReset != null)
        {
            if (_panelReset.activeSelf)
            {
                _actualFlag.GetComponent <BoxCollider>().enabled = false;
                _nextFlag.GetComponent<BoxCollider>().enabled = false;
            }
            else
            {
                _actualFlag.GetComponent<BoxCollider>().enabled = true;
                _nextFlag.GetComponent<BoxCollider>().enabled = true;
            }
        }

        int totalStars = PlayerPrefs.GetInt("TotalStars", 0);
        _starText.text = totalStars.ToString() + "/ 6";

        if(totalStars >= 6)
        {
            totalStars = 6;
        }

        if (totalStars > 2 && totalStars <= 3)
        {
            _levelEffect.SetActive(true); 
        }
        else
        {
            _levelEffect.SetActive(false); 
        }
    }

    public void LoadScene1()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Aucune scène définie pour ce drapeau !");
        }
    }

    public void RestFlag()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("FirstLaunch", 0);
        PlayerPrefs.Save();
        _nextFlag.SetActive(false);
        _finalFlag.SetActive(false);
        _path.SetActive(false);
        _finalpath.SetActive(false);
    }

    private IEnumerator FadeScene()
    {
        _panel.gameObject.SetActive(true);
        _animation.SetBool("Fade",true);
        yield return new WaitForSeconds(1f);
        _panel.gameObject.SetActive(true);
        LoadScene1();
    }

    private IEnumerator FadeSecondScene()
    {
        _panel.gameObject.SetActive(true);
        _animation.SetBool("Fade", true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Game 1");
    }

    private IEnumerator FadeFinalScene()
    {
        _panel.gameObject.SetActive(true);
        _animation.SetBool("Fade", true);
        yield return new WaitForSeconds(1f);
        _panel.gameObject.SetActive(true);
        SceneManager.LoadScene("Final");
    }
}
