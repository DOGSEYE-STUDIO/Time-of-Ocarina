
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
    [SerializeField] private Animator animator;
    [SerializeField] private Animator animatorTop;

    private void Start()
    {
        blockCollider = GetComponent<Collider2D>();

        UpdateBlock(targetSwitch.IsOn);
        targetSwitch.OnStateChanged += UpdateBlock;
    }

    private void UpdateBlock(bool isOn)
    {
        bool shouldBeActive = blockColor == BlockColor.Red ? !isOn : isOn;

        if (animator != null)
        {
            animator.SetBool("cerrado", shouldBeActive);
            animatorTop.SetBool("cerrado", shouldBeActive);
        }

        if (blockCollider != null)
        {
            blockCollider.enabled = shouldBeActive;
        }
    }
}
