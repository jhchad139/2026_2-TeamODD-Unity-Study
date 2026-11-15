using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player player;

    [Tooltip("0~4: 일반 스테이지 1~5,  5: 어려운 스테이지")]
    public GameObject[] stages;
    [Tooltip("예비용. 각 스테이지 안에 이름이 Spawnpoint로 시작하는 오브젝트가 있으면 그걸 우선 사용")]
    public GameObject[] spawnpoint;

    [Header("스테이지별 배경 (stages와 같은 순서)")]
    public GameObject[] backgrounds;
    public Color[] skyColors;

    [Header("별 조건: 이 시간(초) 안에 깨면 별 1개")]
    public float[] starTimes = { 30f, 30f, 30f, 35f, 35f, 50f };

    [Header("플레이 화면 UI")]
    public TMP_Text stageText;   // 왼쪽 위 "STAGE 1" / "HARD STAGE"
    public TMP_Text coinText;
    public TMP_Text timerText;

    [Header("일반 스테이지 클리어 화면")]
    public GameObject resultCanvas;
    public Image[] resultStars;
    public TMP_Text resultInfoText;
    public GameObject nextButton;

    [Header("일시정지 화면 (ESC)")]
    public GameObject pauseCanvas;

    [Header("어려운 스테이지 클리어 화면")]
    public GameObject endCanvas;
    public Image[] endStars;
    public TMP_Text endInfoText;

    [Header("별 색")]
    public Color starOnColor = new Color(1f, 0.82f, 0.2f, 1f);
    public Color starOffColor = new Color(1f, 1f, 1f, 0.15f);

    public int coinCount = 0;
    int totalCoins = 0;
    float playTime = 0f;
    bool playing = false;
    bool paused = false;
    int currentStage = 0;

    private void Awake(){
        if(instance == null || instance == this)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        Time.timeScale = 1f;

        currentStage = Mathf.Clamp(GameData.selectedStage, 0, stages.Length - 1);

        // 고른 스테이지만 켜기
        for(int i = 0; i < stages.Length; i++)
            stages[i].SetActive(i == currentStage);

        // 그 스테이지의 배경과 하늘색
        if(backgrounds != null)
            for(int i = 0; i < backgrounds.Length; i++)
                if(backgrounds[i] != null) backgrounds[i].SetActive(i == currentStage);

        if(skyColors != null && currentStage < skyColors.Length && Camera.main != null)
            Camera.main.backgroundColor = skyColors[currentStage];

        if(resultCanvas != null) resultCanvas.SetActive(false);
        if(endCanvas != null) endCanvas.SetActive(false);
        if(pauseCanvas != null) pauseCanvas.SetActive(false);
    }

    private void Start(){
        player.nowStage = currentStage;

        if(stageText != null)
            stageText.text = currentStage == GameData.HardStageIndex ? "HARD STAGE" : "STAGE " + (currentStage + 1);

        coinCount = 0;
        totalCoins = CountCoins(stages[currentStage]);
        UpdateCoinText();

        playTime = 0f;
        playing = true;

        SpawnPlayer(currentStage);
    }

    private void OnDestroy(){
        if(instance == this) instance = null;
    }

    private void Update(){
        // ESC: 일시정지 / 다시 시작
        if(playing && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame){
            if(SettingsUI.CloseIfOpen()) { }   // 설정창이 열려 있으면 설정창만 닫기
            else if(paused) Resume();
            else Pause();
        }

        if(!playing || paused) return;

        playTime += Time.deltaTime;
        if(timerText != null)
            timerText.text = GameData.FormatTime(playTime);
    }

    int CountCoins(GameObject stage){
        int count = 0;
        foreach(Transform t in stage.GetComponentsInChildren<Transform>(true))
            if(t.CompareTag("coin")) count++;
        return count;
    }

    // 스테이지 안에서 이름이 "Spawnpoint"로 시작하는 오브젝트 찾기
    Transform FindSpawn(int n){
        foreach(Transform t in stages[n].GetComponentsInChildren<Transform>(true))
            if(t.name.StartsWith("Spawnpoint")) return t;

        if(spawnpoint != null && n < spawnpoint.Length && spawnpoint[n] != null)
            return spawnpoint[n].transform;

        Debug.LogWarning(stages[n].name + " 안에 Spawnpoint가 없습니다.");
        return stages[n].transform;
    }

    public void SpawnPlayer(int n){
        player.transform.position = FindSpawn(n).position;
        player.ResetState();

        // 죽었다 살아나면 그 스테이지의 무너진 발판도 원래대로
        foreach(FallingPf pf in stages[n].GetComponentsInChildren<FallingPf>(true))
            pf.ResetPlatform();
    }

    public void AddCoin(){
        coinCount++;
        UpdateCoinText();
    }

    void UpdateCoinText(){
        if(coinText != null)
            coinText.text = coinCount + " / " + totalCoins;
    }

    float TargetTime(int n){
        if(starTimes != null && n < starTimes.Length) return starTimes[n];
        return 60f;
    }

    // Finish에 닿으면 Player가 호출
    public void StageClear(){
        if(!playing) return;
        playing = false;
        player.canMove = false;

        float target = TargetTime(currentStage);
        bool allCoins = coinCount >= totalCoins;
        bool fast = playTime <= target;

        int stars = 1;              // 클리어
        if(allCoins) stars++;       // 코인 전부
        if(fast) stars++;           // 시간 안에

        GameData.SaveResult(currentStage, stars, playTime);

        AudioManager.Play(Sfx.Clear);
        AudioManager.Duck(true);

        string info =
            "CLEAR\n" +
            "COIN   " + coinCount + " / " + totalCoins + (allCoins ? "   OK" : "") + "\n" +
            "TIME   " + GameData.FormatTime(playTime) + "  (goal " + GameData.FormatTime(target) + ")" + (fast ? "   OK" : "");

        if(currentStage == GameData.HardStageIndex){
            ShowStars(endStars, stars);
            if(endInfoText != null) endInfoText.text = info;
            if(endCanvas != null) endCanvas.SetActive(true);
        }
        else {
            ShowStars(resultStars, stars);
            if(resultInfoText != null) resultInfoText.text = info;
            if(nextButton != null) nextButton.SetActive(HasNextStage());
            if(resultCanvas != null) resultCanvas.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    void ShowStars(Image[] images, int count){
        if(images == null) return;
        for(int i = 0; i < images.Length; i++)
            images[i].color = i < count ? starOnColor : starOffColor;
    }

    bool HasNextStage(){
        int next = currentStage + 1;
        if(next >= stages.Length) return false;
        return GameData.IsUnlocked(next);
    }

    // ----- 일시정지 -----

    public void Pause(){
        if(!playing || paused) return;
        paused = true;
        player.canMove = false;
        AudioManager.Duck(true);
        if(pauseCanvas != null) pauseCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume(){
        if(!paused) return;
        SettingsUI.CloseIfOpen();
        paused = false;
        player.canMove = true;
        AudioManager.Duck(false);
        if(pauseCanvas != null) pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    // ----- 버튼용 -----

    public void NextStage(){
        if(!HasNextStage()) return;
        GameData.selectedStage = currentStage + 1;
        LoadScene("Game");
    }

    public void Restart(){
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoStageSelect(){
        LoadScene("StageScene");
    }

    public void GoTitle(){
        LoadScene("Title");
    }

    void LoadScene(string name){
        Time.timeScale = 1f;
        SceneManager.LoadScene(name);
    }
}
