using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public InteractHitbox hitbox;

    private PlayerPush playerPush;

    private void Awake()
    {
        playerPush = GetComponent<PlayerPush>();
    }

    public void UpdateFacingDirection(Vector2 input)
    {
        if (input == Vector2.zero) return;

        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        hitbox.transform.localRotation = Quaternion.Euler(0, 0, angle + 90f);
    }

    public void Interact()
    {
        if (hitbox.interactable != null)
        {
            hitbox.interactable.OnPlayerInteract();
        }
    }

    public void BeginInteract()
    {
        PushableObject pushable = hitbox.interactable as PushableObject;

        if (pushable != null)
        {
            playerPush.TryGrab(pushable);
        }
    }

    public void EndInteract()
    {
        playerPush.ReleaseObject();
    }
}
