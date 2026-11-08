using UnityEngine;

/// <summary>
/// State used when the player is knocked back by an enemy or hazard.
/// It applies a brief directional knockback and a vertical knockup before returning to the grounded state.
/// </summary>
[System.Serializable]
public class PlayerHitState : PlayerState
{
    // Horizontal force used to push the player away from the hit source.
    [SerializeField] private float knockbackSpeed;

    // Vertical impulse applied immediately when the hit begins.
    [SerializeField] private float knockupSpeed;

    // How long the knockback state lasts before it automatically ends.
    [SerializeField] private float knockbackDuration;

    // Gravity pulling the player back down during the knockback state.
    [SerializeField] private float gravity;

    // Remaining time left in the knockback effect.
    private float knockbackTimer;

    public override void StartState(PlayerController player)
    {
        // Start the knockback timer and set the initial movement values.
        knockbackTimer = knockbackDuration;
        player.MoveDirection = player.transform.forward;
        player.LookDirection = player.transform.forward;
        player.CurrentSpeed = -knockbackSpeed;
        player.VerticalSpeed = knockupSpeed;
    }

    public override void UpdateState(PlayerController player)
    {
        // Countdown the hit state and apply gravity so the player falls back to the ground.
        knockbackTimer -= Time.deltaTime;
        player.VerticalSpeed -= gravity * Time.deltaTime;
    }

    public override void ChangeState(PlayerController player)
    {
        // Exit the hit state when the timer runs out or when the player lands on the ground.
        if (knockbackTimer <= 0f || (player.VerticalSpeed <= 0f && player.CheckCollision(player.transform.position + player.GroundOffset, player.CheckRadius, player.GroundLayer)))
        {
            player.SetState(player.GroundState);
        }
    }

    public override void EndState(PlayerController player)
    {
        // No cleanup is required for this state right now.
    }
}
