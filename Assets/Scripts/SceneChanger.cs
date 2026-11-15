using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public static void EndGame(){
        SceneManager.LoadScene("End");
    }
}