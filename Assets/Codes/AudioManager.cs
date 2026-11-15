using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// 소리 관리자
//  - 게임이 시작되면 자동으로 만들어지고, 씬이 바뀌어도 사라지지 않음 (씬에 따로 놓을 필요 없음)
//  - 소리 파일은 Assets/Resources/Audio 폴더에서 이름으로 불러옴
//  - 씬에 맞춰 배경음악 자동 변경: Title/StageScene → 메뉴 음악, Game → 스테이지 음악 (Hard는 따로)
//  - 모든 UI 버튼에 클릭 소리 자동 연결
//  - M 키: 소리 켜기/끄기
//
// 다른 스크립트에서 효과음 재생:  AudioManager.Play(Sfx.Jump);
public enum Sfx { Jump, Coin, Death, Clear, Click, Crumble }

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Range(0f, 1f)] public float bgmVolume = 0.35f;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    AudioSource bgmSource;
    AudioSource sfxSource;
    AudioClip[] sfxClips;
    AudioClip menuBgm, gameBgm, hardBgm;
    bool muted = false;
    bool ducked = false;   // 일시정지/클리어 중에는 음악을 작게

    // 게임 시작 시 자동 생성
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Create(){
        if(instance != null) return;
        GameObject go = new GameObject("AudioManager");
        DontDestroyOnLoad(go);
        go.AddComponent<AudioManager>();
    }

    // 에디터에서 플레이를 다시 시작할 때를 대비해 초기화
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatic(){
        instance = null;
    }

    private void Awake(){
        if(instance != null && instance != this){
            Destroy(gameObject);
            return;
        }
        instance = this;

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        sfxClips = new AudioClip[System.Enum.GetValues(typeof(Sfx)).Length];
        sfxClips[(int)Sfx.Jump] = Load("jump");
        sfxClips[(int)Sfx.Coin] = Load("coin");
        sfxClips[(int)Sfx.Death] = Load("death");
        sfxClips[(int)Sfx.Clear] = Load("clear");
        sfxClips[(int)Sfx.Click] = Load("click");
        sfxClips[(int)Sfx.Crumble] = Load("crumble");

        menuBgm = Load("bgm_menu");
        gameBgm = Load("bgm_game");
        hardBgm = Load("bgm_hard");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy(){
        if(instance == this){
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    AudioClip Load(string name){
        AudioClip clip = Resources.Load<AudioClip>("Audio/" + name);
        if(clip == null) Debug.LogWarning("소리 파일을 찾을 수 없음: Resources/Audio/" + name);
        return clip;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        ducked = false;

        // 씬에 맞는 배경음악
        AudioClip bgm = menuBgm;
        if(scene.name == "Game")
            bgm = GameData.selectedStage == GameData.HardStageIndex ? hardBgm : gameBgm;
        PlayBgm(bgm);

        // 모든 버튼에 클릭 소리 (꺼져 있는 화면의 버튼 포함)
        foreach(Button b in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            b.onClick.AddListener(() => Play(Sfx.Click));
    }

    private void Update(){
        if(Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
            muted = !muted;

        // 볼륨 반영 (Time.timeScale = 0 일 때도 소리는 계속 나옴)
        float target = muted ? 0f : bgmVolume * (ducked ? 0.35f : 1f);
        bgmSource.volume = Mathf.MoveTowards(bgmSource.volume, target, Time.unscaledDeltaTime * 2f);
        sfxSource.volume = muted ? 0f : sfxVolume;
    }

    void PlayBgm(AudioClip clip){
        if(clip == null) return;
        if(bgmSource.clip == clip && bgmSource.isPlaying) return; // 같은 음악이면 이어서 재생
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    // ----- 설정창에서 쓰는 값 -----

    public static float BgmVolume {
        get { return instance != null ? instance.bgmVolume : 0f; }
        set { if(instance != null) instance.bgmVolume = Mathf.Clamp01(value); }
    }

    public static float SfxVolume {
        get { return instance != null ? instance.sfxVolume : 0f; }
        set { if(instance != null) instance.sfxVolume = Mathf.Clamp01(value); }
    }

    public static bool Muted {
        get { return instance != null && instance.muted; }
        set { if(instance != null) instance.muted = value; }
    }

    // ----- 다른 스크립트에서 부르는 함수 -----

    public static void Play(Sfx sfx){
        if(instance == null) return;
        AudioClip clip = instance.sfxClips[(int)sfx];
        if(clip != null) instance.sfxSource.PlayOneShot(clip);
    }

    // 일시정지/클리어 화면에서 음악을 작게
    public static void Duck(bool on){
        if(instance != null) instance.ducked = on;
    }
}
