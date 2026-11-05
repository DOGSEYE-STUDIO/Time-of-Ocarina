using UnityEngine;

public class InteractHitbox : MonoBehaviour
{
    public APlayerInteractable interactable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        APlayerInteractable found = other.GetComponent<APlayerInteractable>();
        if (found != null) interactable = found;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<APlayerInteractable>() == interactable) interactable = null;
    }
}
