using UnityEngine;

public class SwitchCombiner : ASwitch
{
    [SerializeField] private ASwitch[] switches;

    private void OnEnable()
    {
        if (switches == null)
            return;

        foreach (ASwitch sw in switches)
        {
            if (sw != null)
                sw.OnStateChanged += CheckSwitches;
        }
    }

    private void OnDisable()
    {
        if (switches == null)
            return;

        foreach (ASwitch sw in switches)
        {
            if (sw != null)
                sw.OnStateChanged -= CheckSwitches;
        }
    }

    private void Start()
    {
        CheckSwitches(false);
    }

    private void CheckSwitches(bool state)
    {
        foreach (ASwitch sw in switches)
        {
            if (sw == null || !sw.IsOn)
            {
                if (IsOn)
                    Toggle();

                return;
            }
        }

        if (!IsOn)
            Toggle();
    }
}