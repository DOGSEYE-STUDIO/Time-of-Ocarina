using MelenitasDev.SoundsGood;
using UnityEngine;

[RequireComponent(typeof(ItemDrop))]
public class SwordBreakableDrop : SwordBreakable
{
    [SerializeField] private bool _isGrass;
    [SerializeField] private GameObject _efectoRotura;

    public override void OnSwordHit()
    {
        new Sound(_isGrass ? SFX.GrassCut : SFX.VaseBreak).SetOutput(Output.SFX).Play();
        if (_efectoRotura != null) Instantiate(_efectoRotura, transform.position, Quaternion.identity);
        GetComponent<ItemDrop>().Drop();
        base.OnSwordHit();
    }
}
