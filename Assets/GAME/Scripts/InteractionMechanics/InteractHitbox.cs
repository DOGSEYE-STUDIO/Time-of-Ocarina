using UnityEngine;

public class InteractHitbox : MonoBehaviour
{
    public APlayerInteractable interactable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        interactable = other.GetComponent<APlayerInteractable>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        interactable = null;
    }
}
