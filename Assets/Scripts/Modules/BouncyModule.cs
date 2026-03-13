using UnityEngine;

public class BouncyModule : Module
{
    [Header("Jump Settings")]
    public float jumpStrength = 10.0f;
    public float jumpCoyoteTime = 0.2f;

    private float timeSinceLastGrounded = 0.0f;
    private bool canJump = true;

    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = playerObject.GetComponent<PlayerMovement>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (playerMovement.IsGrounded())
        {
            timeSinceLastGrounded = 0.0f;
            canJump = true;
        }
        else
        {
            timeSinceLastGrounded += Time.fixedDeltaTime;
        }

        if (timeSinceLastGrounded > jumpCoyoteTime)
        {
            canJump = false;
        }
    }

    protected override void PerformAction(Vector3 direction)
    {
        playerMovement.velocity.y = jumpStrength;
    }

    public override bool CanPerformAction()
    {
        return canJump;
    }
}
