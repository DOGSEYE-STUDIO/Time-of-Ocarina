using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 64f;
    [SerializeField] private int pixelsPerUnit = 16;

    [SerializeField] private Sprite spriteUp;
    [SerializeField] private Sprite spriteLeft;
    [SerializeField] private Sprite spriteDown;
    [SerializeField] private Sprite spriteRight;

    public bool IsMoving { get; private set; }

    public Vector2 FacingDirection { get; private set; } = Vector2.down;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveInput;
    private Vector2 startPosition;


    private float PixelSize => 1f / pixelsPerUnit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        startPosition = rb.position;
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;

        IsMoving = input != Vector2.zero;

        if (IsMoving)
        {
            UpdateFacingDirection(input);
            UpdateSprite();
        }
    }

    public void Move()
    {
        Vector2 movement = moveInput * (speed / pixelsPerUnit) * Time.fixedDeltaTime;
        Vector2 target = rb.position + movement;

        target.x = startPosition.x + Mathf.Round((target.x - startPosition.x) / PixelSize) * PixelSize;
        target.y = startPosition.y + Mathf.Round((target.y - startPosition.y) / PixelSize) * PixelSize;

        rb.MovePosition(target);

    }

    private void UpdateFacingDirection(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) { FacingDirection = input.x > 0 ? Vector2.right : Vector2.left; }
        else { FacingDirection = input.y > 0 ? Vector2.up : Vector2.down; }
    }

    private void UpdateSprite()
    {
        if (FacingDirection == Vector2.up)
        {
            spriteRenderer.sprite = spriteUp;
        }
        else if (FacingDirection == Vector2.down)
        {
            spriteRenderer.sprite = spriteDown;
        }
        else if (FacingDirection == Vector2.left)
        {
            spriteRenderer.sprite = spriteLeft;
        }
        else if (FacingDirection == Vector2.right)
        {
            spriteRenderer.sprite = spriteRight;
        }
    }
}
