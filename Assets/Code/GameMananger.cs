using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMananger : MonoBehaviour
{

    public static GameMananger instance;

    public Player player;
    public fallingPf fallingPf;

    public GameObject[] stages;
    public GameObject[] spawnPoint;

    public TMP_Text coinText;

    public GameObject EndCanvas;

    public int coinCount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        coinCount = 0;

    }

    public void MoveStage(int n)
    {
        if (n >= stages.Length)
        {
            OpenClear();
            return;
        }
        stages[n - 1].SetActive(false);

        stages[n].SetActive(true);
        
        SpawnPlayer(n);
    }

    public void SpawnPlayer(int n)
    {
        player.transform.position = spawnPoint[n].transform.position;
        foreach (Transform child in stages[n].transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void Addcoin()
    {
        coinCount++;

        coinText.text = "coin : " + coinCount;
    }

    public void OpenClear()
    {
        EndCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    public void GoTitle()
    {
        SceneManager.LoadScene("Title");
        Time.timeScale = 1f;
    }

}
