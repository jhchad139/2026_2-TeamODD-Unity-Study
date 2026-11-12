using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public GameObject[] stages;
    public GameObject[] spawnPoint;
    public TMP_Text coinText;
    public int coinCount=0;
    public GameObject endCanvas;
    private void Awake()
    {
        if (instance == null)
        {
         instance = this;
        }
        else
            Destroy(gameObject);
    }

    public void MoveStage(int n)
    {
            if (n >= stages.Length)
            {
                OpenClear();
                return;
            }
            if (n<stages.Length && n>0)
            {
                stages[n-1].SetActive(false);
                stages[n].SetActive(true);
                SpawnPlayer(n);
            }
    }

    public void SpawnPlayer(int n)
    {
        player.transform.position = spawnPoint[n].transform.position;
        Fallingplatform.instance.respawn();
    }
    public void addcoin()
    {
        coinCount++;
        coinText.text = "coin : " + coinCount;
    }
    public void OpenClear()
    {
        endCanvas.SetActive(true);
        Time.timeScale = 0f;
    }
    public void GoTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }
}
