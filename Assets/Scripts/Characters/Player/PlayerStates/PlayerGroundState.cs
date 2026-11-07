using UnityEngine;

/// <summary>
/// Handles movement and jumping while the player is grounded.
/// This state accelerates movement, slows the player when no input is pressed, and triggers a jump when conditions are met.
/// </summary>
[System.Serializable]
public class PlayerGroundState : PlayerState
{
    [Header("Movement Settings")]
    // Base movement speed while grounded.
    [SerializeField] private float baseMaxSpeed = 5f;

    // Extra speed gained from agility stat scaling.
    [SerializeField] private float addSpeed = 7f;

    // Rate at which ground movement accelerates.
    [SerializeField] private float accel = 15f;

    // Rate at which the player slows down when turning or stopping.
    [SerializeField] private float decel = 20f;

    // Friction applied when there is no movement input.
    [SerializeField] private float fric = 10f;

    // Angle threshold used to determine if the player should rotate before moving.
    [SerializeField] private float turnAngle = 90f;

    // Current calculated movement speed for this frame.
    private float moveSpeed;

    [Header("Vertical Settings")]
    // Downward force applied while grounded to keep the player attached to the ground.
    [SerializeField] private float stickForce = -5f;

    // Base vertical jump speed.
    [SerializeField] private float baseJumpSpeed = 7f;

    // Extra jump velocity added based on agility.
    [SerializeField] private float addJumpSpeed = 5f;

    // Prevents repeated jump triggers while the jump button is held down.
    private bool canJump;

    public override void StartState(PlayerController player)
    {
        // Keep the player anchored to the ground on entry.
        player.VerticalSpeed = stickForce;

        // Reset jump buffering so the next press can trigger a jump.
        canJump = false;
    }

    public override void UpdateState(PlayerController player)
    {
        // Speed scales with agility so faster characters move faster.
        float maxSpeed = baseMaxSpeed + (addSpeed * (player.Stats.stats["AGI"] / player.Stats.stats["MAX_AGI"]));
        moveSpeed = maxSpeed * InputHub.Instance.Move.magnitude;

        Vector3 direction = player.GetDirection();

        if (direction != Vector3.zero)
        {
            // If the input direction is far from the current facing direction, reduce speed and rotate.
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

        // Jump force can also be increased based on agility.
        float jumpSpeed = baseJumpSpeed + (addJumpSpeed * (player.Stats.stats["AGI"] / player.Stats.stats["MAX_AGI"]));

        // Trigger jump only once per button press.
        if (InputHub.Instance.Jump && canJump)
        {
            player.VerticalSpeed = jumpSpeed;
        }

        canJump = !InputHub.Instance.Jump;
    }

    public override void ChangeState(PlayerController player)
    {
        // Leave grounded state if the player has vertical speed upward or has lost ground contact.
        if (player.VerticalSpeed > 0f || !player.CheckCollision(player.GroundOffset, player.CheckRadius, player.GroundLayer))
        {
            player.SetState(player.AirState);
        }
    }

    public override void EndState(PlayerController player)
    {
        // Cleanup ground state logic here
    }
}
