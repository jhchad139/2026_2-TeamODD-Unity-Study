using UnityEngine;
using TMPro;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    public Player player;

    public GameObject[] Stages;
    public GameObject[] SpawnPoint;

    public TMP_Text coinText;

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
        Stages[n - 1].SetActive(false);

        Stages[n].SetActive(true);
        SpawnPlayer(n);
    }

    public void Addcoin()
    {
        coinCount ++;
        coinText.text = "Coin: " + coinCount;

    }

    public void SpawnPlayer(int n)
    {
        player.transform.position = SpawnPoint[n].transform.position;
      
    }
}
