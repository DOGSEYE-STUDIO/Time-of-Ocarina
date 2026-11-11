using DG.Tweening;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private int pixelsPerUnit = 16;

    [SerializeField] private Sprite spriteUp;
    [SerializeField] private Sprite spriteLeft;
    [SerializeField] private Sprite spriteDown;
    [SerializeField] private Sprite spriteRight;

    public bool IsMoving { get; private set; }
    private bool isMovingWithTween;

    public bool IsBlocked { get; private set; }

    private Vector2 positionBeforeMove;
    private bool wasTryingToMove;
    private const float BlockedThreshold = 0.0001f;
    private Vector2 movementAccumulator;

    public Vector2 FacingDirection { get; private set; } = Vector2.down;

    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector2 moveInput;
    private Vector2 startPosition;

    private float PixelSize => 1f / pixelsPerUnit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        startPosition = rb.position;
    }

    private void FixedUpdate()
    {
        if (isMovingWithTween)
        {
            IsBlocked = false;
            wasTryingToMove = false;
            return;
        }

        if (wasTryingToMove)
        {
            float distanceMoved = Vector2.Distance(rb.position, positionBeforeMove);

            IsBlocked = moveInput != Vector2.zero && distanceMoved < BlockedThreshold && speed * Time.fixedDeltaTime >= PixelSize;
        }
        else
        {
            IsBlocked = false;
        }

        positionBeforeMove = rb.position;
        wasTryingToMove = moveInput != Vector2.zero;

        Move();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;

        IsMoving = input != Vector2.zero;

        if (!IsMoving)
        {
            movementAccumulator = Vector2.zero;
            IsBlocked = false;
            wasTryingToMove = false;
        }

        if (IsMoving)
        {
            UpdateFacingDirection(input);
        }
    }

    public void Move()
    {
        movementAccumulator += moveInput * speed * Time.fixedDeltaTime;

        Vector2 pixelMovement = new Vector2(
            Mathf.Round(movementAccumulator.x / PixelSize),
            Mathf.Round(movementAccumulator.y / PixelSize)
        );

        Vector2 target = rb.position + pixelMovement * PixelSize;

        movementAccumulator -= pixelMovement * PixelSize;

        rb.MovePosition(target);
    }

    public void MovePlayer(int tiles, float duration, Vector2 direction)
    {
        IsMoving = true;
        isMovingWithTween = true;
        movementAccumulator = Vector2.zero;

        PlayerController.Instance.StopPlayer();

        Vector2 tileMovement = direction * tiles;

        Vector2 target = rb.position + tileMovement;

        Sequence pushSequence = DOTween.Sequence();
        
        pushSequence.Join(rb.DOMove(target, duration).SetEase(Ease.Linear));

        pushSequence.OnComplete(() =>
        {
            IsMoving = false;
            isMovingWithTween = false;
            movementAccumulator = Vector2.zero;

            PlayerController.Instance.ResumePlayer();
        });
    }

    public void UpdateFacingDirection(Vector2 input)
    {
        if (input == Vector2.zero) return;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) { FacingDirection = input.x > 0 ? Vector2.right : Vector2.left; }
        else { FacingDirection = input.y > 0 ? Vector2.up : Vector2.down; }

        UpdateSprite();
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

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public float GetSpeed()
    {
        return speed;
    }
}
