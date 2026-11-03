using System;
using DG.Tweening;
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string _gameScene = "GAME";
    [SerializeField] private string _webUrl;
    [SerializeField] private InputActionReference _cancel;

    [Header("Paneles")]
    [SerializeField] private GameObject _pressStartPanel;
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _creditsPanel;
    [SerializeField] private CanvasGroup _pressStartText;
    [SerializeField] private Transform _infoButtonRoot;

    [Header("Botones")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _infoButton;
    [SerializeField] private Button _settingsFirstButton;
    [SerializeField] private Button _settingsBackButton;
    [SerializeField] private Button _creditsBackButton;

    private GameObject _openPanel;
    private GameObject _returnSelection;
    private GameObject _lastSelected;
    private IDisposable _anyButton;

    private void Start()
    {
        _mainPanel.SetActive(false);
        _settingsPanel.SetActive(false);
        _creditsPanel.SetActive(false);
        _pressStartPanel.SetActive(true);

        _playButton.onClick.AddListener(() => SceneManager.LoadScene(_gameScene));
        _settingsButton.onClick.AddListener(() => Open(_settingsPanel, _settingsFirstButton));
        _creditsButton.onClick.AddListener(() => Open(_creditsPanel, _creditsBackButton));
        _quitButton.onClick.AddListener(Application.Quit);
        _infoButton.onClick.AddListener(() => Application.OpenURL(_webUrl));
        _settingsBackButton.onClick.AddListener(Back);
        _creditsBackButton.onClick.AddListener(Back);

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            _quitButton.gameObject.SetActive(false);
        }

        _pressStartText.DOFade(0.25f, 0.7f).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject);
        _infoButtonRoot.DOScale(1.08f, 0.8f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject);

        _cancel.action.Enable();
        _cancel.action.performed += OnCancel;
        _anyButton = InputSystem.onAnyButtonPress.CallOnce(_ => ShowMainPanel());
    }

    private void OnDestroy()
    {
        _cancel.action.performed -= OnCancel;
        _anyButton?.Dispose();
    }

    private void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected != null) _lastSelected = selected;
        else if (_lastSelected != null && _lastSelected.activeInHierarchy) EventSystem.current.SetSelectedGameObject(_lastSelected);
    }

    private void ShowMainPanel()
    {
        new Sound(SFX.coin).SetOutput(Output.SFX).Play();

        _pressStartPanel.SetActive(false);
        _mainPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_playButton.gameObject);
    }

    private void Open(GameObject panel, Button firstButton)
    {
        _returnSelection = EventSystem.current.currentSelectedGameObject;
        _openPanel = panel;

        _mainPanel.SetActive(false);
        panel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
    }

    private void Back()
    {
        _openPanel.SetActive(false);
        _openPanel = null;

        _mainPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_returnSelection);
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (_openPanel == null || RebindButton.IsRebinding) return;

        new Sound(SFX.coin).SetOutput(Output.SFX).Play();
        Back();
    }
}
