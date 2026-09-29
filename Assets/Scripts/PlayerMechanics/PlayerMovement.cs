using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 64f;
    [SerializeField] private int pixelsPerUnit = 16;

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 startPosition;

    private float PixelSize => 1f / pixelsPerUnit;

    public bool canMove = true;

    public InputActionReference moveAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;
    }

    private void Update()
    {
        if (!canMove) return;

        input = moveAction.action.ReadValue<Vector2>().normalized;
    }

    private void FixedUpdate()
    {
        Vector2 movement = input * (speed / pixelsPerUnit) * Time.fixedDeltaTime;
        Vector2 target = rb.position + movement;

        target.x = startPosition.x + Mathf.Round((target.x - startPosition.x) / PixelSize) * PixelSize;
        target.y = startPosition.y + Mathf.Round((target.y - startPosition.y) / PixelSize) * PixelSize;

        rb.MovePosition(target);

    }
}
