using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    public Player player;

    public GameObject[] Stages;
    public GameObject[] SpawnPoint;

    public TMP_Text coinText;

    public GameObject EndCanvas;

    public int coinCount;

    private void Awake()
    {
        if(instance == null) 
        {
            instance = this;
        }
        else
            Destroy(gameObject);

        coinCount = 0;
    }

    public void MoveStage(int n)//지금 진행하고 있는 스테이지 정보
    {
        if (n >= Stages.Length)
        {
            OpenClear();
            return;
        }

        Stages[n - 1].SetActive(false);

        Stages[n].SetActive(true);
        SpawnPlayer(n);
    }

    public void Addcoin()
    {
        coinCount ++;
        coinText.text = "Coin: " + coinCount;

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

    public void SpawnPlayer(int n)
    {
        player.transform.position = SpawnPoint[n].transform.position;
        FallingPf.ResetAll();

    }
}
