using UnityEngine;

[RequireComponent(typeof(ItemDrop))]
public class SwordBreakableDrop : SwordBreakable
{
    public override void OnSwordHit()
    {
        GetComponent<ItemDrop>().Drop();
        base.OnSwordHit();
    }
}
