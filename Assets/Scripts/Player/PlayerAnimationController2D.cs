using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimationController2D : MonoBehaviour
{
    [Header("Optional")]
    [SerializeField] private PlayerStealthState stealthState;

    [Header("Tuning")]
    [SerializeField] private float idleThreshold = 0.05f;

    private Animator _anim;
    private Rigidbody2D _rb;

    // default facing down (feel free to change)
    private Vector2 _lastMoveDir = Vector2.down;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private static readonly int IsStealthHash = Animator.StringToHash("IsStealth");

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();

        if (stealthState == null)
            stealthState = GetComponent<PlayerStealthState>();
    }

    private void Update()
    {
        Vector2 v = _rb.linearVelocity;
        float speed = v.magnitude;

        _anim.SetFloat(SpeedHash, speed);

        // update facing direction only when we actually move
        if (speed > idleThreshold)
        {
            Vector2 dir = v.normalized;
            _lastMoveDir = dir;

            _anim.SetFloat(MoveXHash, dir.x);
            _anim.SetFloat(MoveYHash, dir.y);
        }
        else
        {
            // keep last direction for idle facing
            _anim.SetFloat(MoveXHash, _lastMoveDir.x);
            _anim.SetFloat(MoveYHash, _lastMoveDir.y);
        }

        bool isStealth = stealthState != null && stealthState.IsStealth;
        _anim.SetBool(IsStealthHash, isStealth);
    }
}
