using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] stages;
    public GameObject[] spawnPoint;
    public Player player;
    public TextMeshProUGUI coinText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    void Start(){

    }

    void Update(){
        if (coinText != null)
        {
            coinText.text = "Coins: " + player.coinCount;
        }
    }

    public void MoveStage(int n)
    {
        Debug.Log(n);
        if(n != 0)
            stages[n - 1].SetActive(false);

        if(n >= stages.Length){
            SceneChanger.EndGame();
            return ;
        }

        stages[n].SetActive(true);
        player.SetSpawnPoint(spawnPoint[n]);
        if (!player.isRespawning)
            player.Respawn();
    }
}
