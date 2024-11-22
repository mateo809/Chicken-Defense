using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void LanceGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuiGame()
    {
        Application.Quit();
    }
}
