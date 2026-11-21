using DG.Tweening;
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryMenu : MonoBehaviour
{
    [SerializeField] private string _menuScene = "MAIN MENU";
    [SerializeField] private CanvasGroup _panel;
    [SerializeField] private Button _menuButton;

    public static VictoryMenu Instance { get; private set; }

    private bool _mostrando;

    private void Awake()
    {
        Instance = this;

        _panel.gameObject.SetActive(false);
        _menuButton.onClick.AddListener(GoToMenu);
    }

    private void Update()
    {
        if (_mostrando && EventSystem.current.currentSelectedGameObject == null) EventSystem.current.SetSelectedGameObject(_menuButton.gameObject);
    }

    public void Show()
    {
        if (_mostrando) return;

        _mostrando = true;
        PlayerController.Instance.StopPlayer();
        new Sound(SFX.MenuStart).SetOutput(Output.SFX).Play();

        _panel.gameObject.SetActive(true);
        _panel.alpha = 0f;
        _panel.DOFade(1f, 0.8f).SetUpdate(true).SetLink(gameObject);
        EventSystem.current.SetSelectedGameObject(_menuButton.gameObject);
    }

    private void GoToMenu()
    {
        SceneManager.LoadScene(_menuScene);
    }
}
