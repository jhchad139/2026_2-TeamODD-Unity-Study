using UnityEngine;
using TMPro;
using Unity.VisualScripting;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player player;

    public GameObject[] stages;
    public GameObject[] spawnpoint;

    public TMP_Text coinText;
    public int coinCount = 0;

    private void Awake(){
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void MoveStage(int n){ // n = 지금 진행하고 있는 스테이지
        stages[n-1].SetActive(false);
        stages[n].SetActive(true);
        SpawnPlayer(n);
    }

    public void SpawnPlayer(int n){
        player.transform.position = spawnpoint[n].transform.position;
    }

    public void AddCoint()
{
        coinCount++;
        coinText.text = "coin : " + coinCount;
    }
}
