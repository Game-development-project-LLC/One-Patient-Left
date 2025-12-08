using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float normalSpeed = 4f;
    [SerializeField] private float slowSpeed = 2f;

    [Header("Input Settings")]
    [SerializeField] private string horizontalAxisName = "Horizontal";
    [SerializeField] private string verticalAxisName = "Vertical";
    [SerializeField] private KeyCode slowToggleKey = KeyCode.Z; // כפתור להחלפת מהירות

    private Rigidbody2D rb;
    private Vector2 inputDirection = Vector2.zero;
    private bool useSlowSpeed = false;
    private float currentSpeed;

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

    private void ReadMovementInput()
    {
        float horizontal = Input.GetAxisRaw(horizontalAxisName);
        float vertical = Input.GetAxisRaw(verticalAxisName);

        inputDirection = new Vector2(horizontal, vertical).normalized;
    }

    private void HandleSpeedToggle()
    {
        if (Input.GetKeyDown(slowToggleKey))
        {
            useSlowSpeed = !useSlowSpeed;
        }

        currentSpeed = useSlowSpeed ? slowSpeed : normalSpeed;
    }

    private void MoveCharacter()
    {
        Vector2 targetPosition =
            rb.position + inputDirection * currentSpeed * Time.fixedDeltaTime;

        rb.MovePosition(targetPosition);
    }

    public float getNormalSpeed()
    {
        return normalSpeed;
    }

    public float getSlowSpeed()
    {
        return slowSpeed;
    }


    public bool IsUsingSlowSpeed => useSlowSpeed;
    public float CurrentSpeed => currentSpeed;
}
