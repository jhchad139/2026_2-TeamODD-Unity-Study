using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// StageScene의 스테이지 버튼 하나
public class StageButton : MonoBehaviour
{
    public int stageIndex;          // 0~4: 스테이지 1~5,  5: 어려운 스테이지
    public Button button;
    public Image[] stars;
    public TMP_Text infoText;
    public GameObject lockObject;   // 잠겼을 때 보여줄 덮개

    public void Refresh(Color starOn, Color starOff){
        bool unlocked = GameData.IsUnlocked(stageIndex);

        if(button != null) button.interactable = unlocked;
        if(lockObject != null) lockObject.SetActive(!unlocked);

        int s = GameData.bestStars[stageIndex];
        for(int i = 0; i < stars.Length; i++)
            stars[i].color = i < s ? starOn : starOff;

        if(infoText != null){
            if(GameData.cleared[stageIndex])
                infoText.text = "BEST " + GameData.FormatTime(GameData.bestTimes[stageIndex]);
            else
                infoText.text = unlocked ? "NEW" : "";
        }
    }

    public void OnClick(){
        if(!GameData.IsUnlocked(stageIndex)) return;
        GameData.selectedStage = stageIndex;
        SceneManager.LoadScene("Game");
    }
}
