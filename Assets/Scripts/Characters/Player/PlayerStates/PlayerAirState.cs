using System;
using UnityEngine;

/// <summary>
/// Handles the player's behavior while they are airborne.
/// This state applies gravity, restricts upward movement near head collisions, and transitions back to ground movement when landing.
/// </summary>
[System.Serializable]
public class PlayerAirState : PlayerState
{
    // Gravity force applied every frame while the player is in the air.
    [SerializeField] private float gravity = 20f;

    public override void StartState(PlayerController player)
    {
        canAttack = true;

        player.Anim.SetBool("IsGrounded", false);
    }

    public override void UpdateState(PlayerController player)
    {
        // Continue facing the current movement direction while airborne.
        player.FaceDirection();

        // If the jump button is released, or the player hits an overhead obstacle, stop upward movement.
        if (!InputHub.Instance.Jump || player.CheckCollision(player.HeadOffset, player.CheckRadius, player.GroundLayer))
        {
            player.VerticalSpeed = Mathf.Min(player.VerticalSpeed, 0f);
        }

        // Apply gravity to pull the player downward over time.
        player.VerticalSpeed -= gravity * Time.deltaTime;
        player.Anim.SetFloat("VerticalSpeed", player.VerticalSpeed);
    }

    public override void ChangeState(PlayerController player)
    {
        // Return to grounded state once the player is descending and has contact with the ground.
        if (player.VerticalSpeed <= 0f && player.CheckCollision(player.GroundOffset, player.CheckRadius, player.GroundLayer))
        {
            player.SetState(player.GroundState);
        }
    }

    public override void EndState(PlayerController player)
    {
        canAttack = false;
    }
}
