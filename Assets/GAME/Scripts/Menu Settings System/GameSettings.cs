using MelenitasDev.SoundsGood;
using MoreMountains.FeedbacksForThirdParty;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [SerializeField] private InputActionAsset _actions;

    private ColorAdjustments _colorAdjustments;

    public bool IsWeb { get { return Application.platform == RuntimePlatform.WebGLPlayer; } }

    public float Brightness
    {
        get { return PlayerPrefs.GetFloat("Brightness", 0.5f); }
        set
        {
            PlayerPrefs.SetFloat("Brightness", value);
            _colorAdjustments.postExposure.Override((value - 0.5f) * 2f);
        }
    }

    public bool CameraShake
    {
        get { return PlayerPrefs.GetInt("CameraShake", 1) == 1; }
        set
        {
            PlayerPrefs.SetInt("CameraShake", value ? 1 : 0);
            MMF_CinemachineImpulse.FeedbackTypeAuthorized = value;
        }
    }

    public bool VSync
    {
        get { return PlayerPrefs.GetInt("VSync", 1) == 1; }
        set
        {
            PlayerPrefs.SetInt("VSync", value ? 1 : 0);
            QualitySettings.vSyncCount = value ? 1 : 0;
        }
    }

    public bool Fullscreen
    {
        get { return Screen.fullScreen; }
        set
        {
            PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
            Screen.fullScreenMode = value ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Volume volume = gameObject.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 100;
        volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
        _colorAdjustments = volume.profile.Add<ColorAdjustments>();

        string bindings = PlayerPrefs.GetString("Bindings", "");
        if (bindings != "") _actions.LoadBindingOverridesFromJson(bindings);

        SceneManager.sceneLoaded += OnSceneLoaded;
        EnablePostProcessing();
    }

    private void Start()
    {
        SetVolume(Output.Master, GetVolume(Output.Master));
        SetVolume(Output.Music, GetVolume(Output.Music));
        SetVolume(Output.SFX, GetVolume(Output.SFX));

        Brightness = Brightness;
        CameraShake = CameraShake;

        if (IsWeb) return;

        VSync = VSync;

        if (PlayerPrefs.HasKey("Fullscreen")) Fullscreen = PlayerPrefs.GetInt("Fullscreen") == 1;
        if (PlayerPrefs.HasKey("ResolutionWidth")) SetResolution(PlayerPrefs.GetInt("ResolutionWidth"), PlayerPrefs.GetInt("ResolutionHeight"));
    }

    private void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public float GetVolume(Output output)
    {
        return PlayerPrefs.GetFloat(output.ToString(), 1f);
    }

    public void SetVolume(Output output, float volume)
    {
        SoundsGoodManager.ChangeOutputVolume(output, volume);
    }

    public void SetResolution(int width, int height)
    {
        PlayerPrefs.SetInt("ResolutionWidth", width);
        PlayerPrefs.SetInt("ResolutionHeight", height);
        Screen.SetResolution(width, height, Screen.fullScreenMode);
    }

    public void SaveBindings()
    {
        PlayerPrefs.SetString("Bindings", _actions.SaveBindingOverridesAsJson());
    }

    public void ResetBindings()
    {
        _actions.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey("Bindings");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnablePostProcessing();
    }

    private void EnablePostProcessing()
    {
        if (Camera.main != null) Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing = true;
    }
}
