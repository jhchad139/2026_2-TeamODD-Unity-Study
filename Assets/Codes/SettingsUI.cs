using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

// 설정창: 배경음악/효과음 볼륨, 소리 켜기/끄기
// 값은 AudioManager에 저장되어 씬이 바뀌어도 유지됨 (게임을 완전히 끄면 초기화)
public class SettingsUI : MonoBehaviour
{
    public static SettingsUI current;

    public GameObject panel;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public TMP_Text bgmValueText;
    public TMP_Text sfxValueText;
    public TMP_Text muteText;

    public static bool IsOpen {
        get { return current != null && current.panel != null && current.panel.activeSelf; }
    }

    private void Awake(){
        current = this;
        if(panel != null) panel.SetActive(false);
    }

    private void OnDestroy(){
        if(current == this) current = null;
    }

    private void Start(){
        bgmSlider.onValueChanged.AddListener(v => { AudioManager.BgmVolume = v; Refresh(); });
        sfxSlider.onValueChanged.AddListener(v => { AudioManager.SfxVolume = v; Refresh(); });
    }

    private void Update(){
        // Game 씬에서는 GameManager가 ESC를 처리하고, 나머지 씬에서는 여기서 ESC로 닫기
        if(GameManager.instance == null && IsOpen &&
           Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            Close();
    }

    public void Open(){
        bgmSlider.SetValueWithoutNotify(AudioManager.BgmVolume);
        sfxSlider.SetValueWithoutNotify(AudioManager.SfxVolume);
        panel.SetActive(true);
        Refresh();
    }

    public void Close(){
        panel.SetActive(false);
    }

    public void ToggleMute(){
        AudioManager.Muted = !AudioManager.Muted;
        Refresh();
    }

    // 열려 있으면 닫고 true
    public static bool CloseIfOpen(){
        if(!IsOpen) return false;
        current.Close();
        return true;
    }

    void Refresh(){
        if(bgmValueText != null) bgmValueText.text = Mathf.RoundToInt(bgmSlider.value * 100) + "%";
        if(sfxValueText != null) sfxValueText.text = Mathf.RoundToInt(sfxSlider.value * 100) + "%";
        if(muteText != null) muteText.text = AudioManager.Muted ? "SOUND : OFF" : "SOUND : ON";
    }
}
