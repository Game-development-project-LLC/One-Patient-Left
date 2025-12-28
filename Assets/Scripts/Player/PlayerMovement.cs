using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// player movement
/// Move: Vector2
/// SlowWalk: Button (hold)
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float normalSpeed = 4f;
    [SerializeField] private float slowSpeed = 2f;

    [Header("Input Actions (New Input System)")]
    [Tooltip("Action type: Value (Vector2). Example binding: 2D Vector composite (WASD / Arrows).")]
    [SerializeField] private InputActionReference moveAction;

    [Tooltip("Action type: Button. Example binding: Left Ctrl / Right Ctrl). Hold to slow-walk.")]
    [SerializeField] private InputActionReference slowWalkAction;

    private Rigidbody2D rb;
    private Vector2 inputDirection = Vector2.zero;
    private float currentSpeed;
    private bool isSlowHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = normalSpeed;
    }

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
        if (slowWalkAction != null) slowWalkAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
        if (slowWalkAction != null) slowWalkAction.action.Disable();
    }

    private void Update()
    {
        ReadInput();
        UpdateSpeed();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void ReadInput()
    {
        if (moveAction == null)
        {
            inputDirection = Vector2.zero;
            return;
        }

        Vector2 raw = moveAction.action.ReadValue<Vector2>();
        inputDirection = raw.sqrMagnitude > 0f ? raw.normalized : Vector2.zero;
    }

    private void UpdateSpeed()
    {
        isSlowHeld = slowWalkAction != null && slowWalkAction.action.IsPressed();
        currentSpeed = isSlowHeld ? slowSpeed : normalSpeed;
    }

    private void MoveCharacter()
    {
        if (inputDirection.sqrMagnitude <= 0f)
            return;

        Vector2 targetPosition = rb.position + inputDirection * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);
    }

    // Kept for compatibility with other scripts
    public float getNormalSpeed() => normalSpeed;
    public float getSlowSpeed() => slowSpeed;

    public bool IsUsingSlowSpeed => isSlowHeld;
    public float CurrentSpeed => currentSpeed;
}
