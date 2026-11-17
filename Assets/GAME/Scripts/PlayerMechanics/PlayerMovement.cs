using DG.Tweening;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private int pixelsPerUnit = 16;

    private PlayerAnimation playerAnimation;

    public bool IsMoving { get; private set; }
    private bool isMovingWithTween;

    public bool IsPushing { get; private set; }

    private Vector2 collisionBoxSize = new Vector2(0.8f, 0.8f);
    private Vector2 collisionBoxOffset = Vector2.zero;

    private readonly Collider2D[] overlapResults = new Collider2D[32];

    private Vector2 movementAccumulator;

    public Vector2 FacingDirection { get; private set; } = Vector2.down;

    private Rigidbody2D rb;

    private Vector2 moveInput;

    private float PixelSize => 1f / pixelsPerUnit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    private void FixedUpdate()
    {
        if (isMovingWithTween)
            return;

        Move();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;

        if (IsPushing)
        {
            movementAccumulator = Vector2.zero;

            playerAnimation.UpdateLocomotion(IsMoving, IsPushing, FacingDirection);

            return;
        }

        IsMoving = input != Vector2.zero;

        if (!IsMoving)
            movementAccumulator = Vector2.zero;

        if (IsMoving)
            UpdateFacingDirection(input);

        playerAnimation.UpdateLocomotion(IsMoving, IsPushing, FacingDirection);
    }

    public void Move()
    {
        if (IsPushing)
            return;

        movementAccumulator += moveInput * speed * Time.fixedDeltaTime;

        Vector2 pixelMovement = new Vector2(
            Mathf.Round(movementAccumulator.x / PixelSize),
            Mathf.Round(movementAccumulator.y / PixelSize)
        );

        Vector2 target = rb.position + pixelMovement * PixelSize;

        movementAccumulator -= pixelMovement * PixelSize;

        rb.MovePosition(target);
    }

    public void MovePlayer(int tiles, float duration, Vector2 direction, Action onComplete = null)
    {
        IsMoving = true;
        isMovingWithTween = true;
        movementAccumulator = Vector2.zero;

        playerAnimation.UpdateLocomotion(IsMoving, IsPushing, FacingDirection);

        Vector2 startPosition = SnapToPixel(rb.position);
        Vector2 target = startPosition + direction * tiles;

        rb.position = startPosition;

        rb.DOMove(target, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                rb.position = target;

                IsMoving = false;
                isMovingWithTween = false;
                movementAccumulator = Vector2.zero;

                playerAnimation.UpdateLocomotion(
                    IsMoving,
                    IsPushing,
                    FacingDirection
                );

                onComplete?.Invoke();
            });
    }

    private Vector2 SnapToPixel(Vector2 position)
    {
        float pixelSize = 1f / pixelsPerUnit;

        position.x = Mathf.Round(position.x / pixelSize) * pixelSize;
        position.y = Mathf.Round(position.y / pixelSize) * pixelSize;

        return position;
    }

    public void AlignToObject(Transform targetObject, Vector2 direction, float duration, Action onComplete)
    {
        isMovingWithTween = true;
        movementAccumulator = Vector2.zero;

        Vector2 targetPosition = rb.position;
        Vector2 objectPosition = targetObject.position;

        if (direction == Vector2.up || direction == Vector2.down)
        {
            targetPosition.x = objectPosition.x;
        }
        else
        {
            targetPosition.y = objectPosition.y;
        }

        targetPosition = SnapToPixel(targetPosition);

        rb.DOMove(targetPosition, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                rb.position = SnapToPixel(rb.position);

                isMovingWithTween = false;
                movementAccumulator = Vector2.zero;

                onComplete?.Invoke();
            });
    }

    public bool CanMove(Vector2 direction, float distance, Transform ignoredObject)
    {
        Vector2 targetPosition = rb.position + direction * distance;

        int count = Physics2D.OverlapBox(
            targetPosition + collisionBoxOffset,
            collisionBoxSize,
            rb.rotation,
            new ContactFilter2D
            {
                useTriggers = true
            },
            overlapResults
        );

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = overlapResults[i];

            if (hit == null)
                continue;

            if (hit.transform == transform ||
                hit.transform.IsChildOf(transform))
                continue;

            if (hit.transform == ignoredObject ||
                hit.transform.IsChildOf(ignoredObject))
                continue;

            if (hit.GetComponent<PressurePlate>() != null)
                continue;

            return false;
        }

        return true;
    }

    public void UpdateFacingDirection(Vector2 input)
    {
        if (input == Vector2.zero || IsPushing) return;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) { FacingDirection = input.x > 0 ? Vector2.right : Vector2.left; }
        else { FacingDirection = input.y > 0 ? Vector2.up : Vector2.down; }
    }

    public void SetPushing(bool pushing)
    {
        IsPushing = pushing;

        playerAnimation.UpdateLocomotion( IsMoving, IsPushing, FacingDirection);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetFacingDirection(Vector2 direction)
    {
        if (IsPushing) return;

        UpdateFacingDirection(direction);
    }
}
