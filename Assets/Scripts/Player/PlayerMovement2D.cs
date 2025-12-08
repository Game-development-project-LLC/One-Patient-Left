using UnityEngine;

/// <summary>
/// Simple 2D top-down movement with a toggle between normal and slow speed.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float normalSpeed = 4f;
    [SerializeField] private float slowSpeed = 2f;

    [Header("Input Settings")]
    [SerializeField] private string horizontalAxisName = "Horizontal";
    [SerializeField] private string verticalAxisName = "Vertical";
    [SerializeField] private KeyCode slowToggleKey = KeyCode.Z;

    private Rigidbody2D rb;
    private Vector2 inputDirection = Vector2.zero;
    private float currentSpeed;
    private bool useSlowSpeed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = normalSpeed;
    }

    private void Update()
    {
        ReadMovementInput();
        HandleSpeedToggle();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    /// <summary>
    /// Reads WASD / arrow input axes and normalizes the direction.
    /// </summary>
    private void ReadMovementInput()
    {
        float horizontal = Input.GetAxisRaw(horizontalAxisName);
        float vertical = Input.GetAxisRaw(verticalAxisName);

        inputDirection = new Vector2(horizontal, vertical).normalized;
    }

    /// <summary>
    /// Toggles between normal and slow speed when the toggle key is pressed.
    /// </summary>
    private void HandleSpeedToggle()
    {
        if (Input.GetKeyDown(slowToggleKey))
        {
            useSlowSpeed = !useSlowSpeed;
        }

        currentSpeed = useSlowSpeed ? slowSpeed : normalSpeed;
    }

    /// <summary>
    /// Moves the character using Rigidbody2D.MovePosition for smooth motion.
    /// </summary>
    private void MoveCharacter()
    {
        if (inputDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        Vector2 targetPosition =
            rb.position + inputDirection * currentSpeed * Time.fixedDeltaTime;

        rb.MovePosition(targetPosition);
    }

    // Kept for compatibility with other scripts
    public float getNormalSpeed() => normalSpeed;
    public float getSlowSpeed() => slowSpeed;

    public bool IsUsingSlowSpeed => useSlowSpeed;
    public float CurrentSpeed => currentSpeed;
}
