using UnityEngine;
using TMPro;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;
    public Player player;


    public GameObject[] stages;
    public GameObject[] spawnPoint;

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
        stages[n - 1].SetActive(false);

        stages[n].SetActive(true);
        SpawnPlayer(n);
    }

    public void SpawnPlayer(int n)  // n = 현재 스테이지
    {
        player.transform.position = spawnPoint[n].transform.position;
    }

    public void AddCoin()
    {
        coinCount++;
        
        coinText.text = "coin : " + coinCount;
    }

}
