using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerAttack basicAttack;
    private PlayerInteraction interaction;

    [Header("InputActions")]
    public InputActionReference moveAction;
    public InputActionReference attackAction;

    public static PlayerController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        basicAttack = GetComponent<PlayerAttack>();
        interaction = GetComponent<PlayerInteraction>();
    }

    private void OnEnable()
    {
        moveAction.action.performed += OnMove;
        moveAction.action.canceled += OnMove;
        moveAction.action.Enable();

        attackAction.action.performed += OnAttack;
        attackAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.performed -= OnMove;
        moveAction.action.canceled -= OnMove;
        moveAction.action.Disable();

        attackAction.action.performed -= OnAttack;
        attackAction.action.Disable();
    }

    // MOVEMENT
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>().normalized;

        movement.SetMoveInput(value);
        interaction.UpdateFacingDirection(movement.FacingDirection);
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

        movement.SetMoveInput(moveAction.action.ReadValue<Vector2>());
    }

    public bool isPlayerMoving() { return movement.IsMoving; }

    // ATTACK
    private void OnAttack(InputAction.CallbackContext context)
    {
        Vector2 value = moveAction.action.ReadValue<Vector2>();
        movement.UpdateFacingDirection(value);

        basicAttack.Attack(movement.FacingDirection);
    }

    public void StopPlayerAttack()
    {
        attackAction.action.performed -= OnAttack;
    }

    public void ResumePlayerAttack()
    {
        attackAction.action.performed += OnAttack;
    }
}
