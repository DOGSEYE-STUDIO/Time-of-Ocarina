using UnityEngine;

public class PressurePlate : ASwitch
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (IsOn) return;

        PushableObject pushable = other.gameObject.GetComponent<PushableObject>();

        if (other.gameObject.CompareTag("Player") || pushable != null)
        {
            IsOn = true;
            UpdateState();
            UpdateSprite();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PushableObject pushable = other.gameObject.GetComponent<PushableObject>();

        if (other.gameObject.CompareTag("Player") || pushable != null)
        {
            IsOn = false;
            UpdateState();
            UpdateSprite();
        }
    }
}
