using UnityEngine;

[System.Serializable]
public class PlayerGroundState : PlayerState
{
    [Header("Movement Settings")]
    [SerializeField] private float baseMaxSpeed = 5f;
    [SerializeField] private float addSpeed = 7f;
    [SerializeField] private float accel = 15f;
    [SerializeField] private float decel = 20f;
    [SerializeField] private float fric = 10f;
    [SerializeField] private float turnAngle = 90f;

    private float moveSpeed;

    [Header("Vertical Settings")]
    [SerializeField] private float stickForce = -5f;
    [SerializeField] private float baseJumpSpeed = 7f;
    [SerializeField] private float addJumpSpeed = 5f;
    
    private bool canJump;

    public override void StartState(PlayerController player)
    {
        player.VerticalSpeed = stickForce;

        canJump = false;
    }

    public override void UpdateState(PlayerController player)
    {
        float maxSpeed = baseMaxSpeed + (addSpeed * (player.Stats.stats["AGI"] / player.Stats.stats["MAX_AGI"]));
        moveSpeed = maxSpeed * InputHub.Instance.Move.magnitude;

        Vector3 direction = player.GetDirection();

        if (direction != Vector3.zero)
        {
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
            player.CurrentSpeed -= Mathf.Min(player.CurrentSpeed, fric * Time.deltaTime);
        }

        player.FaceDirection();

        float jumpSpeed = baseJumpSpeed + (addJumpSpeed * (player.Stats.stats["AGI"] / player.Stats.stats["MAX_AGI"]));

        if (InputHub.Instance.Jump && canJump)
        {
            player.VerticalSpeed = jumpSpeed;
        }

        canJump = !InputHub.Instance.Jump;
    }

    public override void ChangeState(PlayerController player)
    {
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
