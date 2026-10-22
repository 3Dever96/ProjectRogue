using UnityEngine;

[System.Serializable]
public class PlayerAirState : PlayerState
{
    [SerializeField] private float airMoveSpeed;
    [SerializeField] private float doubleJumpSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float fallSpeed;

    [SerializeField] private Vector3 headOffset;
    [SerializeField] private Vector3 groundOffset;
    [SerializeField] private float collisionRadius;

    private bool canDoubleJump;
    private bool canJump;

    public override void StartState(PlayerController player)
    {
        canDoubleJump = true;
        canJump = false;
    }

    public override void UpdateState(PlayerController player)
    {
        if (!InputHub.Instance.Jump || player.CheckCollitions(headOffset, collisionRadius))
        {
            player.VerticalSpeed = Mathf.Min(0f, player.VerticalSpeed);
        }

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

            canJump = !InputHub.Instance.Jump;
        }

        player.FaceDirection();

        if (player.VerticalSpeed > fallSpeed)
        {
            player.VerticalSpeed += gravity * Time.deltaTime;
        }
    }

    public override void ChangeState(PlayerController player)
    {
        if (player.VerticalSpeed <= 0f && player.CheckCollitions(groundOffset, collisionRadius))
        {
            player.SetState(player.GroundState);
        }
    }

    public override void ExitState(PlayerController player)
    {

    }
}
