using UnityEngine;

public class ShopKeeper : APlayerInteractable
{
    [SerializeField] private ShopManager _shop;

    public override string ActionText { get { return _shop.Held != null ? "Pagar" : "Hablar"; } }

    public override void OnPlayerInteract()
    {
        if (_shop.Held == null) return;

        if (!_shop.Buy())
        {
            Debug.Log("No tienes monedas suficientes");
            _shop.ReturnHeld();
        }
    }
}
