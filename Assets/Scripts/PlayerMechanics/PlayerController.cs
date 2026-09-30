using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;

    [Header("InputActions")]
    public InputActionReference moveAction;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();

        // INPUTS
        moveAction.action.performed += OnMove;
    }

    // MOVEMENT
    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        moveAction.action.performed += OnMove;
        moveAction.action.canceled += OnMove;

        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.performed -= OnMove;
        moveAction.action.canceled -= OnMove;

        moveAction.action.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        movement.SetMoveInput(value);
    }

    public void StopPlayerMovement()
    {
        movement.SetMoveInput(Vector2.zero);

        moveAction.action.performed -= OnMove;
        moveAction.action.canceled -= OnMove;
    }

    public void ResumePlayerMovement()
    {
        moveAction.action.performed += OnMove;
        moveAction.action.canceled += OnMove;
    }

    public bool isPlayerMoving() { return movement.IsMoving; }

    // ATTACK
}
