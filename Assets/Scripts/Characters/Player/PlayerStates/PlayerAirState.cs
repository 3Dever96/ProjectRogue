using UnityEngine;

/// <summary>
/// State handler for when the player is airborne (falling, jumping, or double-jumping).
/// </summary>
[System.Serializable]
public class PlayerAirState : PlayerState
{
    [Header("Movement Settings")]
    [SerializeField] private float airMoveSpeed;
    [SerializeField] private float doubleJumpSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float fallSpeed;

    [Header("Collision Settings")]
    [SerializeField] private Vector3 headOffset;
    [SerializeField] private Vector3 groundOffset;
    [SerializeField] private float collisionRadius;

    private bool canDoubleJump;
    private bool canJump;

    public override void StartState(PlayerController player)
    {
        canDoubleJump = true;
        canJump = false; // Prevents accidental instant double-jumps if button is held

        player.Animator.SetBool("IsGrounded", false);
    }

    public override void UpdateState(PlayerController player)
    {
        // Stop upward momentum if player hits their head
        if (!InputHub.Instance.Jump || player.CheckCollitions(headOffset, collisionRadius))
        {
            player.VerticalSpeed = Mathf.Min(0f, player.VerticalSpeed);
        }

        // Handle Double Jump Execution
        if (canDoubleJump)
        {
            if (InputHub.Instance.Jump && canJump)
            {
                player.MoveDirection = player.GetInputDirection();
                player.LookDirection = player.MoveDirection != Vector3.zero ? player.MoveDirection : player.LookDirection;
                player.CurrentSpeed = airMoveSpeed;

                player.VerticalSpeed = doubleJumpSpeed;
                canDoubleJump = false;
            }

            // Must release jump button before being allowed to double jump
            canJump = !InputHub.Instance.Jump;
        }

        player.FaceDirection();

        // Apply downward gravity acceleration
        if (player.VerticalSpeed > fallSpeed)
        {
            player.VerticalSpeed += gravity * Time.deltaTime;
        }

        player.Animator.SetFloat("VerticalSpeed", player.VerticalSpeed);
    }

    public override void ChangeState(PlayerController player)
    {
        // Transition back to ground when descending and touching a solid layer
        if (player.VerticalSpeed <= 0f && player.CheckCollitions(groundOffset, collisionRadius))
        {
            player.SetState(player.GroundState);
        }
    }

    public override void ExitState(PlayerController player)
    {
        // Intentional cleanup placeholder
    }
}
