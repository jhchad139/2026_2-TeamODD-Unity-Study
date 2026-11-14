using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void GoTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }
}
