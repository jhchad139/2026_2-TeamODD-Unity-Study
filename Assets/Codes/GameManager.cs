using System;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player player;
    
    public GameObject[] stages;
    public GameObject[] spawnPoints;

    [SerializeField] private TMP_Text coinText;
    
    public int coinCount;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        coinCount = 0;
    }

    public void MoveStage(int n)
    {
        stages[n - 1].SetActive(false);
 
        stages[n].SetActive(true);
        
        SpawnPlayer(n);
    }
    
    public void SpawnPlayer(int n)
    {
        player.transform.position = spawnPoints[n].transform.position;
    }

    public void AddCoin()
    {
        coinCount ++;
        coinText.text = $"Coin : {coinCount}";
    }
}
