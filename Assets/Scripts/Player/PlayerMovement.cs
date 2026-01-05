using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player movement (New Input System).
/// Move: Vector2
/// SlowWalk: Button (hold)
///
/// This script also drives the Animator parameters:
/// - MoveX (float) : last facing/move direction X in range [-1..1]
/// - MoveY (float) : last facing/move direction Y in range [-1..1]
/// - Speed (float) : movement amount (0 when idle)
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

    [Header("Animation (optional)")]
    [Tooltip("If empty, the script will try to find Animator on this GameObject.")]
    [SerializeField] private Animator animator;

    [Tooltip("Animator float parameter name for facing/move X.")]
    [SerializeField] private string moveXParam = "MoveX";

    [Tooltip("Animator float parameter name for facing/move Y.")]
    [SerializeField] private string moveYParam = "MoveY";

    [Tooltip("Animator float parameter name for speed (0=idle).")]
    [SerializeField] private string speedParam = "Speed";

    private Rigidbody2D rb;

    // Raw input (for Speed). Normalized direction (for MoveX/MoveY)
    private Vector2 rawInput = Vector2.zero;
    private Vector2 inputDirection = Vector2.zero;

    // Remember last non-zero direction so idle faces the last direction.
    private Vector2 lastMoveDir = Vector2.down;

    private float currentSpeed;
    private bool isSlowHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = normalSpeed;

        if (animator == null)
            animator = GetComponent<Animator>();
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
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void ReadInput()
    {
        if (moveAction == null)
        {
            rawInput = Vector2.zero;
            inputDirection = Vector2.zero;
            return;
        }

        rawInput = moveAction.action.ReadValue<Vector2>();
        inputDirection = rawInput.sqrMagnitude > 0f ? rawInput.normalized : Vector2.zero;

        if (inputDirection.sqrMagnitude > 0f)
            lastMoveDir = inputDirection;
    }

    private void UpdateSpeed()
    {
        isSlowHeld = slowWalkAction != null && slowWalkAction.action.IsPressed();
        currentSpeed = isSlowHeld ? slowSpeed : normalSpeed;
    }

    private void MoveCharacter()
    {
        // If no input, don't move (rb stays).
        if (inputDirection.sqrMagnitude <= 0f)
            return;

        Vector2 targetPosition = rb.position + inputDirection * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        // Speed is 0 when idle, >0 when moving.
        float speed01 = rawInput.sqrMagnitude > 0f ? 1f : 0f;

        // While moving: face movement direction. While idle: face last direction.
        Vector2 dir = speed01 > 0f ? inputDirection : lastMoveDir;

        animator.SetFloat(moveXParam, dir.x);
        animator.SetFloat(moveYParam, dir.y);
        animator.SetFloat(speedParam, speed01);
    }

    // Kept for compatibility with other scripts
    public float getNormalSpeed() => normalSpeed;
    public float getSlowSpeed() => slowSpeed;

    public bool IsUsingSlowSpeed => isSlowHeld;
    public float CurrentSpeed => currentSpeed;
}
