using UnityEngine;
using DG.Tweening;

public class PushableObject : APlayerInteractable
{
    [SerializeField] private float pushDuration = 0.4f;

    private Rigidbody2D rb;

    private int pixelsPerTile = 16;
    private bool isMoving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnPlayerInteract()
    {
        if (isMoving)
            return;

        MoveObject();
    }

    private void MoveObject()
    {
        isMoving = true;

        PlayerController.Instance.StopPlayer();

        Vector2 tileMovement = PlayerController.Instance.GetFacingDirection() * (pixelsPerTile / 16f);

        Vector2 objectTarget = rb.position + tileMovement;

        PlayerController.Instance.MovePlayer(1, pushDuration);

        Sequence pushSequence = DOTween.Sequence();

        pushSequence.Join(
            rb
                .DOMove(objectTarget, pushDuration)
                .SetEase(Ease.Linear)
        );

        pushSequence.OnComplete(() =>
        {
            isMoving = false;

            PlayerController.Instance.ResumePlayer();
        });
    }
}