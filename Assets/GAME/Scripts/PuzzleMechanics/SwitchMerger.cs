
using System.Collections.Generic;
using UnityEngine;

public class SwitchMerger : MonoBehaviour
{
    [SerializeField] private ASwitch[] switches;
    [SerializeField] private bool mergeState = true;

    private bool isUpdating;

    private readonly Dictionary<ASwitch, System.Action<bool>> handlers =
        new Dictionary<ASwitch, System.Action<bool>>();

    private void OnEnable()
    {
        handlers.Clear();

        foreach (ASwitch currentSwitch in switches)
        {
            if (currentSwitch == null)
                continue;

            ASwitch capturedSwitch = currentSwitch;

            System.Action<bool> handler = isOn =>
                OnSwitchToggled(capturedSwitch, isOn);

            handlers.Add(capturedSwitch, handler);

            capturedSwitch.OnStateChanged += handler;
        }
    }

    private void OnDisable()
    {
        foreach (var pair in handlers)
        {
            if (pair.Key == null)
                continue;

            pair.Key.OnStateChanged -= pair.Value;
        }

        handlers.Clear();
    }

    private void OnSwitchToggled(ASwitch source, bool isOn)
    {
        if (isUpdating)
            return;

        isUpdating = true;

        foreach (ASwitch currentSwitch in switches)
        {
            if (currentSwitch == null || currentSwitch == source)
                continue;

            if (mergeState)
            {
                if (currentSwitch.IsOn != isOn)
                {
                    currentSwitch.Toggle();
                }
            }
            else
            {
                currentSwitch.Toggle();
            }
        }

        isUpdating = false;
    }
}
