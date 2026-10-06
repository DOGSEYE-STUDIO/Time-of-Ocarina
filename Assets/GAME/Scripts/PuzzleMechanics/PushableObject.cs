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

        PlayerMovement playerMovement =
            PlayerController.Instance.GetComponent<PlayerMovement>();

        Rigidbody2D playerRb =
            PlayerController.Instance.GetComponent<Rigidbody2D>();

        Vector2 direction = playerMovement.FacingDirection;

        Vector2 tileMovement = direction * (pixelsPerTile / 16f);

        Vector2 playerTarget = playerRb.position + tileMovement;
        Vector2 objectTarget = rb.position + tileMovement;

        Sequence pushSequence = DOTween.Sequence();

        pushSequence.Join(
            playerRb
                .DOMove(playerTarget, pushDuration)
                .SetEase(Ease.Linear)
        );

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