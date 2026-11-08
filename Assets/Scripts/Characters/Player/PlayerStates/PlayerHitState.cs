using UnityEngine;

[System.Serializable]
public class PlayerHitState : PlayerState
{
    [SerializeField] private float knockbackSpeed;
    [SerializeField] private float knockupSpeed;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private float gravity;

    private float knockbackTimer;

    public override void StartState(PlayerController player)
    {
        knockbackTimer = knockbackDuration;
        player.MoveDirection = player.transform.forward;
        player.LookDirection = player.transform.forward;
        player.CurrentSpeed = -knockbackSpeed;
        player.VerticalSpeed = knockupSpeed;
    }

    public override void UpdateState(PlayerController player)
    {
        knockbackTimer -= Time.deltaTime;

        player.VerticalSpeed -= gravity * Time.deltaTime;
    }

    public override void ChangeState(PlayerController player)
    {
        if (knockbackTimer <= 0f || (player.VerticalSpeed <= 0f && player.CheckCollision(player.transform.position + player.GroundOffset, player.CheckRadius, player.GroundLayer)))
        {
            player.SetState(player.GroundState);
        }
    }

    public override void EndState(PlayerController player)
    {
        
    }
}
