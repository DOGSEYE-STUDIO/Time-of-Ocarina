using UnityEngine;

public class RoomClose : MonoBehaviour
{
    [SerializeField] ASwitch targetSwitch;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        targetSwitch.Toggle();

        GetComponent<Collider2D>().enabled = false;
    }
}
