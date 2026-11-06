using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenechanger : MonoBehaviour
{
    

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}
