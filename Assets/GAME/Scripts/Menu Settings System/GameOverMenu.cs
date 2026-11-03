using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private string _menuScene = "MAIN MENU";
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _menuButton;

    public static GameOverMenu Instance { get; private set; }
    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        Instance = this;

        _panel.SetActive(false);

        _retryButton.onClick.AddListener(Retry);
        _menuButton.onClick.AddListener(GoToMenu);
    }

    public void Show()
    {
        IsGameOver = true;
        PlayerController.Instance.StopPlayer();

        _panel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_retryButton.gameObject);
    }

    private void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMenu()
    {
        SceneManager.LoadScene(_menuScene);
    }
}
