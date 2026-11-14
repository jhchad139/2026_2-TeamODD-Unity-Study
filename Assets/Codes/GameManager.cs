using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Serializable]
    private struct Stage
    {
        public GameObject root;
        public Transform spawnPoint;
    }

    private const int StartingLives = 3;

    [SerializeField] private Player player;
    [SerializeField] private Stage[] stages;
    [SerializeField] private Text coinText;
    [SerializeField] private Text livesText;
    [SerializeField] private GameObject endCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private Button retryButton;
    [SerializeField] private float fallLimit = -6f;

    private int stageIndex;
    private int coinCount;
    private int lives = StartingLives;
    private double lastDamageStep = double.NegativeInfinity;

    public bool IsCleared { get; private set; }
    public bool IsFinished => IsCleared || lives == 0;

    private void Start()
    {
        Time.timeScale = 1f;
        for (int index = 0; index < stages.Length; index++)
        {
            stages[index].root.SetActive(index == 0);
        }

        endCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
        coinText.text = "coin : 0";
        UpdateLivesText();
        RespawnPlayer();
    }

    private void Update()
    {
        if (!IsFinished && player.transform.position.y < fallLimit)
        {
            LoseLife();
        }
    }

    public void LoseLife()
    {
        if (IsFinished || lastDamageStep == Time.fixedTimeAsDouble)
        {
            return;
        }

        lastDamageStep = Time.fixedTimeAsDouble;
        lives--;
        UpdateLivesText();

        if (lives == 0)
        {
            ShowEndScreen(gameOverCanvas);
            EventSystem.current.SetSelectedGameObject(retryButton.gameObject);
            return;
        }

        RespawnPlayer();
    }

    private void UpdateLivesText()
    {
        livesText.text = $"목숨 {lives}";
    }

    private void RespawnPlayer()
    {
        Stage stage = stages[stageIndex];
        foreach (var platform in stage.root.GetComponentsInChildren<FallingPlatform>())
        {
            platform.ResetPlatform();
        }

        player.Respawn(stage.spawnPoint.position);
    }

    public void MoveToNextStage()
    {
        if (IsFinished)
        {
            return;
        }

        if (stageIndex + 1 == stages.Length)
        {
            IsCleared = true;
            ShowEndScreen(endCanvas);
            return;
        }

        stages[stageIndex].root.SetActive(false);
        stageIndex++;
        stages[stageIndex].root.SetActive(true);
        RespawnPlayer();
    }

    public void AddCoin()
    {
        coinCount++;
        coinText.text = $"coin : {coinCount}";
    }

    private void ShowEndScreen(GameObject canvas)
    {
        player.enabled = false;
        canvas.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnDestroy()
    {
        if (IsFinished)
        {
            Time.timeScale = 1f;
        }
    }
}
