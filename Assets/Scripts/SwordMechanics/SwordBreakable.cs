using UnityEngine;

public class SwordBreakable : ASwordInteractable
{
    public override void OnSwordHit()
    {
        Destroy(gameObject);
    }
}