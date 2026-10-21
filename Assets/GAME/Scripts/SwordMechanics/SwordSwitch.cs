using UnityEngine;

public class SwordSwitch : ASwordInteractable
{
    private ASwitch switchController;

    private void Start()
    {
        switchController = GetComponent<ASwitch>();
    }

    public override void OnSwordHit()
    {
        if (switchController != null)
            switchController.Toggle();
    }
}