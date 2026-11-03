using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private string _menuScene = "MAIN MENU";
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _menuButton;

    private InputAction _pauseAction;
    private bool _paused;

    private void Awake()
    {
        _panel.SetActive(false);

        _resumeButton.onClick.AddListener(Resume);
        _menuButton.onClick.AddListener(GoToMenu);

        _pauseAction = new InputAction(binding: "<Keyboard>/escape");
        _pauseAction.AddBinding("<Gamepad>/start");
    }

    private void OnEnable()
    {
        _pauseAction.performed += OnPause;
        _pauseAction.Enable();
    }

    private void OnDisable()
    {
        _pauseAction.performed -= OnPause;
        _pauseAction.Disable();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (GameOverMenu.Instance.IsGameOver) return;

        if (_paused) Resume();
        else Pause();
    }

    private void Pause()
    {
        _paused = true;
        Time.timeScale = 0f;
        PlayerController.Instance.StopPlayer();

        _panel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_resumeButton.gameObject);
    }

    private void Resume()
    {
        _paused = false;
        Time.timeScale = 1f;
        PlayerController.Instance.ResumePlayer();

        _panel.SetActive(false);
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_menuScene);
    }
}
