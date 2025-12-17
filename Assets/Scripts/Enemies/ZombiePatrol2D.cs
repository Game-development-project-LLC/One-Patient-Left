using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ZombiePatrol2D : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float arriveDistance = 0.1f;
    [SerializeField] private bool loop = true;

    private Rigidbody2D rb;
    private int index = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void FixedUpdate()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Vector2 target = waypoints[index].position;
        Vector2 pos = rb.position;
        Vector2 dir = (target - pos);

        if (dir.magnitude <= arriveDistance)
        {
            index++;
            if (index >= waypoints.Length)
            {
                index = loop ? 0 : waypoints.Length - 1;
            }
            return;
        }

        rb.MovePosition(pos + dir.normalized * speed * Time.fixedDeltaTime);
    }
}
