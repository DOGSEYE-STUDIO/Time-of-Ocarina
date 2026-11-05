using UnityEngine;

public class InteractNPC : APlayerInteractable
{
    public override string ActionText { get { return "Hablar"; } }

    public override void OnPlayerInteract()
    {
        Debug.Log("hola soy verity");
    }
}
