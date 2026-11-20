
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
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start()
    {
        blockCollider = GetComponent<Collider2D>();

        UpdateBlock(targetSwitch.IsOn);
        targetSwitch.OnStateChanged += UpdateBlock;
    }

    private void UpdateBlock(bool isOn)
    {
        bool shouldBeActive = blockColor == BlockColor.Red ? !isOn : isOn;

        if (spriteRenderer != null)
        {
            //spriteRenderer.sprite = shouldBeActive ? activeSprite : inactiveSprite;
        }

        if (blockCollider != null)
        {
            blockCollider.enabled = shouldBeActive;
        }
    }
}
