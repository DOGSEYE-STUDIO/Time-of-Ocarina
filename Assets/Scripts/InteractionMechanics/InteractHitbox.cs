using UnityEngine;

public class InteractHitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        APlayerInteractable interactable = other.GetComponent<APlayerInteractable>();

        if (interactable != null)
        {
            interactable.OnPlayerInteract();
        }
    }
}
