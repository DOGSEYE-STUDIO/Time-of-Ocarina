using MelenitasDev.SoundsGood;
using UnityEngine;

public class ShopKeeper : APlayerInteractable
{
    [SerializeField] private ShopManager _shop;

    public override string ActionText { get { return _shop.Held != null ? "Pagar" : "Hablar"; } }

    public override void OnPlayerInteract()
    {
        if (_shop.Held == null)
        {
            new Sound(SFX.Talk).SetOutput(Output.SFX).Play();
            return;
        }

        if (_shop.Buy())
        {
            new Sound(SFX.Buy).SetOutput(Output.SFX).Play();
        }
        else
        {
            Debug.Log("No tienes monedas suficientes");
            new Sound(SFX.Error).SetOutput(Output.SFX).Play();
            _shop.ReturnHeld();
        }
    }
}
