using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private float pushDuration = 0.4f;

    private int pixelsPerTile = 16;

    private PlayerController player;
    private PlayerMovement movement;

    private PushableObject targetObject;

    private bool isPushing;
    private bool isMovingObject;
    private bool wantsToRelease;

    private bool playerMoveFinished;
    private bool objectMoveFinished;

    private float alignDuration = 0.1f;

    private Vector2 pushDirection;

    private bool isAligning;

    public bool IsPushing => isPushing;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (!isPushing)
            return;

        Vector2 input = player.GetMovementInput();

        if (isMovingObject)
            return;

        if (isAligning)
            return;

        Vector2 direction = GetPushDirection(input);

        if (direction == Vector2.zero)
            return;

        PushObject(direction);
    }

    private Vector2 GetPushDirection(Vector2 input)
    {
        if (input == Vector2.zero)
            return Vector2.zero;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            return input.x > 0 ? Vector2.right : Vector2.left;

        return input.y > 0 ? Vector2.up : Vector2.down;
    }

    public void TryGrab(PushableObject pushable)
    {
        if (isPushing || pushable == null)
            return;

        pushDirection = movement.FacingDirection;

        targetObject = pushable;
        isPushing = true;
        wantsToRelease = false;
        isAligning = true;

        movement.SetPushing(true);

        movement.AlignToObject(
            targetObject.transform,
            pushDirection,
            alignDuration,
            () =>
            {
                isAligning = false;

                if (wantsToRelease)
                    FinishRelease();
            }
        );
    }

    private void PushObject(Vector2 direction)
    {
        if (targetObject == null || isMovingObject)
            return;

        float tileSize = pixelsPerTile / 16f;

        if (!movement.CanMove(direction, tileSize, targetObject.transform))
            return;

        if (!targetObject.CanPush(direction, tileSize))
            return;

        isMovingObject = true;
        playerMoveFinished = false;
        objectMoveFinished = false;

        movement.MovePlayer(1, pushDuration, direction, () =>
        {
            playerMoveFinished = true;
            CheckMovementFinished();
        });

        targetObject.Push(direction, tileSize, pushDuration, () =>
        {
            objectMoveFinished = true;
            CheckMovementFinished();
        });
    }

    private void CheckMovementFinished()
    {
        if (!playerMoveFinished || !objectMoveFinished)
            return;

        isMovingObject = false;

        if (wantsToRelease)
        {
            FinishRelease();
        }
    }

    public void ReleaseObject()
    {
        if (!isPushing)
            return;

        if (isMovingObject || isAligning)
        {
            wantsToRelease = true;
            return;
        }

        FinishRelease();
    }

    private void FinishRelease()
    {
        isPushing = false;
        wantsToRelease = false;
        targetObject = null;

        movement.SetPushing(false);
    }
}