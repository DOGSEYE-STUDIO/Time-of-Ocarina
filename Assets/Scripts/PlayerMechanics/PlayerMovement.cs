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

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 oldinput;
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

        oldinput = input;

        input = moveAction.action.ReadValue<Vector2>().normalized;

        if (input != oldinput) 
        {

            if (input.y < 0)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = spriteDown;
            }

            if (input.y > 0)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = spriteUp;
            }

            if (input.x < 0)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = spriteLeft;
            }

            if (input.x > 0)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = spriteRight;
            }

            //gameObject.GetComponent<SpriteRenderer>().sprite = spriteLeft;
        }

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
