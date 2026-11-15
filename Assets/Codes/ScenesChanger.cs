using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesChanger : MonoBehaviour
{
    // Title의 START 버튼 → 스테이지 선택 화면
    public void StartGame(){
        SceneManager.LoadScene("StageScene");
    }

    public void QuitGame(){
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
