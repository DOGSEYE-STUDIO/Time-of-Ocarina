using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindButton : MonoBehaviour
{
    [Serializable]
    private struct ControlIcon
    {
        public string Control;
        public Sprite Icon;
    }

    public static bool IsRebinding;

    [SerializeField] private InputActionReference _action;
    [SerializeField] private string _compositePart;
    [SerializeField] private string _device = "<Keyboard>";
    [SerializeField] private TMP_Text _label;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _message;
    [SerializeField] private ControlIcon[] _icons;

    private InputActionRebindingExtensions.RebindingOperation _operation;
    private int _bindingIndex;
    private bool _actionWasEnabled;

    private void Awake()
    {
        var bindings = _action.action.bindings;

        for (int i = 0; i < bindings.Count; i++)
        {
            if (bindings[i].isComposite || !bindings[i].path.StartsWith(_device)) continue;
            if (_compositePart != "" && bindings[i].name != _compositePart) continue;

            _bindingIndex = i;
            break;
        }

        GetComponent<Button>().onClick.AddListener(StartRebind);
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void OnDisable()
    {
        EndRebind();
    }

    public void Refresh()
    {
        string path = _action.action.bindings[_bindingIndex].effectivePath;
        string control = path.Substring(path.IndexOf('/') + 1);
        Sprite sprite = null;

        foreach (ControlIcon entry in _icons)
        {
            if (entry.Control == control) sprite = entry.Icon;
        }

        _icon.sprite = sprite;
        _icon.gameObject.SetActive(sprite != null);
        _label.text = sprite != null ? "" : _action.action.GetBindingDisplayString(_bindingIndex);
    }

    private void StartRebind()
    {
        if (IsRebinding) return;

        IsRebinding = true;
        EventSystem.current.sendNavigationEvents = false;

        _actionWasEnabled = _action.action.enabled;
        _action.action.Disable();
        _icon.gameObject.SetActive(false);
        _label.text = "...";
        _message.text = "Pulsa una tecla o botón. Esc cancela.";

        _operation = _action.action.PerformInteractiveRebinding(_bindingIndex)
            .WithControlsHavingToMatchPath(_device)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(_ => FinishRebind())
            .OnCancel(_ => CancelRebind())
            .Start();
    }

    private void FinishRebind()
    {
        if (IsAlreadyUsed())
        {
            _action.action.RemoveBindingOverride(_bindingIndex);
            _message.text = "Ese control ya está en uso.";
        }
        else
        {
            GameSettings.Instance.SaveBindings();
            _message.text = "";
        }

        EndRebind();
        Refresh();
    }

    private void CancelRebind()
    {
        _message.text = "";
        EndRebind();
        Refresh();
    }

    private bool IsAlreadyUsed()
    {
        string newPath = _action.action.bindings[_bindingIndex].effectivePath;

        foreach (InputAction action in _action.action.actionMap.actions)
        {
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action == _action.action && i == _bindingIndex) continue;
                if (action.bindings[i].effectivePath == newPath) return true;
            }
        }

        return false;
    }

    private void EndRebind()
    {
        if (_operation == null) return;

        _operation.Dispose();
        _operation = null;

        if (_actionWasEnabled) _action.action.Enable();

        DOVirtual.DelayedCall(0.2f, () =>
        {
            IsRebinding = false;
            if (EventSystem.current != null) EventSystem.current.sendNavigationEvents = true;
        }, true);
    }
}
