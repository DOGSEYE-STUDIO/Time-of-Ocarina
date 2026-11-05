using MelenitasDev.SoundsGood;
using UnityEngine;

[RequireComponent(typeof(ItemDrop))]
public class SwordBreakableDrop : SwordBreakable
{
    [SerializeField] private bool _isGrass;

    public override void OnSwordHit()
    {
        new Sound(_isGrass ? SFX.GrassCut : SFX.VaseBreak).SetOutput(Output.SFX).Play();
        GetComponent<ItemDrop>().Drop();
        base.OnSwordHit();
    }
}
