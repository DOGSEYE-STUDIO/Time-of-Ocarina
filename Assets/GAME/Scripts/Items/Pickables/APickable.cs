using UnityEngine;

public abstract class APickable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnPickup();
            Destroy(gameObject);
        }
    }

    public abstract void OnPickup();
}
