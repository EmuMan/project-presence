using UnityEngine;

public class JumpingEnemy : MonoBehaviour
{
    public Transform GroundCheck;
    public LayerMask ground;

    private Enemy enemy;
    private Rigidbody rb;

    public float jumpVerticalAmount = 5f;
    public float jumpHorizontalAmount = 2f;
    public float jumpCooldown = 0.5f;

    public float timeOfNextJump;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody>();

        timeOfNextJump = Time.time;
    }


    void FixedUpdate()
    {
        Transform playerTransform = enemy.GetTarget();

        if (playerTransform != null)
        {
            Debug.DrawLine(transform.position, playerTransform.position, Color.red, 0.1f);
        }
        bool grounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        //change this to check if the object is touching the ground
        if (enemy.canAct && playerTransform != null && grounded && Time.time >= timeOfNextJump)
        {
            JumpTowardPlayer(playerTransform);
            timeOfNextJump = Time.time + jumpCooldown;
        }
    }

    void JumpTowardPlayer(Transform player)
    {
        //direction for enemy to move towards
        Vector3 toPlayer = (player.position - transform.position);
        toPlayer.y = 0f;    //reset y

        Vector3 dir = toPlayer.normalized;

        if (toPlayer.sqrMagnitude < 0.0001f)
        {
            return;
        }

        //Vector3 velocity = rb.linearVelocity;
        //velocity.y = 0f;
        //rb.linearVelocity = velocity;
        rb.linearVelocity = Vector3.zero;

        Vector3 jumpVector = dir * jumpHorizontalAmount + Vector3.up * jumpVerticalAmount;

        rb.AddForce(jumpVector, ForceMode.Impulse);

        //movement
        //rb.AddForce((toPlayer * 0.005f) + Vector3.up * jumpVerticalAmount, ForceMode.Impulse);
    }


}

/*
using UnityEngine;

public class JumpingEnemy : MonoBehaviour
{
    public Transform player;

    [Header("Jump")]
    public float jumpUpForce = 6f;
    public float jumpForwardForce = 8f;
    public float jumpCooldown = 1.5f;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask = ~0;

    private Rigidbody rb;
    private float nextJumpTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Time.time < nextJumpTime) return;
        if (!IsGrounded()) return;

        JumpTowardPlayer();
        nextJumpTime = Time.time + jumpCooldown;
    }

    void JumpTowardPlayer()
    {
        Vector3 toPlayer = (player.position - transform.position);
        toPlayer.y = 0f;
        Vector3 dir = toPlayer.sqrMagnitude > 0.001f ? toPlayer.normalized : transform.forward;

        // Optional: face the target before jumping
        transform.rotation = Quaternion.LookRotation(dir);

        // Clear existing vertical speed so jumps feel consistent
        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        Vector3 impulse = dir * jumpForwardForce + Vector3.up * jumpUpForce;
        rb.AddForce(impulse, ForceMode.Impulse);
    }

    bool IsGrounded()
    {
        // Raycast just below the collider; adjust distance if needed
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.1f + groundCheckDistance, groundMask);
    }
}
*/
