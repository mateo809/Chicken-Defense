using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlagManager : MonoBehaviour
{
    [Header("Scene Management")]
    [SerializeField] private string sceneToLoad; 
    [SerializeField] private GameObject nextFlag; 
    [SerializeField] private string levelKey = "LevelStars"; 
    [SerializeField] private int requiredStarsForNextFlag = 2; 

    private void Start()
    {
        ResetFlags();

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
                    LoadScene();
                }
            }
        }
        int starsEarned = PlayerPrefs.GetInt(levelKey, 0);
        Debug.Log($"Étoiles récupérées pour la clé {levelKey} : {starsEarned}");
        if (nextFlag != null)
        {
            if (starsEarned >= requiredStarsForNextFlag)
            {
                nextFlag.SetActive(true);
                Debug.Log("Drapeau suivant activé !");
            }
            else
            {
                nextFlag.SetActive(false);
                Debug.Log("Drapeau suivant désactivé (pas assez d'étoiles) !");
            }
        }
    }

    public void LoadScene()
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

    private void ResetFlags()
    {
        PlayerPrefs.SetInt("LevelStars", 0);
        PlayerPrefs.Save();
    }
}
