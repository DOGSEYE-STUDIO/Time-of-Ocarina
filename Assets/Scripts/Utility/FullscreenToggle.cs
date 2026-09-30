using UnityEngine;
using UnityEngine.InputSystem;

public class FullscreenToggle : MonoBehaviour
{
    private void Start()
    {
        //Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f11Key.wasPressedThisFrame)
        {
            ToggleFullscreen();
        }
    }

    private void ToggleFullscreen()
    {
        if (Screen.fullScreen)
        {
            Screen.SetResolution(1200, 800, FullScreenMode.Windowed);
        }
        else
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        }
    }
}