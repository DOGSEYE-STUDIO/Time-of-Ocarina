using UnityEngine;

public abstract class APlayerInteractable : MonoBehaviour
{
    public abstract void OnPlayerInteract();

    public virtual string ActionText { get { return "Interactuar"; } }
}
