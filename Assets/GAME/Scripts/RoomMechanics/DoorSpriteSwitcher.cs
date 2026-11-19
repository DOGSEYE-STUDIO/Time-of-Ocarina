using UnityEngine;

public class DoorSpriteSwitcher : MonoBehaviour
{
    [SerializeField] private Door door;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite lockedSprite;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        door.onDoorChange += UpdateSprite;

        if (door.isLocked) spriteRenderer.sprite = lockedSprite;
    }

    void UpdateSprite(bool isOn)
    {
        if (isOn)
        {
            spriteRenderer.sprite = closedSprite;
        }
    }
}
