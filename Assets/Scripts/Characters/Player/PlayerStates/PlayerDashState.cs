using UnityEngine;

/// <summary>
/// State handler that moves the character forward rapidly for a fixed duration.
/// </summary>
[System.Serializable]
public class PlayerDashState : PlayerState
{
    [Header("Dash Tuning")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;

    private float currentTime;

    [Header("Ground Boundaries")]
    [SerializeField] private Vector3 groundOffset;
    [SerializeField] private float groundCheckRadius;

    public override void StartState(PlayerController player)
    {
        // Dash toward current stick input, or default to the player's forward direction if neutral
        player.MoveDirection = player.GetInputDirection() != Vector3.zero ? player.GetInputDirection() : player.transform.forward;
        player.LookDirection = player.MoveDirection;
        currentTime = dashTime;
    }

    public override void UpdateState(PlayerController player)
    {
        player.FaceDirection();
        player.CurrentSpeed = dashSpeed;
        currentTime -= Time.deltaTime; // Tick down the dash window
    }

    public override void ChangeState(PlayerController player)
    {
        // Return to ground if duration expires or player releases the dash early
        if (!InputHub.Instance.Dash || currentTime <= 0f)
        {
            player.SetState(player.GroundState);
        }

        // Cancel dash into air state if player speeds upward or dashes off a cliff
        if (player.VerticalSpeed > 0f || !player.CheckCollitions(groundOffset, groundCheckRadius))
        {
            player.SetState(player.AirState);
        }
    }

    public override void ExitState(PlayerController player)
    {
        // Intentional cleanup placeholder
    }
}
