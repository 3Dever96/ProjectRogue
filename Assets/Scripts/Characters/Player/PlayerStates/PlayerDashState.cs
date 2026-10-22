using UnityEngine;

[System.Serializable]
public class PlayerDashState : PlayerState
{
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;

    private float currentTime;

    [SerializeField] private Vector3 groundOffset;
    [SerializeField] private float groundCheckRadius;

    public override void StartState(PlayerController player)
    {
        player.MoveDirection = player.GetInputDirection() != Vector3.zero ? player.GetInputDirection() : player.transform.forward;
        player.LookDirection = player.MoveDirection;
        currentTime = dashTime;
    }

    public override void UpdateState(PlayerController player)
    {
        player.FaceDirection();
        player.CurrentSpeed = dashSpeed;
        currentTime -= Time.deltaTime;
    }

    public override void ChangeState(PlayerController player)
    {
        if (!InputHub.Instance.Dash || currentTime <= 0f)
        {
            player.SetState(player.GroundState);
        }

        if (player.VerticalSpeed > 0f || !player.CheckCollitions(groundOffset, groundCheckRadius))
        {
            player.SetState(player.AirState);
        }
    }

    public override void ExitState(PlayerController player)
    {

    }
}
