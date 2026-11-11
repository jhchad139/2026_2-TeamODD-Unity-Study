using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    // 플레이어가 리스폰될 때 발생하는 이벤트 (FallingPf 등이 구독)
    public static event System.Action OnPlayerRespawn;

    public Player player;
    public GameObject[] stages;
    public GameObject[] spawnPoint;
    public GameObject EndCanvas;
    public TMP_Text coinText;

    public int coinCount;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);

        coinCount = 0;
    }

    public void MoveStage(int n)  // n = 현재 스테이지
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

    public void SpawnPlayer(int n)  // n = 현재 스테이지
    {
        player.transform.position = spawnPoint[n].transform.position;

        OnPlayerRespawn?.Invoke();  // 사라졌던 발판 등 복구
    }

    public void AddCoin()
    {
        coinCount++;
        
        coinText.text = "coin : " + coinCount;
    }
    public void DecreaseCoin()
    {
        if (coinCount > 0)
            coinCount--;
        else
            coinCount = 0;

        coinText.text = "coin : " + coinCount;
    }

    public void OpenClear()
    {
        EndCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    public void GoTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }

}
