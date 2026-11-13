using UnityEngine;

/// <summary>
/// Handles grounded movement, acceleration, deceleration, and jump behavior.
/// This state updates the player's speed, facing direction, and jump trigger while contact with the ground is maintained.
/// </summary>
[System.Serializable]
public class PlayerGroundState : PlayerState
{
    [Header("Movement Settings")]
    // Base movement speed while grounded.
    [SerializeField] private float baseMaxSpeed = 5f;

    // Extra speed added by agility-based scaling.
    [SerializeField] private float addSpeed = 7f;

    // Rate at which ground movement accelerates.
    [SerializeField] private float accel = 15f;

    // Rate at which the player slows down while turning or stopping.
    [SerializeField] private float decel = 20f;

    // Friction applied when no movement input is provided.
    [SerializeField] private float fric = 10f;

    // Angle threshold used to decide whether the player should rotate before moving.
    [SerializeField] private float turnAngle = 90f;

    // Current calculated movement speed for this frame.
    private float moveSpeed;

    [Header("Vertical Settings")]
    // Downward force applied while grounded to keep the player anchored to the surface.
    [SerializeField] private float stickForce = -5f;

    // Base vertical jump speed.
    [SerializeField] private float baseJumpSpeed = 7f;

    // Extra jump velocity added based on agility.
    [SerializeField] private float addJumpSpeed = 5f;

    // Prevents repeated jump triggers while the jump button remains held.
    private bool canJump;

    public override void StartState(PlayerController player)
    {
        canAttack = true;

        // Keep the player grounded when entering this state.
        player.VerticalSpeed = stickForce;

        // Reset the jump latch so the next press can trigger a jump.
        canJump = false;

        player.Anim.SetBool("IsGrounded", true);
    }

    public override void UpdateState(PlayerController player)
    {
        // Movement speed scales with agility so faster characters move faster.
        float maxSpeed = baseMaxSpeed + (addSpeed * (player.Stats.stats["AGI"].Value / player.Stats.stats["MAX_AGI"].Value));
        moveSpeed = maxSpeed * InputHub.Instance.Move.magnitude;

        Vector3 direction = player.GetDirection();

        if (direction != Vector3.zero)
        {
            // If the input direction differs significantly from the current facing direction, reduce speed and rotate.
            if (Vector3.Angle(direction, player.LookDirection) > turnAngle)
            {
                player.CurrentSpeed -= decel * Time.deltaTime;

                if (player.CurrentSpeed <= 0f)
                {
                    player.CurrentSpeed = 0f;
                    player.LookDirection = direction;
                }
            }
            else
            {
                // Smoothly accelerate toward the desired movement speed.
                if (player.CurrentSpeed < moveSpeed)
                {
                    player.CurrentSpeed += accel * Time.deltaTime;
                }
                else if (player.CurrentSpeed > moveSpeed + 0.1f)
                {
                    player.CurrentSpeed -= accel * Time.deltaTime;
                }
                else
                {
                    player.CurrentSpeed = moveSpeed;
                }

                player.MoveDirection = direction;
                player.LookDirection = direction;
            }
        }
        else
        {
            // Slow down smoothly when no input is provided.
            player.CurrentSpeed -= Mathf.Min(player.CurrentSpeed, fric * Time.deltaTime);
        }

        player.FaceDirection();
        player.Anim.SetFloat("CurrentSpeed", player.CurrentSpeed);

        // Jump force can also be increased based on agility.
        float jumpSpeed = baseJumpSpeed + (addJumpSpeed * (player.Stats.stats["AGI"].Value / player.Stats.stats["MAX_AGI"].Value));

        // Trigger a jump only once per button press.
        if (InputHub.Instance.Jump && canJump)
        {
            player.VerticalSpeed = jumpSpeed;
        }

        canJump = !InputHub.Instance.Jump;
    }

    public override void ChangeState(PlayerController player)
    {
        // Exit the grounded state if the player is moving upward or if ground contact is lost.
        if (player.VerticalSpeed > 0f || !player.CheckCollision(player.GroundOffset, player.CheckRadius, player.GroundLayer))
        {
            player.SetState(player.AirState);
        }
    }

    public override void EndState(PlayerController player)
    {
        canAttack = false;
    }
}
