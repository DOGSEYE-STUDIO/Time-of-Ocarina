using UnityEngine;

public class FrisbeeSwitch : AFrisbeeInteractable
{
    private ASwitch switchController;

    private void Start()
    {
        switchController = GetComponent<ASwitch>();
    }

    public override void OnFrisbeeHit()
    {
        if (switchController != null)
            switchController.Toggle();
    }
}
