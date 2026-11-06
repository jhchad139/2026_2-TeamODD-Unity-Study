using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player player;
    
    public GameObject[] stages;
    public GameObject[] spawnPoints;

    [SerializeField] private TMP_Text coinText;
    [SerializeField] private GameObject endCanvas;
    
    public int coinCount;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

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
        player.transform.position = spawnPoints[n].transform.position;
        foreach (var platform in stages[n].GetComponentsInChildren<FallingPlatform>(true))
        {
            platform.gameObject.SetActive(true);
        }
    }

    public void AddCoin()
    {
        coinCount ++;
        coinText.text = $"Coin : {coinCount}";
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
