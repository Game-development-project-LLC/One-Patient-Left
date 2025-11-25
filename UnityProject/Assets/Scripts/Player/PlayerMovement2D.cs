using UnityEngine;

/// <summary>
/// Simple top-down movement using WASD / Arrow keys.
/// Press Z to toggle slow walking on/off.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float normalMoveSpeed = 4f;
    public float slowMoveSpeed = 2f;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private bool isSlowMode = false;   // if true -> use slowMoveSpeed

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Movement input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(moveX, moveY).normalized;

        // Toggle slow walk with Z
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isSlowMode = !isSlowMode;
            Debug.Log("Slow walk mode: " + (isSlowMode ? "ON" : "OFF"));
        }
    }

    private void FixedUpdate()
    {
        float currentSpeed = isSlowMode ? slowMoveSpeed : normalMoveSpeed;
        rb.MovePosition(rb.position + movementInput * currentSpeed * Time.fixedDeltaTime);
    }
}
