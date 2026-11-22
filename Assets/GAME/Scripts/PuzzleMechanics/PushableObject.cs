using UnityEngine;
using DG.Tweening;
using System;

public class PushableObject : APlayerInteractable
{
    private Rigidbody2D rb;
    private bool isMoving;

    private Vector2 collisionBoxSize = new Vector2(0.9f, 0.9f);
    private Vector2 collisionBoxOffset = Vector2.zero;

    private readonly Collider2D[] overlapResults = new Collider2D[32];

    private int pixelsPerUnit = 16;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override string ActionText { get { return ""; } }

    public override void OnPlayerInteract() {}

    public void Push(Vector2 direction, float distance, float duration, Action onComplete)
    {
        if (isMoving)
            return;

        isMoving = true;

        Vector2 startPosition = SnapToPixel(rb.position);
        Vector2 target = startPosition + direction * distance;

        rb.position = startPosition;

        rb.DOMove(target, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                rb.position = target;
                isMoving = false;

                onComplete?.Invoke();
            });
    }

    public bool CanPush(Vector2 direction, float distance)
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

            if (hit == null) continue;

            if (hit.transform == PlayerController.Instance.transform || hit.transform.IsChildOf(PlayerController.Instance.transform)) continue;

            if (hit.gameObject.GetComponent<PressurePlate>() != null || hit.gameObject.CompareTag("CameraTrigger")) continue;

            return false;
        }

        return true;
    }

    private Vector2 SnapToPixel(Vector2 position)
    {
        float pixelSize = 1f / pixelsPerUnit;

        position.x = Mathf.Round(position.x / pixelSize) * pixelSize;
        position.y = Mathf.Round(position.y / pixelSize) * pixelSize;

        return position;
    }
}