using System.Collections.Generic;
using MelenitasDev.SoundsGood;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Pestañas")]
    [SerializeField] private Button[] _tabButtons;
    [SerializeField] private GameObject[] _tabs;
    [SerializeField] private Sprite _activeTabSprite;
    [SerializeField] private Sprite _idleTabSprite;

    [Header("Sonido")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    [Header("Pantalla")]
    [SerializeField] private Toggle _fullscreenToggle;
    [SerializeField] private Toggle _vSyncToggle;
    [SerializeField] private Slider _brightnessSlider;
    [SerializeField] private GameObject _resolutionRow;
    [SerializeField] private GameObject _vSyncRow;
    [SerializeField] private TMP_Text _resolutionLabel;
    [SerializeField] private Button _previousResolution;
    [SerializeField] private Button _nextResolution;

    [Header("Controles")]
    [SerializeField] private RebindButton[] _rebindButtons;
    [SerializeField] private Button _restoreButton;
    [SerializeField] private TMP_Text _controlsMessage;

    [Header("Accesibilidad")]
    [SerializeField] private Toggle _cameraShakeToggle;

    private readonly List<Vector2Int> _resolutions = new List<Vector2Int>();
    private int _resolutionIndex;
    private float _nextSfxPreview;

    private void Awake()
    {
        for (int i = 0; i < _tabButtons.Length; i++)
        {
            int index = i;
            _tabButtons[i].onClick.AddListener(() => ShowTab(index));
        }

        GameSettings settings = GameSettings.Instance;

        _masterSlider.onValueChanged.AddListener(value => settings.SetVolume(Output.Master, value));
        _musicSlider.onValueChanged.AddListener(value => settings.SetVolume(Output.Music, value));
        _sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        _fullscreenToggle.onValueChanged.AddListener(value => settings.Fullscreen = value);
        _vSyncToggle.onValueChanged.AddListener(value => settings.VSync = value);
        _brightnessSlider.onValueChanged.AddListener(value => settings.Brightness = value);
        _cameraShakeToggle.onValueChanged.AddListener(value => settings.CameraShake = value);
        _previousResolution.onClick.AddListener(() => ChangeResolution(-1));
        _nextResolution.onClick.AddListener(() => ChangeResolution(1));
        _restoreButton.onClick.AddListener(RestoreControls);

        if (settings.IsWeb)
        {
            _resolutionRow.SetActive(false);
            _vSyncRow.SetActive(false);
        }

        BuildResolutions();
    }

    private void OnEnable()
    {
        GameSettings settings = GameSettings.Instance;

        _masterSlider.SetValueWithoutNotify(settings.GetVolume(Output.Master));
        _musicSlider.SetValueWithoutNotify(settings.GetVolume(Output.Music));
        _sfxSlider.SetValueWithoutNotify(settings.GetVolume(Output.SFX));
        _fullscreenToggle.SetIsOnWithoutNotify(settings.Fullscreen);
        _vSyncToggle.SetIsOnWithoutNotify(settings.VSync);
        _brightnessSlider.SetValueWithoutNotify(settings.Brightness);
        _cameraShakeToggle.SetIsOnWithoutNotify(settings.CameraShake);
        _controlsMessage.text = "";

        ShowTab(0);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }

    private void ShowTab(int index)
    {
        for (int i = 0; i < _tabs.Length; i++)
        {
            _tabs[i].SetActive(i == index);
            _tabButtons[i].image.sprite = i == index ? _activeTabSprite : _idleTabSprite;
        }
    }

    private void OnSfxChanged(float value)
    {
        GameSettings.Instance.SetVolume(Output.SFX, value);

        if (Time.unscaledTime < _nextSfxPreview) return;

        _nextSfxPreview = Time.unscaledTime + 0.12f;
        new Sound(SFX.coin).SetOutput(Output.SFX).Play();
    }

    private void BuildResolutions()
    {
        foreach (Resolution resolution in Screen.resolutions)
        {
            Vector2Int size = new Vector2Int(resolution.width, resolution.height);

            if (size.x > Display.main.systemWidth || size.y > Display.main.systemHeight) continue;
            if (_resolutions.Contains(size)) continue;

            if (size.x == Screen.width && size.y == Screen.height) _resolutionIndex = _resolutions.Count;

            _resolutions.Add(size);
        }

        RefreshResolutionLabel();
    }

    private void ChangeResolution(int direction)
    {
        if (_resolutions.Count == 0) return;

        _resolutionIndex = (_resolutionIndex + direction + _resolutions.Count) % _resolutions.Count;
        GameSettings.Instance.SetResolution(_resolutions[_resolutionIndex].x, _resolutions[_resolutionIndex].y);
        RefreshResolutionLabel();
    }

    private void RefreshResolutionLabel()
    {
        if (_resolutions.Count == 0) return;

        _resolutionLabel.text = _resolutions[_resolutionIndex].x + " x " + _resolutions[_resolutionIndex].y;
    }

    private void RestoreControls()
    {
        GameSettings.Instance.ResetBindings();

        foreach (RebindButton button in _rebindButtons) button.Refresh();

        _controlsMessage.text = "Controles restaurados.";
    }
}
