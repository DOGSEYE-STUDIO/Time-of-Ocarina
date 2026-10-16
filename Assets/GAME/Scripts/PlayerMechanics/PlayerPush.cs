
using UnityEngine;
using DG.Tweening;

public class PlayerPush : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private float pushHoldDelay = 0.25f;
    [SerializeField] private float pushDuration = 0.4f;

    private int pixelsPerTile = 16;

    [Header("Placeholder")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Color baseColor = Color.white;
    [SerializeField] private Color touchingColor = Color.yellow;
    [SerializeField] private Color pushingColor = Color.blue;

    private PlayerController player;
    private PlayerMovement movement;
    private PlayerInteraction interaction;

    private PushableObject targetObject;

    private float holdTimer;
    private bool isPushing;

    public bool IsPushing => isPushing;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        movement = GetComponent<PlayerMovement>();
        interaction = GetComponent<PlayerInteraction>();
    }

    private void Update()
    {
        if (isPushing)
        {
            UpdatePlayerColor();
            return;
        }

        UpdatePushTarget();

        if (targetObject == null)
        {
            holdTimer = 0f;
            UpdatePlayerColor();
            return;
        }

        Vector2 facing = movement.FacingDirection;

        if (!player.IsHoldingDirection(facing))
        {
            holdTimer = 0f;
            UpdatePlayerColor();
            return;
        }

        holdTimer += Time.deltaTime;

        UpdatePlayerColor();

        if (holdTimer >= pushHoldDelay)
        {
            holdTimer = 0f;
            StartPush(facing);
            UpdatePlayerColor();
        }
    }

    private void UpdatePushTarget()
    {
        APlayerInteractable interactable = interaction.hitbox.interactable;
        targetObject = interactable as PushableObject;
    }


    private void StartPush(Vector2 direction)
    {
        if (targetObject == null || isPushing)
            return;

        PushableObject currentObject = targetObject;

        float tileSize = pixelsPerTile / 16f;

        // Comprobamos si el objeto puede avanzar un tile.
        if (!currentObject.CanPush(direction, tileSize))
        {
            holdTimer = 0f;
            return;
        }

        // Solo iniciamos el empuje si el destino está libre.
        isPushing = true;

        player.MovePlayer(1, pushDuration, direction);

        currentObject.Push(direction, tileSize, pushDuration, () =>
        {
            isPushing = false;
            holdTimer = 0f;

            UpdatePlayerColor();
        });
    }

    public void CancelPush()
    {
        holdTimer = 0f;
    }

    private void UpdatePlayerColor()
    {
        if (playerSprite == null)
            return;

        if (isPushing)
        {
            playerSprite.color = pushingColor;
        }
        else if (movement.IsBlocked ||
                 (targetObject != null &&
                  player.IsHoldingDirection(movement.FacingDirection)))
        {
            playerSprite.color = touchingColor;
        }
        else
        {
            playerSprite.color = baseColor;
        }
    }
}
