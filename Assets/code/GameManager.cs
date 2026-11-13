using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player player;

    public GameObject[] stages;
    public GameObject[] spawnPoints;
    public TMP_Text coinText;

    public GameObject endCan;

    public int coinCount=0;

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
    }

    public void MoveStage(int n)
    {
        if (n >= stages.Length)
        {
            OpenClear();
            return;
        }
        stages[n-1].SetActive(false);
        stages[n].SetActive(true);
        SpawnPlayer(n);
    }

    public void Addcoin()
    {
        coinCount++;

        coinText.text = "Coin : " + coinCount;
    }

    public void SpawnPlayer(int n)
    {
        player.transform.position = spawnPoints[n].transform.position;
    }


    public void OpenClear()
    {

        endCan.SetActive(true);
        Time.timeScale = 0f;
    }

    public void GoTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("title");
    }
}
