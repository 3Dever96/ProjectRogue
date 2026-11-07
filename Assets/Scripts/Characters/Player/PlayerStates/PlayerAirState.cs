using System;
using UnityEngine;

[System.Serializable]
public class PlayerAirState : PlayerState
{
    [SerializeField] private float gravity = 20f;

    public override void StartState(PlayerController player)
    {
        // Initialize air state logic here
    }

    public override void UpdateState(PlayerController player)
    {
        player.FaceDirection();

        if (!InputHub.Instance.Jump || player.CheckCollision(player.HeadOffset, player.CheckRadius, player.GroundLayer))
        {
            player.VerticalSpeed = Mathf.Min(player.VerticalSpeed, 0f);
        }

        player.VerticalSpeed -= gravity * Time.deltaTime;
    }
    public override void ChangeState(PlayerController player)
    {
        if (player.VerticalSpeed <= 0f && player.CheckCollision(player.GroundOffset, player.CheckRadius, player.GroundLayer))
        {
            player.SetState(player.GroundState);
        }
    }

    public override void EndState(PlayerController player)
    {
        // Cleanup air state logic here
    }
}
