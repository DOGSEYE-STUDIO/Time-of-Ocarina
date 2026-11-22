using UnityEngine;

public abstract class APickable : MonoBehaviour
{
    [SerializeField] private string _nombre = "un objeto";

    public string Nombre { get { return _nombre; } }

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
