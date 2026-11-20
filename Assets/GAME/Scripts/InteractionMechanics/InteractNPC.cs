using MelenitasDev.SoundsGood;
using UnityEngine;

public class InteractNPC : APlayerInteractable
{
    [SerializeField] private DialogueData dialogue;

    public override string ActionText { get { return "Hablar"; } }

    public override void OnPlayerInteract()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }
}
