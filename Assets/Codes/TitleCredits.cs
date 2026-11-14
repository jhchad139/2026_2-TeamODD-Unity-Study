using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleCredits : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button startButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button closeButton;

    public void Show()
    {
        startButton.interactable = false;
        creditsButton.interactable = false;
        panel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
    }

    public void Hide()
    {
        panel.SetActive(false);
        startButton.interactable = true;
        creditsButton.interactable = true;
        EventSystem.current.SetSelectedGameObject(creditsButton.gameObject);
    }

    public void OpenMusicSource()
    {
        Application.OpenURL("https://www.youtube.com/watch?v=I6TOyrUubmI");
    }
}
