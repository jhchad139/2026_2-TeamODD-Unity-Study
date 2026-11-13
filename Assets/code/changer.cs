using UnityEngine;
using UnityEngine.SceneManagement;

public class changer : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("game");
    }
}
