using MelenitasDev.SoundsGood;
using UnityEngine;

public class InteractNPC : APlayerInteractable
{
    public override string ActionText { get { return "Hablar"; } }

    public override void OnPlayerInteract()
    {
        new Sound(SFX.Talk).SetOutput(Output.SFX).Play();
        Debug.Log("hola soy verity");
    }
}
