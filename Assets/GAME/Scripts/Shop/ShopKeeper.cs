using MelenitasDev.SoundsGood;
using UnityEngine;

public class ShopKeeper : APlayerInteractable
{
    [SerializeField] private ShopManager _shop;
    [SerializeField] private DialogueData _dialogoSaludo;
    [SerializeField] private DialogueData _dialogoGracias;
    [SerializeField] private DialogueData _dialogoSinDinero;
    [SerializeField] private DialogueData _dialogoSinHueco;

    public override string ActionText { get { return _shop.Held != null ? "Pagar" : "Hablar"; } }

    public override void OnPlayerInteract()
    {
        if (_shop.Held == null)
        {
            DialogueManager.Instance.StartDialogue(_dialogoSaludo);
            return;
        }

        bool tieneDinero = PlayerInventory.Instance.HasEnoughCoins(_shop.Held.Price);

        if (_shop.Buy())
        {
            new Sound(SFX.Buy).SetOutput(Output.SFX).Play();
            DialogueManager.Instance.StartDialogue(_dialogoGracias);
        }
        else
        {
            new Sound(SFX.Error).SetOutput(Output.SFX).Play();
            _shop.ReturnHeld();
            DialogueManager.Instance.StartDialogue(tieneDinero ? _dialogoSinHueco : _dialogoSinDinero);
        }
    }
}
