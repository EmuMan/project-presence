using Unity.Mathematics;
using UnityEngine;

public class HardJumpingEnemy : MonoBehaviour
{
    public Transform GroundCheck;
    public LayerMask ground;


    public Transform player;
    private Enemy enemy;
    private Rigidbody rb;

    public float jumpVerticalAmount = 8f;
    public float jumpHorizontalAmount = 5f;
    public float jumpCooldown = 0.5f;

    public float fallMultiplier = 45f;  //make the object fall faster

    public float heightLimit = 10;

    public float timeOfNextJump;
    private float yStart;
    private float startingHorizAmount;
    private bool hasHit = false;
    private float damage = 5.0f;


    void Awake()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody>();
        timeOfNextJump = Time.time;
        startingHorizAmount = jumpHorizontalAmount;
    }


    void FixedUpdate()
    {
        Transform playerTransform = enemy.GetTarget();

        if (playerTransform != null)
        {
            Debug.DrawLine(transform.position, playerTransform.position, Color.red, 0.1f);
        }
        bool grounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        //only jump if currently touching the ground
        //change this to check if the object is touching the ground
        if (enemy.canAct && playerTransform != null && grounded && Time.time >= timeOfNextJump)
        {
            yStart = transform.position.y;
            JumpTowardPlayer(playerTransform);
            timeOfNextJump = Time.time + jumpCooldown;
        }


        //set a height limit, increase jumping speed
        //currently going up (positive y velocity)
        else if (!grounded && transform.position.y >= yStart + heightLimit && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);   //stop going up
        }

        //increase falling force
        //currently falling (negative velocity)
        else if (!grounded && rb.linearVelocity.y < 0)
        {
            rb.AddForce(Vector3.down * fallMultiplier * rb.mass, ForceMode.Acceleration);

        }

        //if the horizontal jumping distance is bigger than the distance between enemy and player
        //then decrease horizontal jumping distance
        if (math.sqrt(math.pow(math.abs(player.position.x - transform.position.x), 2) + math.pow(math.abs(player.position.z - transform.position.z), 2)) <= 8f)
        {
            jumpHorizontalAmount = jumpHorizontalAmount / 2;
            //jumpHorizontalAmount = jumpHorizontalAmount - 1;
            if (jumpHorizontalAmount <= 0.1)
            {
                jumpHorizontalAmount = 0.5f;
            }
        }

        else if (math.sqrt(math.pow(math.abs(player.position.x - transform.position.x), 2) + math.pow(math.abs(player.position.z - transform.position.z), 2)) > 8f)
        {
            jumpHorizontalAmount = jumpHorizontalAmount * 2;
            //jumpHorizontalAmount = jumpHorizontalAmount + 1;
            if(jumpHorizontalAmount >= 10f)
            {
                jumpHorizontalAmount = 8f;
            }
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

        //reset the velocity
        rb.linearVelocity = Vector3.zero;

        Vector3 jumpVector = dir * jumpHorizontalAmount + Vector3.up * jumpVerticalAmount;

        rb.AddForce(jumpVector, ForceMode.Impulse);


    }

    private void OnHit(Collider other)
    {
        // If we already hit something, ignore further collisions to prevent multiple damage applications
        /*
        if (hasHit)
        {
            return;
        }

        hasHit = true;
        */
        if (other.CompareTag("Player") && other.gameObject.TryGetComponent(out Health health))
        {
            health.TakeDamage(damage); // Example damage value
        }
        //Destroy(gameObject);
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnHit(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnHit(other);
    }



}
