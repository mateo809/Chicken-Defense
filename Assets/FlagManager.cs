using UnityEngine;
using UnityEngine.SceneManagement;

public class FlagManager : MonoBehaviour
{
    [Header("Scene Management")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private GameObject _nextFlag;
    [SerializeField] private GameObject _actualFlag;
    [SerializeField] private string levelKey = "LevelStars";
    [SerializeField] private int requiredStarsForNextFlag = 2;
    [SerializeField] private GameObject _path;
    [SerializeField] private GameObject _arrow;
    [SerializeField] private GameObject _panelReset;

    private void Awake()
    {
        Time.timeScale = 1.0f;
        if (!PlayerPrefs.HasKey("FirstLaunch"))
        {
            Debug.Log("Première exécution : Réinitialisation des PlayerPrefs.");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("FirstLaunch", 1);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Le jeu a déjà été lancé auparavant.");
        }
    }

    private void Start()
    {
        int starsEarned = PlayerPrefs.GetInt(levelKey, 0);
        Debug.Log($"Étoiles récupérées pour la clé {levelKey} : {starsEarned}");
        if (_nextFlag != null)
        {
            if (starsEarned >= requiredStarsForNextFlag)
            {
                _path.gameObject.SetActive(true);
                _nextFlag.gameObject.SetActive(true);
                _actualFlag.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                _actualFlag.GetComponent<BoxCollider>().enabled = false;
                _arrow.gameObject.SetActive(false);
                
            }
            else
            {
                _nextFlag.SetActive(false);
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
                if (hit.collider.gameObject)
                {
                    LoadScene1();
                }
                if (hit.collider.gameObject.CompareTag("Flag1"))
                {
                    SceneManager.LoadScene("Game1");;
                }
            }
        }

        if (_panelReset != null)
        {
            if (_panelReset.activeSelf)
            {
                _actualFlag.GetComponent <BoxCollider>().enabled = false;
                if (_nextFlag.activeSelf)
                {
                    _nextFlag.GetComponent<BoxCollider>().enabled = false;
                }
            }
            else
            {
                _actualFlag.GetComponent<BoxCollider>().enabled = true;
                if (_nextFlag.activeSelf)
                {
                    _nextFlag.GetComponent<BoxCollider>().enabled = true;
                }
            }
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
    }
}
