using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// StageScene 전체 관리: 버튼마다 별/잠금 상태 갱신
public class StageSelectUI : MonoBehaviour
{
    public StageButton[] buttons;
    public TMP_Text totalStarText;
    public TMP_Text hardHintText;

    public Color starOnColor = new Color(1f, 0.82f, 0.2f, 1f);
    public Color starOffColor = new Color(1f, 1f, 1f, 0.15f);

    private void Start(){
        Time.timeScale = 1f;

        foreach(StageButton b in buttons)
            b.Refresh(starOnColor, starOffColor);

        if(totalStarText != null)
            totalStarText.text = GameData.TotalStars() + " / " + (GameData.TotalStageCount * 3);

        if(hardHintText != null){
            hardHintText.text = GameData.IsHardUnlocked()
                ? "HARD STAGE UNLOCKED!"
                : "Get 3 stars on all 5 stages to unlock   (" + GameData.NormalStars() + " / 15)";
        }
    }

    public void GoTitle(){
        SceneManager.LoadScene("Title");
    }
}
