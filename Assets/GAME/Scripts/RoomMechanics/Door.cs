using UnityEngine;

public class Door : APlayerInteractable
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    public bool isLocked;
    public bool isBossDoor;

    [SerializeField] private ASwitch targetSwitch;

    public Sprite openSprite;
    public Sprite closedSprite;
    public Sprite lockedSprite;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (!isLocked) targetSwitch.OnStateChanged += UpdateDoor;
        else spriteRenderer.sprite = lockedSprite;
    }

    private void UpdateDoor(bool isOn)
    {
        boxCollider.enabled = !isOn;
        spriteRenderer.sprite = isOn ? closedSprite : openSprite;
    }

    public override void OnPlayerInteract()
    {
        if (isLocked && PlayerInventory.Instance.HasKey())
        {
            UpdateDoor(true);
            PlayerInventory.Instance.RemoveKey();
            return;
        }

        if (isBossDoor && isLocked && PlayerInventory.Instance.HasBossKey)
        {
            UpdateDoor(true);
            return;
        }
    }
}
