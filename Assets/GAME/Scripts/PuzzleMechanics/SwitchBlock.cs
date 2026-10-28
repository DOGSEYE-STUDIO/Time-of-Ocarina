
using UnityEngine;

public class SwitchBlock : MonoBehaviour
{
    public enum BlockColor
    {
        Red,
        Blue
    }

    [SerializeField] private BlockColor blockColor;
    [SerializeField] private ASwitch targetSwitch;

    private Collider2D blockCollider;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    private void Start()
    {
        blockCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateBlock(targetSwitch.IsOn);
        targetSwitch.OnStateChanged += UpdateBlock;
    }

    private void UpdateBlock(bool isOn)
    {
        bool shouldBeActive = blockColor == BlockColor.Red ? !isOn : isOn;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = shouldBeActive ? activeSprite : inactiveSprite;
        }

        if (blockCollider != null)
        {
            blockCollider.enabled = shouldBeActive;
        }
    }
}
