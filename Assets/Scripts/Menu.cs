using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static Menu instance;
    //[SerializeField] private GameObject _fade;
    

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    public void LanceGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuiGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Select Level");
    }

    public void DestroyFade()
    {
        gameObject.SetActive(false);
    }

    public void LaunchSelectLevel()
    {
        StartCoroutine(AnimSelectLevel());
    }

    private IEnumerator AnimSelectLevel()
    {
        gameObject.SetActive (true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Select Level");
    }

    public void LaunchMainmenu()
    {
        StartCoroutine(AnimMainMenu());
    }

    private IEnumerator AnimMainMenu()
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Main Menu");
    }
}
